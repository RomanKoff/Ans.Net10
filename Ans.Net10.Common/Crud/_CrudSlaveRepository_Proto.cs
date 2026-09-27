// rev 2026-09-21

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
		T GetNew(int masterPtr);

		/// <summary>
		/// Формирует запрос <see cref="IQueryable{T}"/> без отслеживания изменений,
		/// жестко ограниченный рамками одного владельца.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="filter">
		/// Дополнительное выражение фильтрации. Если <see langword="null"/>,
		/// выборка ограничивается только по <paramref name="masterPtr"/>.
		/// </param>
		IQueryable<T> GetItemsAsQueryable(int masterPtr, Expression<Func<T, bool>>? filter);

		/// <summary>
		/// Возвращает общее количество всех подчиненных сущностей, принадлежащих указанному владельцу.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		int GetItemsCount(int masterPtr);
	}



	/// <summary>
	/// Абстрактный прототип класса для реализации репозиториев CRUD-операций
	/// над подчиненными сущностями с использованием Entity Framework.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, реализующей <see cref="ISlaveEntity"/>.</typeparam>
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual int GetItemsCount(
			int masterPtr)
		{
			return base.GetItemsCount(x => x.MasterPtr == masterPtr);
		}

	}

}
