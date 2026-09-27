// rev 2026-09-26

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для работы с объектами <see cref="DbSet{TEntity}"/>.
	/// </summary>
	public static partial class Exts_DbSet
	{

		/// <summary>
		/// Возвращает экземпляр контекста базы данных <see cref="DbContext"/>, к которому принадлежит текущий набор данных <see cref="DbSet{TEntity}"/>.
		/// </summary>
		/// <typeparam name="TEntity">Тип доменной сущности, управляемой набором данных. Должен быть ссылочным типом (<see langword="class"/>).</typeparam>
		/// <param name="dbSet">Текущий экземпляр набора данных, для которого запрашивается контекст.</param>
		/// <returns>
		/// Связанный экземпляр <see cref="DbContext"/> или <see langword="null"/>, если контекст не удалось извлечь из внутренней инфраструктуры EF Core.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DbContext? GetDbContext<TEntity>(
			this DbSet<TEntity> dbSet)
			where TEntity : class
		{
			var infrastructure1 = dbSet as IInfrastructure<IServiceProvider>;
			var provider1 = infrastructure1?.Instance;
			var currentDbContext1 = provider1?.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
			return currentDbContext1?.Context;
		}

	}

}
