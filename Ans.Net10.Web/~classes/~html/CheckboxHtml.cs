// rev 2026-10-01

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Высокоуровневый HTML-компонент, генерирующий законченный Bootstrap-блок флажка ввода (<c>&lt;div class="form-check"&gt;</c>).
	/// </summary>
	/// <remarks>
	/// Автоматически объединяет в себе элемент управления <see cref="InputCheckboxTag"/> 
	/// и связанную подпись <c>&lt;label&gt;</c> с поддержкой инлайнового размещения.
	/// </remarks>
	public class CheckboxHtml
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="CheckboxHtml"/>, собирая контейнер, чекбокс и текстовую метку.
		/// </summary>
		/// <param name="name">Системное имя HTML-элемента для группировки данных формы (атрибут <c>name</c>).</param>
		/// <param name="id">Уникальный идентификатор HTML-элемента (атрибут <c>id</c>), связывающий инпут и подпись.</param>
		/// <param name="value">Передаваемое на сервер значение чекбокса при его активации (атрибут <c>value</c>).</param>
		/// <param name="title">Отображаемый текст подписи. Если равен <see langword="null"/>, используется значение из параметра <paramref name="value"/>.</param>
		/// <param name="isInline">Признак инлайнового (горизонтального) размещения элемента в ряд. Добавляет класс <c>form-check-inline</c>.</param>
		/// <param name="isChecked">Признак начальной отмеченности флажка.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> или <paramref name="id"/> равен <see langword="null"/>.</exception>
		public CheckboxHtml(
			string name,
			string id,
			string? value,
			string? title,
			bool isInline,
			bool isChecked)
			: base("div", TagRenderMode.Normal)
		{
			ArgumentNullException.ThrowIfNull(name);
			ArgumentNullException.ThrowIfNull(id);
			Name = name;
			Id = id;
			Value = value ?? string.Empty;
			Title = title ?? Value;
			IsInline = isInline;
			IsChecked = isChecked;
			AddCssClass("form-check");
			if (IsInline)
				AddCssClass("form-check-inline");
			var ctrl1 = new InputCheckboxTag(Name, Id, Value, IsChecked);
			var label1 = new TagBuilderExt("label", TagRenderMode.Normal);
			label1.AddCssClass("form-check-label");
			label1.MergeAttribute("for", Id);
			label1.InnerHtml.AppendHtml(Title);
			InnerHtml.AppendHtmlLine(ctrl1.ToString());
			InnerHtml.AppendHtmlLine(label1.ToString());
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя поля ввода флажка.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает уникальный идентификатор HTML-элемента флажка.
		/// </summary>
		public string Id { get; }


		/// <summary>
		/// Возвращает строковое значение, передаваемое при отмеченном флажке.
		/// </summary>
		public string Value { get; }


		/// <summary>
		/// Возвращает отображаемый текст подписи флажка.
		/// </summary>
		public string Title { get; }


		/// <summary>
		/// Возвращает признак инлайнового (горизонтального) размещения элемента.
		/// </summary>
		public bool IsInline { get; }


		/// <summary>
		/// Возвращает признак того, отмечен ли данный флажок.
		/// </summary>
		public bool IsChecked { get; }

	}

}
