// rev 2026-10-01

using Ans.Net10.Common.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ans.Net10.Web.Crud
{

	/// <summary>
	/// Интерфейс асинхронного CRUD-контроллера для управления жизненным циклом главных (Master) сущностей.
	/// </summary>
	/// <typeparam name="T">Тип обрабатываемой главной сущности. Должен реализовывать <see cref="IMasterEntity"/>.</typeparam>
	public interface ICrudMasterController<T>
		: ICrudController<T>
		where T : class, IMasterEntity
	{
		/// <summary>Асинхронно формирует постраничную выборку и отображает табличный список сущностей.</summary>
		/// <param name="order">Строка определения полей и направлений многокритериальной сортировки.</param>
		/// <param name="page">Номер запрашиваемой страницы (индексация с 1).</param>
		/// <param name="itemsOnPage">Количество отображаемых элементов на одной странице.</param>
		Task<IActionResult> List(string? order, int page, int itemsOnPage);

		/// <summary>Отображает пустую форму создания новой главной сущности.</summary>
		IActionResult Add();

		/// <summary>Асинхронно обрабатывает отправку формы создания новой главной сущности.</summary>
		/// <param name="model">Заполненный экземпляр сущности, полученный из контекста связывания модели.</param>
		Task<IActionResult> AddPost(T model);
	}



	/// <summary>
	/// Абстрактный прототип класса для реализации асинхронных CRUD-контроллеров 
	/// над главными (Master) сущностями с поддержкой встроенной пагинации.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, реализующей <see cref="IMasterEntity"/>.</typeparam>
	/// <remarks>
	/// [Authorize()]
	/// [Route("/")]
	/// [ApiExplorerSettings(IgnoreApi = true)]
	/// </remarks>
	public abstract class _CrudMasterController_Proto<T>(
		ICrudMasterRepository<T> repository)
		: __CrudController_Base<T>(repository),
		ICrudMasterController<T>
		where T : class, IMasterEntity
	{

		/* readonly properties */


		/// <summary>
		/// Возвращает строго типизированный экземпляр репозитория для работы с главными сущностями.
		/// </summary>
		public new ICrudMasterRepository<T> Repository
			=> (ICrudMasterRepository<T>)base.Repository;


		/* overrides */


		/// <inheritdoc />
		public override void InitView(T model)
			=> InitView();


		/* virtuals */


		/// <summary>
		/// Виртуальный метод инициализации контекста представления (например, списков ViewBag), не привязанный к конкретной модели.
		/// </summary>
		public virtual void InitView()
		{
		}


		/// <summary>
		/// Генерирует перенаправление пользователя на пустую форму добавления записи.
		/// </summary>
		public virtual IActionResult RedirectToAdd()
		{
			return RedirectToAction("Add", null, null);
		}


		/// <summary>
		/// Генерирует перенаправление пользователя на форму редактирования указанной сущности.
		/// </summary>
		public virtual IActionResult RedirectToEdit(
			T model)
		{
			ArgumentNullException.ThrowIfNull(model);
			return RedirectToAction(
				"Edit", null, new RouteValueDictionary { { "id", model.Id } });
		}


		/// <summary>
		/// Генерирует перенаправление пользователя на карточку детальных сведений указанной сущности.
		/// </summary>
		public virtual IActionResult RedirectToDetails(
			T model)
		{
			ArgumentNullException.ThrowIfNull(model);
			return RedirectToAction(
				"Details", null, new RouteValueDictionary { { "id", model.Id } });
		}


		/* actions */


		/// <summary>
		/// Асинхронно формирует постраничную выборку главных сущностей с учетом фильтрации и сортировки, 
		/// наполняет метаданные пагинации во <see cref="Controller.ViewData"/> и возвращает табличное представление.
		/// </summary>
		/// <param name="order">Строка определения полей и направлений многокритериальной сортировки. Если пуста, используется <see cref="__CrudController_Base{T}.DefaultOrder"/>.</param>
		/// <param name="page">Номер запрашиваемой страницы (индексация начинается с 1).</param>
		/// <param name="itemsOnPage">Запрашиваемое количество элементов на одной странице.</param>
		/// <returns>Задача, результатом выполнения которой является <see cref="IActionResult"/> с Razor-представлением списка сущностей текущей страницы.</returns>
		/// <remarks>
		/// [HttpGet("")]
		/// </remarks>
		public virtual async Task<IActionResult> List(
			string? order,
			int page,
			int itemsOnPage)
		{
			var baseOrder1 = order ?? DefaultOrder ?? "Id";
			var result1 = await this.GetPaginatedListAsync(
				Repository.DbSet,
				ListFilter,
				baseOrder1,
				page,
				itemsOnPage,
				DefaultItemsOnPage,
				MaxItemsOnPage,
				CustomListViewName);
			return GetListView(result1);
		}


		/// <summary>
		/// Возвращает пустую форму создания новой главной сущности с инициализированными дефолтными значениями.
		/// </summary>
		/// <returns>Объект <see cref="IActionResult"/> с Razor-представлением формы добавления объекта.</returns>
		/// <remarks>
		/// [HttpGet("add")]
		/// </remarks>
		public virtual IActionResult Add()
		{
			var model1 = Repository.GetNew();
			return GetAddView(model1);
		}


		/// <summary>
		/// Асинхронно обрабатывает отправку формы создания новой главной сущности, выполняет валидацию, 
		/// фиксацию в репозитории и осуществляет ветвление редиректа согласно правилу <see cref="__CrudController_Base{T}.ViewAfterAdd"/>.
		/// </summary>
		/// <param name="model">Заполненный экземпляр сущности, полученный из контекста связывания модели.</param>
		/// <returns>Задача, результатом которой является редирект на целевую страницу при успехе, либо текущая форма с ошибками валидации.</returns>
		/// <remarks>
		/// [HttpPost("add")]
		/// [ActionName("Add")]
		/// [ValidateAntiForgeryToken]
		/// </remarks>
		public virtual async Task<IActionResult> AddPost(
			T model)
		{
			if (model == null)
				return NotFound();
			FixModelAfterInput(model);
			BeforeAdd(model);
			EncodeModelBeforeSave(model);
			ValidationModel(model);
			if (ModelState.IsValid)
			{
				try
				{
					Repository.Add(model);
					await Repository.DbContext.SaveChangesAsync();
					AfterAdd(model);
					PrepareRedirectToList(model);
					return ViewAfterAdd switch
					{
						CrudViewEnum.Add => RedirectToAdd(),
						CrudViewEnum.Edit => RedirectToEdit(model),
						CrudViewEnum.Details => RedirectToDetails(model),
						_ => RedirectToList(model)
					};
				}
				catch (Exception ex)
				{
					ParseException(ex);
				}
			}
			return GetAddView(model);
		}


		/* functions */


		/// <summary>
		/// Формирует базовый запрос <see cref="IQueryable{T}"/> для извлечения списочных данных с учетом глобального фильтра.
		/// </summary>
		/// <returns>Запрос <see cref="IQueryable{T}"/> без отслеживания изменений, готовый к дальнейшему секционированию.</returns>
		public virtual IQueryable<T> GetListQuery()
		{
			return Repository.GetItemsAsQueryable(ListFilter);
		}


		/* services */


		public IActionResult GetListView(
			IActionResult result)
		{
			ArgumentNullException.ThrowIfNull(result);
			InitView();
			if (result is ViewResult viewResult1
				&& viewResult1.Model is IEnumerable<T> entities1)
				PrepareForList(entities1);
			return CustomListViewName == null
				? View(result)
				: View(CustomListViewName, result);
		}

	}

}
