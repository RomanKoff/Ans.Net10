// rev 2026-10-01

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ans.Net10.Common.Crud
{

	/// <summary>
	/// Интерфейс универсального репозитория для выполнения базовых асинхронных операций
	/// чтения и синхронного управления состояниями сущностей типа <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, управляемой репозиторием. Должен быть ссылочным типом (<see langword="class"/>).</typeparam>
	public interface ICrudRepository<T>
		where T : class
	{
		/* readonly properties */

		/// <summary>
		/// Возвращает экземпляр контекста базы данных Entity Framework Core.
		/// </summary>
		/// <value>Объект контекста <see cref="DbContext"/>.</value>
		DbContext DbContext { get; }

		/// <summary>
		/// Возвращает набор сущностей <see cref="DbSet{T}"/> для работы с текущим типом данных.
		/// </summary>
		/// <value>Объект набора данных <see cref="DbSet{T}"/>.</value>
		DbSet<T> DbSet { get; }

		/* functions */

		/// <summary>
		/// Формирует запрос <see cref="IQueryable{T}"/> без отслеживания изменений (AsNoTracking) с применением фильтрации.
		/// </summary>
		/// <param name="filter">Предикатное выражение для фильтрации сущностей. Если равен <see langword="null"/>, фильтрация не применяется.</param>
		/// <returns>Запрос <see cref="IQueryable{T}"/> для извлечения сущностей.</returns>
		IQueryable<T> GetItemsAsQueryable(Expression<Func<T, bool>>? filter);

		/// <summary>
		/// Асинхронно возвращает сущность по её уникальному целочисленному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор искомой сущности.</param>
		/// <returns>Задача, результатом которой является найденная сущность типа <typeparamref name="T"/> или <see langword="null"/>, если запись не найдена.</returns>
		Task<T?> GetItemAsync(int id);

		/// <summary>
		/// Асинхронно возвращает первую сущность, удовлетворяющую условию фильтра, без отслеживания изменений.
		/// </summary>
		/// <param name="filter">Выражение-фильтр для поиска сущности.</param>
		/// <returns>Задача, результатом которой является первая найденная сущность типа <typeparamref name="T"/> или <see langword="null"/>, если совпадений не найдено.</returns>
		Task<T?> GetItemAsync(Expression<Func<T, bool>> filter);

		/// <summary>
		/// Асинхронно возвращает общее количество сущностей, удовлетворяющих заданному фильтру.
		/// </summary>
		/// <param name="filter">Выражение-фильтр для подсчета. Если равен <see langword="null"/>, подсчитываются все записи в наборе.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом которой является общее количество записей, соответствующих условию.</returns>
		Task<int> GetItemsCountAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default);

		/* methods */

		/// <summary>
		/// Синхронно добавляет новую сущность в контекст со статусом <see cref="EntityState.Added"/>.
		/// </summary>
		/// <param name="entity">Добавляемая сущность.</param>
		void Add(T entity);

		/// <summary>
		/// Синхронно прикрепляет сущность к контексту и помечает все её свойства как измененные (<see cref="EntityState.Modified"/>).
		/// </summary>
		/// <param name="entity">Обновляемая сущность.</param>
		void UpdateEvery(T entity);

		/// <summary>
		/// Синхронно прикрепляет сущность к контексту и помечает как измененные только указанные свойства.
		/// </summary>
		/// <param name="entity">Обновляемая сущность.</param>
		/// <param name="properties">Коллекция системных имен свойств, которые подлежат обновлению.</param>
		void UpdateSelective(T entity, IEnumerable<string> properties);

		/// <summary>
		/// Синхронно помечает указанную сущность для удаления из базы данных.
		/// </summary>
		/// <param name="entity">Удаляемая сущность.</param>
		void Remove(T entity);

		/// <summary>
		/// Асинхронно находит сущность по идентификатору в БД и помечает её для удаления.
		/// </summary>
		/// <param name="id">Идентификатор удаляемой сущности.</param>
		/// <returns>Задача, представляющая асинхронную операцию поиска перед удалением.</returns>
		Task RemoveAsync(int id);

		/// <summary>
		/// Асинхронно добавляет связи «многие ко многим» для указанного главного объекта и набора связанных ключей.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главного (владеющего) объекта.</param>
		/// <param name="keys">Коллекция идентификаторов связываемых объектов.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая асинхронную операцию добавления связей.</returns>
		Task AddManyrefsAsync(int masterPtr, IEnumerable<int> keys, CancellationToken cancellationToken = default);

		/// <summary>
		/// Асинхронно удаляет связи «многие ко многим» для указанного главного объекта и набора связанных ключей.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главного (владеющего) объекта.</param>
		/// <param name="keys">Коллекция идентификаторов отвязываемых объектов.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая асинхронную операцию удаления связей.</returns>
		Task RemoveManyrefsAsync(int masterPtr, IEnumerable<int> keys, CancellationToken cancellationToken = default);

		/// <summary>
		/// Асинхронно синхронизирует связи «многие ко многим», вычисляя добавленные и удаленные ключи.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главного (владеющего) объекта.</param>
		/// <param name="oldKeys">Старый (текущий) набор связанных ключей.</param>
		/// <param name="newKeys">Новый (целевой) набор связанных ключей.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая асинхронную операцию синхронизации связей.</returns>
		Task ManyrefUpdateAsync(int masterPtr, IEnumerable<int> oldKeys, IEnumerable<int> newKeys, CancellationToken cancellationToken = default);
	}



	/// <summary>
	/// Абстрактный прототип базового класса репозиториев CRUD-операций с использованием Entity Framework Core.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, управляемой репозиторием. Должен быть ссылочным типом (<see langword="class"/>).</typeparam>
	public abstract class __CrudRepository_Base<T>(
		DbContext db)
		: ICrudRepository<T>
		where T : class
	{

		/* readonly properties */


		/// <inheritdoc />
		public DbContext DbContext { get; } = db;


		/// <inheritdoc />
		public DbSet<T> DbSet { get; } = db.Set<T>();


		/* virtual functions */


		/// <inheritdoc />
		public virtual IQueryable<T> GetItemsAsQueryable(
			Expression<Func<T, bool>>? filter)
		{
			return filter == null
				? DbSet.AsNoTracking()
				: DbSet.AsNoTracking().Where(filter);
		}


		/// <inheritdoc />
		public virtual async Task<T?> GetItemAsync(
			int id)
		{
			return await DbSet.FindAsync(id);
		}


		/// <inheritdoc />
		public virtual async Task<T?> GetItemAsync(
			Expression<Func<T, bool>> filter)
		{
			ArgumentNullException.ThrowIfNull(filter);
			return await DbSet
				.AsNoTracking()
				.FirstOrDefaultAsync(filter);
		}


		/// <inheritdoc />
		public virtual async Task<int> GetItemsCountAsync(
			Expression<Func<T, bool>>? filter,
			CancellationToken cancellationToken = default)
		{
			return filter == null
				? await DbSet.CountAsync(cancellationToken)
				: await DbSet.Where(filter).CountAsync(cancellationToken);
		}


		/* virtual methods */


		/// <inheritdoc />
		public virtual void Add(
			T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			DbSet.Add(entity);
		}


		/// <inheritdoc />
		public virtual void UpdateEvery(
			T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			DbSet.Attach(entity);
			DbContext.Entry(entity).State = EntityState.Modified;
		}


		/// <inheritdoc />
		public virtual void UpdateSelective(
			T entity,
			IEnumerable<string> properties)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentNullException.ThrowIfNull(properties);
			DbSet.Attach(entity);
			foreach (var property1 in properties)
				DbContext.Entry(entity).Property(property1).IsModified = true;
		}


		/// <inheritdoc />
		public virtual void Remove(
			T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			if (DbContext.Entry(entity).State == EntityState.Detached)
				DbSet.Attach(entity);
			DbSet.Remove(entity);
		}


		/// <inheritdoc />
		public virtual async Task RemoveAsync(
			int id)
		{
			var entity1 = await DbSet.FindAsync(id);
			if (entity1 != null)
				Remove(entity1);
		}


		/// <inheritdoc />
		public virtual Task AddManyrefsAsync(
			int masterPtr,
			IEnumerable<int> keys,
			CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}


		/// <inheritdoc />
		public virtual Task RemoveManyrefsAsync(
			int masterPtr,
			IEnumerable<int> keys,
			CancellationToken cancellationToken = default)
		{
			return Task.CompletedTask;
		}


		/// <inheritdoc />
		public virtual async Task ManyrefUpdateAsync(
			int masterPtr,
			IEnumerable<int> oldKeys,
			IEnumerable<int> newKeys,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(oldKeys);
			ArgumentNullException.ThrowIfNull(newKeys);
			var comparer1 = new KeysComparer(oldKeys, newKeys);
			if (comparer1.HasAdded)
				await AddManyrefsAsync(masterPtr, comparer1.Added, cancellationToken);
			if (comparer1.HasDeleted)
				await RemoveManyrefsAsync(masterPtr, comparer1.Deleted, cancellationToken);
		}

	}

}
