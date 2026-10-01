// rev 2026-10-01

using Ans.Net10.Common;
using Ans.Net10.Common.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ans.Net10.Web.Crud
{

	/// <summary>
	/// Интерфейс асинхронного CRUD-контроллера для управления жизненным циклом подчиненных (Slave) сущностей.
	/// </summary>
	/// <typeparam name="T">Тип обрабатываемой подчиненной сущности. Должен реализовывать <see cref="ISlaveEntity"/>.</typeparam>
	public interface ICrudSlaveController<T>
		: ICrudController<T>
		where T : class, ISlaveEntity
	{
		/// <summary>Асинхронно формирует постраничную выборку подчиненных сущностей для конкретного владельца.</summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="order">Строка определения полей и направлений многокритериальной сортировки.</param>
		/// <param name="page">Номер запрашиваемой страницы (индексация с 1).</param>
		/// <param name="itemsOnPage">Количество отображаемых элементов на одной странице.</param>
		Task<IActionResult> List(int masterPtr, string? order, int page, int itemsOnPage);

		/// <summary>Отображает пустую форму создания новой подчиненной сущности для указанного владельца.</summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		IActionResult Add(int masterPtr);

		/// <summary>Асинхронно обрабатывает отправку формы создания новой подчиненной сущности.</summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="model">Заполненный экземпляр сущности, полученный из контекста связывания модели.</param>
		Task<IActionResult> AddPost(int masterPtr, T model);
	}



	/// <summary>
	/// Абстрактный прототип класса для реализации асинхронных CRUD-контроллеров 
	/// над подчиненными (Slave) сущностями с жесткой привязкой к владельцу.
	/// </summary>
	/// <typeparam name="T">Тип доменной сущности, реализующей <see cref="ISlaveEntity"/>.</typeparam>
	/// <remarks>
	/// [Authorize()]
	/// [Route("/")]
	/// [ApiExplorerSettings(IgnoreApi = true)]
	/// </remarks>
	public abstract class _CrudSlaveController_Proto<T>(
		ICrudSlaveRepository<T> repository)
		: __CrudController_Base<T>(repository),
		ICrudSlaveController<T>
		where T : class, ISlaveEntity
	{

		/* readonly properties */


		/// <summary>
		/// Возвращает строго типизированный экземпляр репозитория для работы с подчиненными сущностями.
		/// </summary>
		public new ICrudSlaveRepository<T> Repository
			=> (ICrudSlaveRepository<T>)base.Repository;


		/* overrides */


		/// <inheritdoc />
		public override void InitView(
			T model)
		{
			ArgumentNullException.ThrowIfNull(model);
			InitView(model.MasterPtr);
		}


		/// <inheritdoc />
		public override void PrepareRedirectToList(
			T model)
		{
			ArgumentNullException.ThrowIfNull(model);
			ListRouteValues["masterPtr"] = model.MasterPtr;
			base.PrepareRedirectToList(model);
		}


		/* virtuals */


		/// <summary>
		/// Виртуальный метод инициализации контекста представления, привязанный к идентификатору владельца.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		public virtual void InitView(
			int masterPtr)
		{
		}


		/// <summary>
		/// Генерирует перенаправление пользователя на пустую форму добавления подчиненной записи.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		public virtual IActionResult RedirectToAdd(
			int masterPtr)
		{
			return RedirectToAction(
				"Add", null, new RouteValueDictionary { { "masterPtr", masterPtr } });
		}


		/// <summary>
		/// Генерирует перенаправление пользователя на форму редактирования указанной подчиненной сущности.
		/// </summary>
		public virtual IActionResult RedirectToEdit(
			T model)
		{
			ArgumentNullException.ThrowIfNull(model);
			return RedirectToAction(
				"Edit", null, new RouteValueDictionary { { "id", model.Id } });
		}


		/// <summary>
		/// Генерирует перенаправление пользователя на карточку подробных сведений подчиненной сущности.
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
		/// Асинхронно формирует постраничную выборку подчиненных сущностей, жестко ограниченную рамками 
		/// указанного владельца, наполняет метаданные пагинации и возвращает табличное представление.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="order">Строка определения полей и направлений многокритериальной сортировки. Если пуста, используется сортировка по умолчанию.</param>
		/// <param name="page">Номер запрашиваемой страницы (индексация начинается с 1).</param>
		/// <param name="itemsOnPage">Запрашиваемое количество элементов на одной странице.</param>
		/// <returns>Задача, результатом выполнения которой является <see cref="IActionResult"/> с Razor-представлением списка подчиненных сущностей.</returns>
		/// <remarks>
		/// [HttpGet("{masterPtr:int}")]
		/// </remarks>
		public virtual async Task<IActionResult> List(
			int masterPtr,
			string? order,
			int page,
			int itemsOnPage)
		{
			var baseOrder1 = order ?? DefaultOrder ?? "Id";
			var query1 = GetListQuery(masterPtr);
			var combinedFilter1 = ListFilter == null
				? x => x.MasterPtr == masterPtr
				: ListFilter.And(x => x.MasterPtr == masterPtr);
			var result1 = await this.GetPaginatedListAsync(
				Repository.DbSet,
				combinedFilter1,
				baseOrder1,
				page,
				itemsOnPage,
				DefaultItemsOnPage,
				MaxItemsOnPage,
				CustomListViewName);
			return GetListView(result1, masterPtr);
		}


		/// <summary>
		/// Возвращает пустую форму создания новой подчиненной сущности, жестко связанной с указанным владельцем.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <returns>Объект <see cref="IActionResult"/> с Razor-представлением формы добавления.</returns>
		/// <remarks>
		/// [HttpGet("{masterPtr:int}/add")]
		/// </remarks>
		public virtual IActionResult Add(
			int masterPtr)
		{
			var model1 = Repository.GetNew(masterPtr);
			return GetAddView(model1);
		}


		/// <summary>
		/// Асинхронно обрабатывает отправку формы создания новой подчиненной сущности, гарантируя простановку 
		/// внешнего ключа владельца, валидацию и фиксацию изменений в базе данных.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <param name="model">Заполненный экземпляр сущности, полученный из контекста связывания модели.</param>
		/// <returns>Задача, результатом которой является редирект на целевую страницу при успехе, либо текущая форма с ошибками.</returns>
		/// <remarks>
		/// [HttpPost("{masterPtr:int}/add")]
		/// [ActionName("Add")]
		/// [ValidateAntiForgeryToken]
		/// </remarks>
		public virtual async Task<IActionResult> AddPost(
			int masterPtr,
			T model)
		{
			if (model == null)
				return NotFound();
			FixModelAfterInput(model);
			BeforeAdd(model);
			EncodeModelBeforeSave(model);
			model.MasterPtr = masterPtr;
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
						CrudViewEnum.Add => RedirectToAdd(masterPtr),
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
		/// Формирует базовый запрос <see cref="IQueryable{T}"/> для извлечения подчиненных сущностей 
		/// с объединением глобального фильтра списков и фильтра по конкретному владельцу.
		/// </summary>
		/// <param name="masterPtr">Идентификатор главной (владеющей) сущности.</param>
		/// <returns>Запрос <see cref="IQueryable{T}"/> без отслеживания изменений, готовый к материализации.</returns>
		public IQueryable<T> GetListQuery(
			int masterPtr)
		{
			var filter1 = ListFilter == null
				? x => x.MasterPtr == masterPtr
				: ListFilter.And(x => x.MasterPtr == masterPtr);
			return Repository.GetItemsAsQueryable(filter1);
		}


		/* services */


		/// <summary>
		/// Выполняет инициализацию окружения и генерирует Razor-представление для табличного списка подчиненных сущностей.
		/// </summary>
		public IActionResult GetListView(
			IActionResult result,
			int masterPtr)
		{
			ArgumentNullException.ThrowIfNull(result);
			InitView(masterPtr);
			if (result is ViewResult viewResult1
				&& viewResult1.Model is IEnumerable<T> entities1)
				PrepareForList(entities1);
			return CustomListViewName == null
				? View(result)
				: View(CustomListViewName, result);
		}

	}

}
