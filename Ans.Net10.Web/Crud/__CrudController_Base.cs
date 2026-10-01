// rev 2026-10-01

using Ans.Net10.Common;
using Ans.Net10.Common.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq.Expressions;

namespace Ans.Net10.Web.Crud
{

	/// <summary>
	/// Перечисление стандартных Razor-представлений (страниц), используемых в рамках CRUD-сценариев.
	/// </summary>
	public enum CrudViewEnum
	{
		/// <summary>
		/// Представление таблицы со списком записей.
		/// </summary>
		List,

		/// <summary>
		/// Представление формы создания новой записи.
		/// </summary>
		Add,

		/// <summary>
		/// Представление формы редактирования существующей записи.
		/// </summary>
		Edit,

		/// <summary>
		/// Представление подробных сведений об объекте.
		/// </summary>
		Details,

		/// <summary>
		/// Представление формы удаления существующей записи.
		/// </summary>
		Delete
	}



	/// <summary>
	/// Интерфейс асинхронного CRUD-контроллера для управления жизненным циклом доменной сущности.
	/// </summary>
	/// <typeparam name="T">Тип обрабатываемой сущности базы данных. Должен быть ссылочным типом.</typeparam>
	public interface ICrudController<T>
		where T : class
	{
		/// <summary>
		/// Асинхронно отображает карточку деталей объекта.
		/// </summary>
		Task<IActionResult> Details(int id);

		/// <summary>
		/// Асинхронно отображает форму редактирования объекта.
		/// </summary>
		Task<IActionResult> Edit(int id);

		/// <summary>
		/// Асинхронно обрабатывает отправку формы изменения объекта.
		/// </summary>
		Task<IActionResult> EditPost(int id, T model);

		/// <summary>
		/// Асинхронно отображает форму подтверждения удаления объекта.
		/// </summary>
		Task<IActionResult> Delete(int id);

		/// <summary>
		/// Асинхронно обрабатывает подтверждение удаления объекта.
		/// </summary>
		Task<IActionResult> DeletePost(int id);
	}



	/// <summary>
	/// Абстрактный базовый MVC-контроллер, реализующий унифицированную асинхронную логику CRUD-операций.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности. Должен быть ссылочным типом.</typeparam>
	/// <remarks>
	/// [Authorize()]
	/// [Route("/")]
	/// [ApiExplorerSettings(IgnoreApi = true)]
	/// </remarks>
	public abstract class __CrudController_Base<T>
		: Controller, ICrudController<T>
		where T : class
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="__CrudController_Base{T}"/> с внедрением зависимости репозитория.
		/// </summary>
		/// <param name="repository">Экземпляр репозитория данных для выполнения низкоуровневых операций с БД.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="repository"/> равен <see langword="null"/>.</exception>
		protected __CrudController_Base(
			ICrudRepository<T> repository)
		{
			ArgumentNullException.ThrowIfNull(repository);
			Repository = repository;
			InitController();
		}


		/* abstracts */


		/// <summary>
		/// Абстрактный метод инициализации вспомогательных данных представления (например, списков ViewBag) перед рендерингом формы.
		/// </summary>
		public abstract void InitView(
			T model);


		/* virtuals */


		/// <summary>
		/// Виртуальный метод для первичной настройки контроллера на этапе выполнения конструктора.
		/// </summary>
		public virtual void InitController()
		{
		}


		/// <summary>
		/// Проверяет, разрешен ли просмотр детальной информации для указанной сущности.
		/// </summary>
		public virtual bool AllowDetails(
			T model)
			=> true;


		/// <summary>
		/// Базовый признак возможности изменения или удаления указанной сущности.
		/// </summary>
		public virtual bool AllowChange(
			T model)
			=> true;


		/// <summary>
		/// Проверяет, разрешено ли редактирование указанной сущности.
		/// </summary>
		public virtual bool AllowEdit(
			T model)
			=> AllowChange(model);


		/// <summary>
		/// Проверяет, разрешено ли удаление указанной сущности.
		/// </summary>
		public virtual bool AllowDelete(
			T model)
			=> AllowChange(model);


		/// <summary>
		/// Базовый метод подготовки модели перед выводом в любое представление.
		/// </summary>
		public virtual void Prepare(
			T model)
		{
		}


		/// <summary>
		/// Подготовка модели перед открытием формы добавления.
		/// </summary>
		public virtual void PrepareForAdd(
			T model)
			=> Prepare(model);


		/// <summary>
		/// Подготовка модели перед открытием карточки деталей.
		/// </summary>
		public virtual void PrepareForDetails(
			T model)
			=> Prepare(model);


		/// <summary>
		/// Подготовка модели перед открытием формы редактирования.
		/// </summary>
		public virtual void PrepareForEdit(
			T model)
			=> PrepareForDetails(model);


		/// <summary>
		/// Подготовка модели перед открытием формы удаления.
		/// </summary>
		public virtual void PrepareForDelete(
			T model)
			=> PrepareForDetails(model);


		/// <summary>
		/// Пакетная подготовка и декодирование коллекции моделей перед выводом в списочное представление.
		/// </summary>
		public virtual void PrepareForList(
			IEnumerable<T> model)
		{
			foreach (var item1 in model)
				DecodeModelBeforeView(item1);
		}


		/// <summary>
		/// Корректировка и нормализация полей модели сразу после обработки входящего пользовательского ввода.
		/// </summary>
		public virtual void FixModelAfterInput(
			T model)
		{
		}


		/// <summary>
		/// Шифрование или трансформация данных модели непосредственно перед сохранением в базу данных.
		/// </summary>
		public virtual void EncodeModelBeforeSave(
			T model)
		{
		}


		/// <summary>
		/// Декодирование специфических полей модели перед передачей в форму редактирования.
		/// </summary>
		public virtual void DecodeModelBeforeEdit(
			T model)
		{
		}


		/// <summary>
		/// Декодирование специфических полей модели перед передачей в read-only представления (детали, удаление).
		/// </summary>
		public virtual void DecodeModelBeforeView(
			T model)
		{
		}


		/// <summary>
		/// Метод для выполнения кастомной сложной бизнес-валидации модели на уровне контроллера.
		/// </summary>
		public virtual void ValidationModel(
			T model)
		{
		}


		/// <summary>
		/// Общий хук жизненного цикла, выполняемый перед любым изменением данных (добавление/обновление).
		/// </summary>
		public virtual void BeforeChange(
			T model)
		{
		}


		/// <summary>
		/// Хук жизненного цикла, выполняемый перед сохранением новой записи.
		/// </summary>
		public virtual void BeforeAdd(
			T model)
			=> BeforeChange(model);


		/// <summary>
		/// Хук жизненного цикла, выполняемый перед сохранением изменений существующей записи.
		/// </summary>
		public virtual void BeforeUpdate(
			T model)
			=> BeforeChange(model);


		/// <summary>
		/// Хук жизненного цикла, выполняемый перед удалением записи из БД.
		/// </summary>
		public virtual void BeforeDelete(
			T model)
		{
		}


		/// <summary>
		/// Общий хук жизненного цикла, выполняемый после успешного изменения данных (добавление/обновление).
		/// </summary>
		public virtual void AfterChange(
			T model)
		{
		}


		/// <summary>
		/// Хук жизненного цикла, выполняемый после успешного добавления записи.
		/// </summary>
		public virtual void AfterAdd(
			T model)
			=> AfterChange(model);


		/// <summary>
		/// Хук жизненного цикла, выполняемый после успешного обновления записи.
		/// </summary>
		public virtual void AfterUpdate(
			T model)
			=> AfterChange(model);


		/// <summary>
		/// Хук жизненного цикла, выполняемый после успешного удаления записи.
		/// </summary>
		public virtual void AfterDelete()
		{
		}


		/// <summary>
		/// Позволяет наполнить <see cref="ListRouteValues"/> перед выполнением редиректа на таблицу.
		/// </summary>
		public virtual void PrepareRedirectToList(
			T model)
		{
		}


		/// <summary>
		/// Выполняет перенаправление пользователя на списочное (табличное) действие контроллера.
		/// </summary>
		public virtual IActionResult RedirectToList(
			T model)
		{
			return RedirectToAction("List", null, ListRouteValues);
		}


		/// <summary>
		/// Выполняет безопасный разбор исключений СУБД, регистрируя понятные ошибки в <see cref="ControllerBase.ModelState"/>.
		/// </summary>
		public virtual void ParseException(
			Exception exception)
		{
			ArgumentNullException.ThrowIfNull(exception);
			if (exception.TestDuplicateKeyPSQL(ModelState))
				return;
			if (exception.TestDeleteReferencePSQL(ModelState))
				return;
			if (exception.TestCheckConstraintPSQL(ModelState, string.Empty))
				return;
			if (exception.TestDataFormatErrorPSQL(ModelState))
				return;
			ModelState.AddModelError(string.Empty, exception.GetExceptionMessage());
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает экземпляр CRUD-репозитория.
		/// </summary>
		public ICrudRepository<T> Repository { get; }


		/* properties */


		/// <summary>
		/// Направление сортировки по умолчанию для списков.
		/// </summary>
		public string? DefaultOrder { get; set; }


		/// <summary>
		/// Количество записей на странице списка по умолчанию. По умолчанию: 50.
		/// </summary>
		public int DefaultItemsOnPage { get; set; } = 50;


		/// <summary>
		/// Максимально допустимый лимит записей на странице. По умолчанию: 600.
		/// </summary>
		public int MaxItemsOnPage { get; set; } = 600;


		/// <summary>
		/// Определяет целевую страницу перехода после успешного добавления сущности
		/// .</summary>
		public CrudViewEnum ViewAfterAdd { get; set; } = CrudViewEnum.List;


		/// <summary>
		/// Альтернативное имя Razor-представления для табличного списка.
		/// </summary>
		public string? CustomListViewName { get; set; }


		/// <summary>
		/// Альтернативное имя Razor-представления для формы добавления.
		/// </summary>
		public string? CustomAddViewName { get; set; }


		/// <summary>
		/// Альтернативное имя Razor-представления для карточки деталей.
		/// </summary>
		public string? CustomDetailsViewName { get; set; }


		/// <summary>
		/// Альтернативное имя Razor-представления для формы редактирования.
		/// </summary>
		public string? CustomEditViewName { get; set; }


		/// <summary>
		/// Альтернативное имя Razor-представления для страницы удаления.
		/// </summary>
		public string? CustomDeleteViewName { get; set; }


		/// <summary>
		/// Словарь параметров маршрутизации, применяемый при автоматических редиректах на списки.
		/// </summary>
		public RouteValueDictionary ListRouteValues { get; set; } = [];


		/// <summary>
		/// Выражение фильтрации, применяемое по умолчанию при выборке списков сущностей.
		/// </summary>
		public Expression<Func<T, bool>>? ListFilter { get; set; }


		/* actions */


		/// <summary>
		/// Асинхронно извлекает сущность по идентификатору, выполняет её декодирование и отображает карточку деталей.
		/// </summary>
		/// <param name="id">Уникальный числовой идентификатор сущности.</param>
		/// <returns>Объект <see cref="IActionResult"/> с Razor-представлением деталей или <see cref="NotFoundResult"/>.</returns>
		/// <remarks>
		/// [HttpGet("details/{id:int}")]
		/// </remarks>
		public virtual async Task<IActionResult> Details(
			int id)
		{
			var model1 = await Repository.GetItemAsync(id);
			if (model1 == null)
				return NotFound();
			DecodeModelBeforeView(model1);
			return GetDetailView(model1);
		}


		/// <summary>
		/// Асинхронно извлекает сущность по идентификатору, выполняет декодирование и отображает форму редактирования.
		/// </summary>
		/// <param name="id">Уникальный числовой идентификатор сущности.</param>
		/// <returns>Объект <see cref="IActionResult"/> с Razor-представлением формы редактирования или <see cref="NotFoundResult"/>.</returns>
		/// <remarks>
		/// [HttpGet("edit/{id:int}")]
		/// </remarks>
		public virtual async Task<IActionResult> Edit(
			int id)
		{
			var model1 = await Repository.GetItemAsync(id);
			if (model1 == null)
				return NotFound();
			DecodeModelBeforeEdit(model1);
			return GetEditView(model1);
		}


		/// <summary>
		/// Асинхронно обрабатывает отправку формы изменения объекта, выполняя валидацию и фиксацию данных в БД.
		/// </summary>
		/// <param name="id">Уникальный числовой идентификатор изменяемой сущности.</param>
		/// <param name="model">Заполненный экземпляр сущности, полученный из контекста связывания модели.</param>
		/// <returns>Редирект на список при успешном обновлении, либо текущая форма редактирования с отображением ошибок валидации.</returns>
		/// <remarks>
		/// [HttpPost("edit/{id:int}")]
		/// [ActionName("Edit")]
		/// [ValidateAntiForgeryToken]
		/// </remarks>
		public virtual async Task<IActionResult> EditPost(
			int id,
			T model)
		{
			if (model == null)
				return NotFound();
			if (!AllowEdit(model))
				return Forbid();
			FixModelAfterInput(model);
			BeforeUpdate(model);
			EncodeModelBeforeSave(model);
			ValidationModel(model);
			if (ModelState.IsValid)
			{
				try
				{
					Repository.UpdateEvery(model);
					await Repository.DbContext.SaveChangesAsync();
					AfterUpdate(model);
					PrepareRedirectToList(model);
					return RedirectToList(model);
				}
				catch (Exception ex)
				{
					ParseException(ex);
				}
			}
			return GetEditView(model);
		}


		/// <summary>
		/// Асинхронно извлекает сущность по идентификатору, выполняет декодирование и отображает страницу подтверждения удаления.
		/// </summary>
		/// <param name="id">Уникальный числовой идентификатор удаляемой сущности.</param>
		/// <returns>Объект <see cref="IActionResult"/> с Razor-представлением удаления или <see cref="NotFoundResult"/>.</returns>
		/// <remarks>
		/// [HttpGet("delete/{id:int}")]
		/// </remarks>
		public virtual async Task<IActionResult> Delete(
			int id)
		{
			var model1 = await Repository.GetItemAsync(id);
			if (model1 == null)
				return NotFound();
			DecodeModelBeforeView(model1);
			return GetDeleteView(model1);
		}


		/// <summary>
		/// Асинхронно обрабатывает подтверждение удаления сущности из базы данных по её идентификатору.
		/// </summary>
		/// <param name="id">Уникальный числовой идентификатор удаляемой сущности.</param>
		/// <returns>
		/// Задача, результатом которой является <see cref="IActionResult"/> с перенаправлением на список при успехе,
		/// либо текущая страница подтверждения удаления с отображением перехваченных ошибок СУБД.
		/// </returns>
		/// <remarks>
		/// [HttpPost("delete/{id:int}")]
		/// [ActionName("Delete")]
		/// [ValidateAntiForgeryToken]
		/// Метод сначала асинхронно извлекает сущность для проверки прав доступа и выполнения хуков жизненного цикла,
		/// после чего производит неблокирующее удаление и фиксацию изменений в репозитории.
		/// </remarks>
		public virtual async Task<IActionResult> DeletePost(
			int id)
		{
			var model1 = await Repository.GetItemAsync(id);
			if (model1 == null)
				return NotFound();
			if (!AllowDelete(model1))
				return Forbid();
			BeforeDelete(model1);
			try
			{
				await Repository.RemoveAsync(id);
				await Repository.DbContext.SaveChangesAsync();
				AfterDelete();
				PrepareRedirectToList(model1);
				return RedirectToList(model1);
			}
			catch (Exception ex)
			{
				ParseException(ex);
			}
			return GetDeleteView(model1);
		}


		/* services */


		/// <summary>
		/// Выполняет инициализацию окружения и генерирует Razor-представление для формы создания новой сущности.
		/// </summary>
		public IActionResult GetAddView(
			T model)
		{
			if (model == null)
				return NotFound();
			InitView(model);
			PrepareForAdd(model);
			return CustomAddViewName == null
				? View(model)
				: View(CustomAddViewName, model);
		}


		/// <summary>
		/// Выполняет проверку прав доступа, инициализацию окружения и генерирует Razor-представление деталей сущности.
		/// </summary>
		public IActionResult GetDetailView(
			T model)
		{
			if (model == null)
				return NotFound();
			if (!AllowDetails(model))
				return Forbid();
			InitView(model);
			PrepareForDetails(model);
			return CustomDetailsViewName == null
				? View(model)
				: View(CustomDetailsViewName, model);
		}


		/// <summary>
		/// Выполняет проверку прав доступа, инициализацию окружения и генерирует Razor-представление формы редактирования сущности.
		/// </summary>
		public IActionResult GetEditView(
			T model)
		{
			if (model == null)
				return NotFound();
			if (!AllowEdit(model))
				return Forbid();
			InitView(model);
			PrepareForEdit(model);
			return CustomEditViewName == null
				? View(model)
				: View(CustomEditViewName, model);
		}


		/// <summary>
		/// Выполняет проверку прав доступа, инициализацию окружения и генерирует Razor-представление страницы подтверждения удаления.
		/// </summary>
		public IActionResult GetDeleteView(
			T model)
		{
			if (model == null)
				return NotFound();
			if (!AllowDelete(model))
				return Forbid();
			InitView(model);
			PrepareForDelete(model);
			return CustomDeleteViewName == null
				? View(model)
				: View(CustomDeleteViewName, model);
		}

	}

}
