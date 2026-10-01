// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации поля ввода радиокнопки (переключателя) 
	/// <c>&lt;input type="radio" /&gt;</c> со встроенным самозакрывающимся режимом рендеринга.
	/// </summary>
	public class InputRadioTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="InputRadioTag"/>, формируя атрибуты имени группы, идентификатора, значения и состояния выбора.
		/// </summary>
		/// <param name="name">Системное имя группы радиокнопок для обеспечения взаимоисключающего выбора (атрибут <c>name</c>).</param>
		/// <param name="id">Уникальный идентификатор HTML-элемента переключателя (атрибут <c>id</c>).</param>
		/// <param name="value">Передаваемое на сервер значение радиокнопки при её активации (атрибут <c>value</c>).</param>
		/// <param name="isChecked">Признак начальной выбранности радиокнопки. Если <see langword="true"/>, добавляется атрибут <c>checked="checked"</c>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> или <paramref name="id"/> равен <see langword="null"/>.</exception>
		public InputRadioTag(
			string name,
			string id,
			string? value,
			bool isChecked)
			: base("input", TagRenderMode.SelfClosing)
		{
			ArgumentNullException.ThrowIfNull(name);
			ArgumentNullException.ThrowIfNull(id);
			Name = name;
			Id = id;
			Value = value ?? string.Empty;
			IsChecked = isChecked;
			AddCssClass("form-check-input");
			MergeAttribute("name", Name);
			MergeAttribute("id", Id);
			MergeAttribute("type", "radio");
			MergeAttribute("value", Value);
			if (IsChecked)
				MergeAttribute("checked", "checked");
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя группы переключателей.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает уникальный идентификатор HTML-элемента радиокнопки.
		/// </summary>
		public string Id { get; }


		/// <summary>
		/// Возвращает строковое значение, передаваемое при активном переключателе.
		/// </summary>
		public string Value { get; }


		/// <summary>
		/// Возвращает признак того, выбрана ли данная радиокнопка в текущий момент.
		/// </summary>
		public bool IsChecked { get; }

	}

}
