// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для одновременного расчета метаданных пагинации 
	/// и формирования безопасного постраничного LINQ-запроса на основе исходной выборки.
	/// </summary>
	/// <typeparam name="TEntity">Тип доменной сущности или DTO в обрабатываемом запросе. Должен относиться к ссылочным типам (<see langword="class"/>).</typeparam>
	public class PaginatedQueryableHelper<TEntity>
		where TEntity : class
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PaginatedQueryableHelper{TEntity}"/>, 
		/// выполняя синхронный расчет общего количества элементов в базе данных и подготавливая постраничный подзапрос.
		/// </summary>
		/// <remarks>
		/// Метод производит первичную материализацию общего количества записей через стандартный вызов <c>.Count()</c>. 
		/// Если параметры <paramref name="page"/> или <paramref name="itemsOnPage"/> переданы со значением меньше 1, 
		/// они принудительно корректируются и устанавливаются в значение 1 для обеспечения стабильности вычислений.
		/// </remarks>
		/// <param name="query">Исходный запрос <see cref="IQueryable{TEntity}"/> до применения операторов пагинации.</param>
		/// <param name="page">Порядковый номер запрашиваемой страницы. Индексация начинается с 1.</param>
		/// <param name="itemsOnPage">Максимально допустимое количество отображаемых элементов на одной странице. Должно быть больше 0.</param>
		public PaginatedQueryableHelper(
			IQueryable<TEntity> query,
			int page,
			int itemsOnPage)
		{
			page = page < 1
				? 1 : page;
			itemsOnPage = itemsOnPage < 1
				? 1 : itemsOnPage;
			var totalItems1 = query.Count();
			Query = query
				.Skip((page - 1) * itemsOnPage)
				.Take(itemsOnPage);
			PaginationHelper = new PaginationHelper(
				itemsOnPage,
				totalItems1,
				page);
		}


		/* reaonly properties */


		/// <summary>
		/// Возвращает вычисленный хелпер навигации пагинации с актуальными состояниями, флагами и диапазонами страниц.
		/// </summary>
		/// <value>
		/// Наполненный объект класса <see cref="PaginationHelper"/>, содержащий полные метаданные для рендеринга элементов управления интерфейса.
		/// </value>
		public PaginationHelper PaginationHelper { get; }


		/// <summary>
		/// Возвращает модифицированный запрос с примененными операторами секционирования данных.
		/// </summary>
		/// <remarks>
		/// Запрос содержит наложенные методы фильтрации <see cref="Queryable.Skip{TSource}(IQueryable{TSource}, int)"/> 
		/// и <see cref="Queryable.Take{TSource}(IQueryable{TSource}, int)"/>, готовые к ленивой материализации.
		/// </remarks>
		/// <value>
		/// Трансформированный запрос <see cref="IQueryable{TEntity}"/>, представляющий срез данных для текущей страницы.
		/// </value>
		public IQueryable<TEntity> Query { get; }

	}

}
