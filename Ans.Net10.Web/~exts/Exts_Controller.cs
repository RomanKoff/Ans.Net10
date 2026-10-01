// rev 2026-010-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для контроллеров <see cref="ControllerBase"/> и <see cref="Controller"/>, 
	/// упрощающие формирование унифицированных ответов API и постраничных выборок данных.
	/// </summary>
	public static partial class Exts_Controller
	{

		/// <summary>
		/// Формирует унифицированный успешный ответ HTTP 200 OK, содержащий модель результата API.
		/// </summary>
		/// <param name="controller">Текущий экземпляр контроллера API.</param>
		/// <param name="title">Заголовок или сообщение об успешном выполнении операции.</param>
		/// <returns>Объект <see cref="IActionResult"/> с успешным статусом ответа и телом <see cref="ApiResultModel"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="controller"/> равен <see langword="null"/>.</exception>
		public static IActionResult GetOkApiResult(
			this ControllerBase controller,
			string title)
		{
			ArgumentNullException.ThrowIfNull(controller);
			var s1 = $"{title} : success";
			Console.WriteLine(s1);
			return controller.Ok(new ApiResultModel(s1));
		}


		/// <summary>
		/// Формирует унифицированный ответ ошибки HTTP 400 Bad Request, содержащий модель результата API.
		/// </summary>
		/// <param name="controller">Текущий экземпляр контроллера API.</param>
		/// <param name="title">Заголовок или описание произошедшей ошибки.</param>
		/// <returns>Объект <see cref="IActionResult"/> со статусом ошибки и телом <see cref="ApiResultModel"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="controller"/> равен <see langword="null"/>.</exception>
		public static IActionResult GetErrorApiResult(
			this ControllerBase controller,
			string title)
		{
			ArgumentNullException.ThrowIfNull(controller);
			var s1 = $"{title} : error";
			Console.WriteLine(s1);
			return controller.BadRequest(new ApiResultModel(s1));
		}


		/// <summary>
		/// Асинхронно формирует постраничную выборку из набора данных <see cref="DbSet{TEntity}"/>, 
		/// внедряет вычисленные метаданные пагинации во <see cref="Controller.ViewData"/> 
		/// и возвращает соответствующее Razor-представление.
		/// </summary>
		/// <remarks>
		/// Метод использует высокопроизводительный асинхронный хелпер <see cref="PaginatedQueryableHelper{TEntity}"/>,
		/// исключая блокировку потоков сервера при вычислении общего количества записей и извлечении среза данных.
		/// </remarks>
		/// <typeparam name="TEntity">Тип доменной сущности, обрабатываемой в наборе данных. Должен быть ссылочным типом.</typeparam>
		/// <param name="controller">Текущий экземпляр MVC-контроллера страниц.</param>
		/// <param name="dbSet">Набор сущностей Entity Framework для построения запроса.</param>
		/// <param name="filter">Опциональное предикатное выражение для предварительной фильтрации данных перед пагинацией.</param>
		/// <param name="order">Строка определения полей и направлений многокритериальной сортировки (например, "LastName,-Age").</param>
		/// <param name="page">Номер запрашиваемой страницы (индексация начинается с 1).</param>
		/// <param name="itemsOnPage">Запрашиваемое количество элементов на одной странице. Если меньше 1, сбрасывается в 1.</param>
		/// <param name="defaultItemsOnPage">Количество элементов на странице по умолчанию, используемое в качестве fallback-значения.</param>
		/// <param name="maxItemsOnPages">Максимально допустимый лимит элементов на одной странице для защиты от DoS-аллокаций.</param>
		/// <param name="viewName">Опциональное имя или полный виртуальный путь к Razor-представлению. Если равен <see langword="null"/>, используется дефолтное представление экшена.</param>
		/// <returns>Задача, результатом выполнения которой является <see cref="IActionResult"/>, содержащий материализованный список элементов страницы.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="controller"/> или <paramref name="dbSet"/> равен <see langword="null"/>.</exception>
		public static async Task<IActionResult> GetPaginatedListAsync<TEntity>(
			this Controller controller,
			DbSet<TEntity> dbSet,
			Expression<Func<TEntity, bool>>? filter,
			string order,
			int page,
			int itemsOnPage,
			int defaultItemsOnPage,
			int maxItemsOnPages,
			string? viewName)
			where TEntity : class
		{
			ArgumentNullException.ThrowIfNull(controller);
			ArgumentNullException.ThrowIfNull(dbSet);
			var query1 = filter == null
				? dbSet.AsQueryable()
				: dbSet.Where(filter);
			var dataModel1 = new PaginatedDataModel(
				order, page, itemsOnPage, 0, defaultItemsOnPage, maxItemsOnPages);
			var orderedQuery1 = dataModel1.GetQueryOrdered(query1);
			var paginationHelper1 = await PaginatedQueryableHelper<TEntity>.CreateAsync(
				orderedQuery1, dataModel1.Page, dataModel1.ItemsOnPage);
			var model1 = await paginationHelper1.Query.ToListAsync();
			controller.ViewData.SetPaginationData(dataModel1);
			return viewName == null
				? controller.View(model1)
				: controller.View(viewName, model1);
		}

	}

}
