// rev 2026-09-25

using Microsoft.EntityFrameworkCore;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для одновременного асинхронного расчета метаданных пагинации 
	/// и формирования безопасного постраничного LINQ-запроса на основе исходной выборки.
	/// </summary>
	/// <typeparam name="TEntity">Тип доменной сущности или DTO в обрабатываемом запросе. Должен относиться к ссылочным типам (<see langword="class"/>).</typeparam>
	public class PaginatedQueryableHelper<TEntity>
		where TEntity : class
	{

		/* ctor */


		/// <summary>
		/// Асинхронно создает и инициализирует новый экземпляр класса <see cref="PaginatedQueryableHelper{TEntity}"/>, 
		/// выполняя неблокирующий расчет общего количества элементов в базе данных и подготавливая постраничный подзапрос.
		/// </summary>
		/// <remarks>
		/// Метод производит первичную материализацию общего количества записей через асинхронный вызов <see cref="EntityFrameworkQueryableExtensions.CountAsync{TSource}(IQueryable{TSource}, CancellationToken)"/>. 
		/// Если параметры <paramref name="page"/> или <paramref name="itemsOnPage"/> переданы со значением меньше 1, 
		/// они принудительно корректируются и устанавливаются в значение 1 для обеспечения стабильности вычислений.
		/// </remarks>
		/// <param name="query">Исходный запрос <see cref="IQueryable{TEntity}"/> до применения операторов пагинации.</param>
		/// <param name="page">Порядковый номер запрашиваемой страницы. Индексация начинается с 1.</param>
		/// <param name="itemsOnPage">Максимально допустимое количество отображаемых элементов на одной странице. Должно быть больше 0.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом выполнения которой является полностью инициализированный хелпер пагинации запроса.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="query"/> равен <see langword="null"/>.</exception>
		public static async Task<PaginatedQueryableHelper<TEntity>> CreateAsync(
			IQueryable<TEntity> query,
			int page,
			int itemsOnPage,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(query);
			page = page < 1
				? 1 : page;
			itemsOnPage = itemsOnPage < 1
				? 1 : itemsOnPage;
			int totalItems1 = await query.CountAsync(cancellationToken);
			return new PaginatedQueryableHelper<TEntity>(
				query, page, itemsOnPage, totalItems1);
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="PaginatedQueryableHelper{TEntity}"/>.
		/// Конструктор закрыт, для асинхронного создания экземпляра используйте метод <see cref="CreateAsync"/>.
		/// </summary>
		private PaginatedQueryableHelper(
			IQueryable<TEntity> query,
			int page,
			int itemsOnPage,
			int totalItems)
		{
			Query = query
				.Skip((page - 1) * itemsOnPage)
				.Take(itemsOnPage);
			PaginationHelper = new PaginationHelper(
				itemsOnPage, totalItems, page);
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
