// rev 2026-10-01

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Высокоуровневый HTML-компонент, генерирующий законченный Bootstrap-блок радиокнопки переключателя (<c>&lt;div class="form-check"&gt;</c>).
	/// </summary>
	/// <remarks>
	/// Автоматически объединяет в себе элемент управления <see cref="InputRadioTag"/> 
	/// и связанную подпись <c>&lt;label&gt;</c> с поддержкой инлайнового размещения и автогенерацией уникальных ID.
	/// </remarks>
	public class RadioHtml
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RadioHtml"/>, собирая контейнер, радиокнопку и текстовую метку.
		/// </summary>
		/// <param name="name">Системное имя группы радиокнопок для обеспечения взаимоисключающего выбора (атрибут <c>name</c>).</param>
		/// <param name="value">Передаваемое на сервер значение радиокнопки при её активации (атрибут <c>value</c>).</param>
		/// <param name="title">Отображаемый текст подписи. Если равен <see langword="null"/>, используется значение из параметра <paramref name="value"/>.</param>
		/// <param name="key">Уникальный суффикс ключа элемента для генерации составного ID. Если равен <see langword="null"/>, используется значение из параметра <paramref name="value"/>.</param>
		/// <param name="isInline">Признак инлайнового (горизонтального) размещения элемента в ряд. Добавляет класс <c>form-check-inline</c>.</param>
		/// <param name="isChecked">Признак начальной выбранности радиокнопки.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public RadioHtml(
			string name,
			string? value,
			string? title,
			string? key,
			bool isInline,
			bool isChecked)
			: base("div", TagRenderMode.Normal)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			Value = value ?? string.Empty;
			Title = title ?? Value;
			Key = key ?? Value;
			Id = $"{Name}_{Key}";
			IsInline = isInline;
			IsChecked = isChecked;
			AddCssClass("form-check");
			if (IsInline)
				AddCssClass("form-check-inline");
			var ctrl1 = new InputRadioTag(Name, Id, Value, IsChecked);
			var label1 = new TagBuilderExt("label", TagRenderMode.Normal);
			label1.AddCssClass("form-check-label");
			label1.MergeAttribute("for", Id);
			label1.InnerHtml.AppendHtml(Title);
			InnerHtml.AppendHtmlLine(ctrl1.ToString());
			InnerHtml.AppendHtmlLine(label1.ToString());
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя группы радиокнопок.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает строковое значение, передаваемое при активном переключателе.
		/// </summary>
		public string Value { get; }


		/// <summary>
		/// Возвращает отображаемый текст подписи переключателя.
		/// </summary>
		public string Title { get; }


		/// <summary>
		/// Возвращает суффикс ключа элемента, используемый в генерации ID.
		/// </summary>
		public string Key { get; }


		/// <summary>
		/// Возвращает автоматически сгенерированный уникальный идентификатор HTML-элемента флажка.
		/// </summary>
		public string Id { get; }


		/// <summary>
		/// Возвращает признак инлайнового (горизонтального) размещения элемента.
		/// </summary>
		public bool IsInline { get; }


		/// <summary>
		/// Возвращает признак того, выбран ли переключатель.
		/// </summary>
		public bool IsChecked { get; }

	}

}
