// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Модель данных, объединяющая параметры многокритериальной сортировки 
	/// и динамической пагинации для гибкого применения к LINQ-запросам.
	/// </summary>
	public class PaginatedDataModel
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PaginatedDataModel"/>, проводя валидацию 
		/// и автоматический расчет параметров пагинации с защитой от перегрузки памяти (DoS).
		/// </summary>
		/// <param name="order">Строка определения полей и направлений сортировки (например, <c>"LastName,-Age"</c>).</param>
		/// <param name="page">Порядковый номер запрашиваемой страницы. Если передано значение меньше 1, оно принудительно устанавливается в 1.</param>
		/// <param name="itemsOnPage">Запрашиваемое количество элементов на странице. Если равно 0, берется значение из <paramref name="defaultItemsOnPage"/>; если меньше 0, рассчитывается максимальный вывод.</param>
		/// <param name="totalItems">Общее количество доступных элементов в исходной коллекции данных.</param>
		/// <param name="defaultItemsOnPage">Количество элементов на странице по умолчанию, используемое в качестве fallback-значения.</param>
		/// <param name="maxItemsOnPages">Максимально допустимое количество элементов на странице для жесткого ограничения размера выборки. Если установлено в 0 или меньше, лимит не применяется.</param>
		public PaginatedDataModel(
			string? order,
			int page,
			int itemsOnPage,
			int totalItems,
			int defaultItemsOnPage,
			int maxItemsOnPages)
		{
			Order = string.IsNullOrEmpty(order)
				? null : new OrderBuilder(order);
			Page = page < 1
				? 1 : page;
			TotalItems = totalItems;
			if (itemsOnPage == 0)
				itemsOnPage = defaultItemsOnPage;
			else if (itemsOnPage < 0)
				itemsOnPage = maxItemsOnPages < 1
					? totalItems : maxItemsOnPages;
			if (maxItemsOnPages > 0 && itemsOnPage > maxItemsOnPages)
				itemsOnPage = maxItemsOnPages;
			ItemsOnPage = itemsOnPage;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает объект построителя многокритериальной сортировки.
		/// </summary>
		/// <value>
		/// Экземпляр класса <see cref="OrderBuilder"/>, инкапсулирующий правила сортировки, 
		/// или <see langword="null"/>, если параметры сортировки не были заданы.
		/// </value>
		public OrderBuilder? Order { get; }


		/// <summary>
		/// Возвращает номер текущей запрашиваемой страницы. Индексация начинается с 1.
		/// </summary>
		/// <value>Целочисленное значение номера страницы. Гарантированно не меньше 1.</value>
		public int Page { get; }


		/// <summary>
		/// Возвращает итоговое рассчитанное количество отображаемых элементов на одной странице.
		/// </summary>
		/// <value>Целочисленный размер страницы, скорректированный с учетом ограничений безопасности.</value>
		public int ItemsOnPage { get; }


		/// <summary>
		/// Возвращает общее количество доступных элементов в исходной коллекции.
		/// </summary>
		/// <value>Целочисленное общее количество элементов.</value>
		public int TotalItems { get; }


		/// <summary>
		/// Возвращает признак наличия активных параметров сортировки.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если свойство <see cref="Order"/> не равно <see langword="null"/>; в противном случае — <see langword="false"/>.</value>
		public bool HasOrder
			=> Order != null;


		/// <summary>
		/// Возвращает признак активности и применимости параметров постраничной навигации.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если размер страницы и её номер строго больше нуля; в противном случае — <see langword="false"/>.</value>
		public bool HasPage
			=> ItemsOnPage > 0 && Page > 0;


		/* functions */


		/// <summary>
		/// Применяет к исходному LINQ-запросу последовательно операции сортировки и ограничения постраничной пагинации.
		/// </summary>
		/// <typeparam name="T">Тип доменной сущности или DTO в обрабатываемом запросе.</typeparam>
		/// <param name="query">Исходный запрос <see cref="IQueryable{T}"/> до трансформации.</param>
		/// <returns>Измененный запрос <see cref="IQueryable{T}"/> с последовательно примененной сортировкой и операторами <c>Skip/Take</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IQueryable<T> GetQueryTransform<T>(
			IQueryable<T> query)
		{
			return GetQueryPaging(GetQueryOrdered(query));
		}


		/// <summary>
		/// Применяет к исходному LINQ-запросу параметры многокритериальной сортировки, если они были заданы.
		/// </summary>
		/// <remarks>
		/// Для динамического наложения выражений сортировки привлекается внутренний метод расширения <c>ApplyOrder</c>.
		/// </remarks>
		/// <typeparam name="T">Тип доменной сущности или DTO в обрабатываемом запросе.</typeparam>
		/// <param name="query">Исходный запрос <see cref="IQueryable{T}"/>.</param>
		/// <returns>Запрос <see cref="IQueryable{T}"/> с примененными выражениями сортировки, либо оригинальный запрос без изменений, если <see cref="HasOrder"/> равен <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IQueryable<T> GetQueryOrdered<T>(
			IQueryable<T> query)
		{
			return HasOrder
				? query.ApplyOrder(Order!) : query;
		}


		/// <summary>
		/// Применяет к исходному LINQ-запросу операторы секционирования данных (<c>Skip</c> и <c>Take</c>) на основе текущего состояния пагинации.
		/// </summary>
		/// <remarks>
		/// Количество пропускаемых элементов рассчитывается по классической формуле: <c>(Page - 1) * ItemsOnPage</c>.
		/// </remarks>
		/// <typeparam name="T">Тип доменной сущности или DTO в обрабатываемом запросе.</typeparam>
		/// <param name="query">Исходный запрос <see cref="IQueryable{T}"/>.</param>
		/// <returns>Запрос <see cref="IQueryable{T}"/> с примененными ограничениями выборки диапазона данных, либо оригинальный запрос, если <see cref="HasPage"/> равен <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IQueryable<T> GetQueryPaging<T>(
			IQueryable<T> query)
		{
			return HasPage
				? query
					.Skip((Page - 1) * ItemsOnPage)
					.Take(ItemsOnPage)
				: query;
		}

	}

}
