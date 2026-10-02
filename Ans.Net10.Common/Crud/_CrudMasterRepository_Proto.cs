// rev 2026-10-01

using Microsoft.EntityFrameworkCore;

namespace Ans.Net10.Common.Crud
{

	/// <summary>
	/// Интерфейс сущности, обладающей уникальным числовым идентификатором
	/// и выступающей в роли главной (Master) сущности.
	/// </summary>
	public interface IMasterEntity
	{
		/// <summary>
		/// Уникальный идентификатор сущности.
		/// </summary>
		int Id { get; set; }
	}



	/// <summary>
	/// Интерфейс репозитория для работы с главными сущностями, реализующими <see cref="IMasterEntity"/>.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, управляемой репозиторием.</typeparam>
	public interface ICrudMasterRepository<T>
		: ICrudRepository<T>
		where T : class, IMasterEntity
	{
		/// <summary>
		/// Возвращает новый, инициализированный по умолчанию экземпляр сущности.
		/// </summary>
		/// <returns>Новый экземпляр сущности типа <typeparamref name="T"/>.</returns>
		T GetNew();

		/// <summary>
		/// Асинхронно возвращает общее количество всех главных сущностей данного типа в базе данных.
		/// </summary>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом выполнения которой является общее количество записей в таблице.</returns>
		Task<int> GetItemsCountAsync(CancellationToken cancellationToken = default);
	}



	/// <summary>
	/// Абстрактный прототип класса для реализации репозиториев CRUD-операций
	/// над главными сущностями с использованием Entity Framework Core.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, реализующей <see cref="IMasterEntity"/>.</typeparam>
	/// <param name="db">Экземпляр контекста базы данных <see cref="DbContext"/>.</param>
	public abstract class _CrudMasterRepository_Proto<T>(
		DbContext db)
		: __CrudRepository_Base<T>(db),
		ICrudMasterRepository<T>
		where T : class, IMasterEntity
	{

		/// <inheritdoc />
		public abstract T GetNew();


		/// <inheritdoc />
		public virtual async Task<int> GetItemsCountAsync(
			CancellationToken cancellationToken = default)
		{
			return await base.GetItemsCountAsync(null, cancellationToken);
		}

	}

}
