// rev 2026-09-21

using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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
		T GetNew();

		/// <summary>
		/// Возвращает общее количество всех сущностей данного типа в базе данных.
		/// </summary>
		int GetItemsCount();
	}



	/// <summary>
	/// Абстрактный прототип класса для реализации репозиториев CRUD-операций
	/// над главными сущностями с использованием Entity Framework.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, реализующей <see cref="IMasterEntity"/>.</typeparam>
	public abstract class _CrudMasterRepository_Proto<T>(
		DbContext db)
		: __CrudRepository_Base<T>(db),
		ICrudMasterRepository<T>
		where T : class, IMasterEntity
	{

		/// <inheritdoc />
		public abstract T GetNew();


		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual int GetItemsCount()
		{
			return base.GetItemsCount(null);
		}

	}

}
