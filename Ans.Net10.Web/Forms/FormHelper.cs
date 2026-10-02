// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Resources;

namespace Ans.Net10.Web.Forms
{

	/// <summary>
	/// Интерфейс базового визуального компонента ячейки формы.
	/// </summary>
	public interface IFormCellControl
	{
		/// <summary>
		/// Возвращает строковое представление HTML-разметки элемента управления.
		/// </summary>
		/// <returns>Строка валидного HTML-кода.</returns>
		string ToString();
	}



	/// <summary>
	/// Интерфейс именованного поля ввода формы, привязанного к свойству модели.
	/// </summary>
	public interface IFormFieldControl
		: IFormCellControl
	{
		/// <summary>
		/// Системное программное имя поля формы, соответствующее свойству DTO или доменной сущности.
		/// </summary>
		string Name { get; }
	}



	/// <summary>
	/// Интерфейс компонента формы, предназначенного исключительно для отображения данных (Read-Only).
	/// </summary>
	public interface IFormViewControl
		: IFormFieldControl
	{
	}



	/// <summary>
	/// Интерфейс интерактивного компонента формы, предназначенного для редактирования данных (ввода).
	/// </summary>
	public interface IFormEditControl
		: IFormFieldControl
	{
	}



	/// <summary>
	/// Вспомогательный класс для централизованного построения, стилизации 
	/// и автоматического рендеринга полей, ячеек и метаданных валидации на формах CRUD.
	/// </summary>
	public class FormHelper
	{

		private readonly Dictionary<string, CrudFace> _faces = [];


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="FormHelper"/> с привязкой к текущему контексту представления.
		/// </summary>
		/// <param name="helper">Экземпляр системного помощника <see cref="IHtmlHelper"/> текущего Razor-представления.</param>
		/// <param name="resources">Массив пользовательских менеджеров ресурсов локализации.</param>
		public FormHelper(
			IHtmlHelper helper,
			params ResourceManager[] resources)
		{
			Helper = helper;
			Res = new FormResources(resources);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системный помощник <see cref="IHtmlHelper"/> текущего представления.
		/// </summary>
		public IHtmlHelper Helper { get; }


		/// <summary>
		/// Возвращает хелпер каскадных строковых и HTML-ресурсов формы.
		/// </summary>
		public FormResources Res { get; }


		/* functions */


		/// <summary>
		/// Возвращает или кэширует метаданные визуального отображения поля ввода по его системному имени.
		/// </summary>
		/// <param name="name">Системное имя поля для поиска в ресурсах.</param>
		/// <returns>Заполненный объект описания метаданных <see cref="CrudFace"/>.</returns>
		public CrudFace Face(
			string name)
		{
			if (_faces.TryGetValue(name, out var face1))
				return face1;
			var face2 = Res.GetCrudFace(name) ?? new CrudFace(name, string.Empty);
			_faces.Add(name, face2);
			return face2;
		}


		/// <summary>
		/// Генерирует HTML-разметку ячейки таблицы <c>&lt;td&gt;</c> на основе текстового контента.
		/// </summary>
		/// <param name="control">Текстовое или HTML содержимое ячейки.</param>
		/// <param name="cssClasses">Опциональная строка CSS-классов ячейки.</param>
		/// <param name="styles">Опциональная строка инлайновых CSS-стилей.</param>
		/// <param name="attributes">Опциональная строка произвольных HTML-атрибутов.</param>
		/// <returns>Безопасная HTML-строка ячейки таблицы.</returns>
		public static HtmlString AddCell(
			string control,
			string? cssClasses = null,
			string? styles = null,
			string? attributes = null)
		{
			var tag1 = new TagBuilderExt("td", TagRenderMode.Normal);
			tag1.Apply(cssClasses, styles, attributes);
			tag1.InnerHtml.AppendHtml(control);
			return tag1.ToHtml();
		}


		/// <summary>
		/// Генерирует HTML-разметку ячейки таблицы <c>&lt;td&gt;</c> на основе компонента ячейки формы.
		/// </summary>
		/// <param name="control">Компонент ячейки формы, реализующий <see cref="IFormCellControl"/>.</param>
		/// <param name="cssClasses">Опциональная строка CSS-классов ячейки.</param>
		/// <param name="styles">Опциональная строка инлайновых CSS-стилей.</param>
		/// <param name="attributes">Опциональная строка произвольных HTML-атрибутов.</param>
		/// <returns>Безопасная HTML-строка ячейки таблицы.</returns>
		public static HtmlString AddCell(
			IFormCellControl control,
			string? cssClasses = null,
			string? styles = null,
			string? attributes = null)
		{
			return AddCell(
				control.ToString(), cssClasses, styles, attributes);
		}


		/// <summary>
		/// Генерирует и возвращает разметку поля формы в режиме отображения данных (Read-Only) со связанной подписью.
		/// </summary>
		/// <param name="control">Компонент отображения данных, реализующий <see cref="IFormViewControl"/>.</param>
		/// <param name="hideDetails">Флаг принудительного скрытия описания (Description) и примера заполнения (Sample) поля.</param>
		/// <returns>Безопасная HTML-строка контейнера поля формы.</returns>
		public HtmlString AddView(
			IFormViewControl control,
			bool hideDetails = false)
		{
			return _addField(
				control, false, null, hideDetails);
		}


		/// <summary>
		/// Генерирует и возвращает разметку интерактивного поля ввода формы со связанной подписью и блоками ошибок валидации.
		/// </summary>
		/// <param name="control">Компонент ввода данных, реализующий <see cref="IFormEditControl"/>.</param>
		/// <param name="isRequired">Признак обязательности заполнения поля ввода.</param>
		/// <param name="hideDetails">Флаг принудительного скрытия описания (Description) и примера заполнения (Sample) поля.</param>
		/// <returns>Безопасная HTML-строка контейнера поля формы.</returns>
		public HtmlString AddEdit(
			IFormEditControl control,
			bool isRequired = false,
			bool hideDetails = false)
		{
			var errors1 = Helper.ViewContext.ModelState.GetFieldErrors(control.Name);
			return _addField(
				control, isRequired, errors1, hideDetails);
		}


		/* privates */


		private HtmlString _addField(
			IFormFieldControl control,
			bool isRequired,
			string[]? errors,
			bool hideDetails)
		{
			var tag1 = new TagBuilderExt("div", TagRenderMode.Normal);
			tag1.AddCssClass("form-field");
			var face1 = Face(control.Name);
			var label1 = new LabelFieldTag(control.Name, isRequired, face1, errors);
			tag1.InnerHtml.AppendHtml(label1.ToString());
			tag1.InnerHtml.AppendHtml(Environment.NewLine);
			if (!hideDetails)
			{
				if (face1.HasDescription)
				{
					tag1.InnerHtml.AppendHtml(
						$"<div id=\"{control.Name}_desc\" class=\"form-text\">{SuppTypograph.GetTypografMin(face1.Description)}</div>");
					tag1.InnerHtml.AppendHtml(
						Environment.NewLine);
				}
				if (face1.HasSample)
				{
					tag1.InnerHtml.AppendHtml(
						$"<div id=\"{control.Name}_sample\" class=\"form-text\">Пример: <code>{face1.Sample}</code></div>");
					tag1.InnerHtml.AppendHtml(
						Environment.NewLine);
				}
			}
			tag1.InnerHtml.AppendHtml(control.ToString());
			return tag1.ToHtml();
		}

	}

}
