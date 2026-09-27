// rev 2026-09-26

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для динамического построения LINQ-выражений и подготовки запросов.
	/// </summary>
	public static class SuppLinq
	{

		/// <summary>
		/// Безопасно объединяет существующий фильтр с новым предикатом по логическому оператору "И" (<see cref="Expression.AndAlso(Expression, Expression)"/>). 
		/// Если исходный фильтр не задан, возвращает новое выражение.
		/// </summary>
		/// <typeparam name="T">Тип фильтруемой доменной сущности.</typeparam>
		/// <param name="filter">Текущее составное выражение-фильтр. Допускает значение <see langword="null"/>.</param>
		/// <param name="expression">Новое добавляемое предикатное выражение.</param>
		/// <returns>Новое объединенное дерево выражений типа <see cref="Expression{TDelegate}"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Expression<Func<T, bool>> ApplyFilter_Add<T>(
			Expression<Func<T, bool>>? filter,
			Expression<Func<T, bool>> expression)
		{
			return filter == null
				? expression : filter.And(expression);
		}


		/// <summary>
		/// Готовит и модифицирует LINQ-запрос: последовательно объединяет массив фильтров, применяет их, 
		/// рассчитывает метаданные пагинации на основе общего количества записей в БД, 
		/// а также накладывает правила сортировки и постраничного секционирования (Skip/Take).
		/// </summary>
		/// <typeparam name="T">Тип сущности в обрабатываемом запросе.</typeparam>
		/// <param name="query">Исходный запрос <see cref="IQueryable{T}"/>. Допускает значение <see langword="null"/>.</param>
		/// <param name="filters">Массив предикатных выражений для фильтрации данных. Допускает значение <see langword="null"/>.</param>
		/// <param name="order">Строка определения полей и направлений сортировки (например, <c>"Name,-Id"</c>).</param>
		/// <param name="page">Порядковый номер запрашиваемой страницы (индексация начинается с 1).</param>
		/// <param name="itemsOnPage">Запрашиваемое количество элементов на одной странице.</param>
		/// <returns>Модифицированный запрос с примененными фильтрами, сортировкой и ограничениями пагинации.</returns>
		public static IQueryable<T> PrepareQuery<T>(
			IQueryable<T>? query,
			Expression<Func<T, bool>>[]? filters,
			string order,
			int page,
			int itemsOnPage)
		{
			if (query == null)
				return Enumerable.Empty<T>().AsQueryable();
			Expression<Func<T, bool>>? compositeFilter = null;
			if (filters != null && filters.Length > 0)
			{
				compositeFilter = filters[0];
				for (int i1 = 1; i1 < filters.Length; i1++)
					if (filters[i1] != null)
						compositeFilter = compositeFilter.And(filters[i1]);
			}
			if (compositeFilter != null)
				query = query.Where(compositeFilter);
			var pagination1 = new PaginatedDataModel(
				order, page, itemsOnPage, query.Count(), 25, 500);
			return pagination1.GetQueryTransform(query);
		}

	}

}
