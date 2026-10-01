// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации поля ввода флажка 
	/// <c>&lt;input type="checkbox" /&gt;</c> со встроенным самозакрывающимся режимом рендеринга.
	/// </summary>
	public class InputCheckboxTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="InputCheckboxTag"/>, формируя атрибуты имени, идентификатора, значения и состояния выбора.
		/// </summary>
		/// <param name="name">Системное имя HTML-элемента для группировки данных формы (アтрибут <c>name</c>).</param>
		/// <param name="id">Уникальный идентификатор HTML-элемента (атрибут <c>id</c>).</param>
		/// <param name="value">Передаваемое на сервер значение чекбокса при его активации (атрибут <c>value</c>).</param>
		/// <param name="isChecked">Признак начальной отмеченности флажка. Если <see langword="true"/>, добавляется атрибут <c>checked="checked"</c>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> или <paramref name="id"/> равен <see langword="null"/>.</exception>
		public InputCheckboxTag(
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
			MergeAttribute("type", "checkbox");
			MergeAttribute("data-val", Value);
			MergeAttribute("value", Value);
			if (IsChecked)
				MergeAttribute("checked", "checked");
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
		/// Возвращает признак того, отмечен ли данный флажок в текущий момент.
		/// </summary>
		public bool IsChecked { get; }

	}

}
