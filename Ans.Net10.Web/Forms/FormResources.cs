// rev 2026-10-02

using Ans.Net10.Common;
using Ans.Net10.Common.Resources;
using Microsoft.AspNetCore.Html;
using System.Resources;

namespace Ans.Net10.Web.Forms
{

	/// <summary>
	/// Класс-хелпер для каскадного извлечения, локализации и автоматического форматирования 
	/// строковых и HTML-ресурсов экранных форм и страниц CRUD.
	/// </summary>
	/// <remarks>
	/// Наследует базовую логику каскадного поиска метаданных из <see cref="ResourcesHelper"/> 
	/// и обеспечивает интеграцию со стандартными шаблонами локализации <see cref="Form"/>.
	/// </remarks>
	/// <param name="resources">Массив пользовательских менеджеров ресурсов, имеющих приоритет при каскадном поиске.</param>
	public class FormResources(
		params ResourceManager[] resources)
		: ResourcesHelper(resources)
	{

		/* readonly properties */


		/// <summary>
		/// Возвращает множественное имя сущности (например, "Пользователи", "Статьи"), 
		/// извлеченное из мета-ключа "_TitlePluralize".
		/// </summary>
		/// <value>Строка с заголовком. Если ключ не найден, возвращается имя ключа.</value>
		public string TitlePluralize
			=> field ??= GetCrudFace("_TitlePluralize")?.Title ?? "_TitlePluralize";


		/// <summary>
		/// Возвращает множественное имя сущности в виде безопасной HTML-строки с примененными правилами типографики.
		/// </summary>
		public HtmlString TitlePluralize_Html
			=> field ??= TitlePluralize.ToHtml(true);


		/// <summary>
		/// Возвращает имя сущности в именительном падеже (Кто? Что? Например, "пользователь", "статья"), 
		/// извлеченное из мета-ключа "_TitleWhoWhat".
		/// </summary>
		/// <value>Строка с заголовком. Если ключ не найден, возвращается имя ключа.</value>
		public string TitleWhoWhat
			=> field ??= GetCrudFace("_TitleWhoWhat")?.Title ?? "_TitleWhoWhat";


		/// <summary>
		/// Возвращает имя сущности в именительном падеже в виде безопасной HTML-строки с примененными правилами типографики.
		/// </summary>
		public HtmlString TitleWhoWhat_Html
			=> field ??= TitleWhoWhat.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный и отформатированный заголовок страницы для отображения списков и таблиц записей.
		/// </summary>
		/// <value>Строка вида "Пользователи" или "Список объектов".</value>
		public string ListPageTitle
			=> string.Format(Form.Template_PageTitle_List, TitlePluralize);


		/// <summary>
		/// Возвращает локализованный и отформатированный заголовок страницы создания новой записи.
		/// </summary>
		/// <value>Строка вида "Создание пользователя".</value>
		public string AddPageTitle
			=> string.Format(Form.Template_PageTitle_Add, TitleWhoWhat);


		/// <summary>
		/// Возвращает локализованный и отформатированный заголовок страницы редактирования существующей записи.
		/// </summary>
		/// <value>Строка вида "Редактирование пользователя".</value>
		public string EditPageTitle
			=> string.Format(Form.Template_PageTitle_Edit, TitleWhoWhat);


		/// <summary>
		/// Возвращает локализованный и отформатированный заголовок страницы просмотра параметров и деталей записи.
		/// </summary>
		/// <value>Строка вида "Параметры пользователя".</value>
		public string DetailPageTitle
			=> string.Format(Form.Template_PageTitle_Details, TitleWhoWhat);


		/// <summary>
		/// Возвращает локализованный и отформатированный заголовок страницы подтверждения удаления записи.
		/// </summary>
		/// <value>Строка вида "Удаление пользователя".</value>
		public string DeletePageTitle
			=> string.Format(Form.Template_PageTitle_Delete, TitleWhoWhat);


		/// <summary>
		/// Возвращает локализованный text сообщения об отсутствии элементов в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Text_EmptyItems_Html
			=> field ??= Form.Text_EmptyItems.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text кнопки отмены операции в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Text_Cancel_Html
			=> field ??= Form.Text_Cancel.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text действия "Создать" в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Text_Add_Html
			=> field ??= Form.Text_Add.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text действия "Сохранить" в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Text_Save_Html
			=> field ??= Form.Text_Save.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text кнопки подтверждения создания записи в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Text_SubmitAdd_Html
			=> field ??= Form.Text_SubmitAdd.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text кнопки создания и продолжения редактирования записи в виде безопасной HTML-строки.
		/// </summary>
		public HtmlString Text_SubmitAddAndEdit_Html
			=> field ??= Form.Text_SubmitAddAndEdit.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text кнопки отправки формы сохранения изменений в виде безопасной HTML-строки.
		/// </summary>
		public HtmlString Text_SubmitSave_Html
			=> field ??= Form.Text_SubmitSave.ToHtml(true);


		/// <summary>
		/// Возвращает локализованный text кнопки подтверждения безвозвратного удаления записи в виде безопасной HTML-строки.
		/// </summary>
		public HtmlString Text_SubmitDelete_Html
			=> field ??= Form.Text_SubmitDelete.ToHtml(true);


		/// <summary>
		/// Возвращает локализованную ссылку-заголовок "редактировать" в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Title_Edit_Html
			=> field ??= Form.Title_Edit.ToHtml(true);


		/// <summary>
		/// Возвращает локализованную ссылку-заголовок "просмотр" в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Title_Detail_Html
			=> field ??= Form.Title_Detail.ToHtml(true);


		/// <summary>
		/// Возвращает локализованную ссылку-заголовок "удалить" в виде безопасной HTML-строки с типографикой.
		/// </summary>
		public HtmlString Title_Delete_Html
			=> field ??= Form.Title_Delete.ToHtml(true);

	}

}
