// rev 2026-10-01

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ans.Net10.Common.Crud
{

	/// <summary>
	/// Интерфейс подчиненной (Slave) сущности, жестко связанной с главной через внешний ключ.
	/// </summary>
	public interface ISlaveEntity
		: IMasterEntity
	{
		/// <summary>
		/// Идентификатор главной (владеющей) сущности.
		/// </summary>
		int MasterPtr { get; set; }
	}



	/// <summary>
	/// Интерфейс репозитория для работы с подчиненными сущностями, реализующими <see cref="ISlaveEntity"/>.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, управляемой репозиторием.</typeparam>
	public interface ICrudSlaveRepository<T>
		: ICrudRepository<T>
		where T : class, ISlaveEntity
	{
		/// <summary>
		/// Возвращает новый, инициализированный по умолчанию экземпляр подчиненной сущности для указанного владельца.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <returns>Новый экземпляр подчиненной сущности типа <typeparamref name="T"/>.</returns>
		T GetNew(int masterPtr);

		/// <summary>
		/// Формирует запрос <see cref="IQueryable{T}"/> без отслеживания изменений,
		/// жестко ограниченный рамками одного владельца.
		/// </summary>
		/// <remarks>
		/// Метод возвращает ленивый запрос, к которому в дальнейшем можно асинхронно применить методы материализации.
		/// </remarks>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="filter">
		/// Дополнительное выражение фильтрации. Если <see langword="null"/>,
		/// выборка ограничивается только по <paramref name="masterPtr"/>.
		/// </param>
		/// <returns>Запрос <see cref="IQueryable{T}"/> для извлечения подчиненных сущностей.</returns>
		IQueryable<T> GetItemsAsQueryable(int masterPtr, Expression<Func<T, bool>>? filter);

		/// <summary>
		/// Асинхронно возвращает общее количество всех подчиненных сущностей, принадлежащих указанному владельцу.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом выполнения которой является количество записей, связанных с указанным владельцем.</returns>
		Task<int> GetItemsCountAsync(int masterPtr, CancellationToken cancellationToken = default);
	}



	/// <summary>
	/// Абстрактный прототип класса для реализации репозиториев CRUD-операций
	/// над подчиненными сущностями с использованием Entity Framework Core.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, реализующей <see cref="ISlaveEntity"/>.</typeparam>
	/// <param name="db">Экземпляр контекста базы данных <see cref="DbContext"/>.</param>
	public abstract class _CrudSlaveRepository_Proto<T>(
		DbContext db)
		: __CrudRepository_Base<T>(db),
		ICrudSlaveRepository<T>
		where T : class, ISlaveEntity
	{

		/// <inheritdoc />
		public abstract T GetNew(
			int masterPtr);


		/// <inheritdoc />
		public virtual IQueryable<T> GetItemsAsQueryable(
			int masterPtr,
			Expression<Func<T, bool>>? filter)
		{
			filter = filter == null
				? (x => x.MasterPtr == masterPtr)
				: filter.And(x => x.MasterPtr == masterPtr);
			return DbSet.AsNoTracking().Where(filter);
		}


		/// <inheritdoc />
		public virtual async Task<int> GetItemsCountAsync(
			int masterPtr,
			CancellationToken cancellationToken = default)
		{
			return await base.GetItemsCountAsync(
				x => x.MasterPtr == masterPtr, cancellationToken);
		}

	}

}
