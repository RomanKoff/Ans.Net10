// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации скрытого поля ввода 
	/// <c>&lt;input type="hidden" /&gt;</c> со встроенным самозакрывающимся режимом рендеринга.
	/// </summary>
	public class InputHiddenTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="InputHiddenTag"/>, формируя атрибуты имени и типа скрытого поля ввода.
		/// </summary>
		/// <param name="name">Системное имя HTML-элемента (атрибут <c>name</c>).</param>
		/// <param name="value">Опциональное начальное строковое значение скрытого поля ввода (атрибут <c>value</c>).</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public InputHiddenTag(
			string name,
			string? value)
			: base("input", TagRenderMode.SelfClosing)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			Value = value;
			MergeAttribute("name", Name);
			MergeAttribute("type", "hidden");
			if (!string.IsNullOrEmpty(Value))
				MergeAttribute("value", Value);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя скрытого поля ввода.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает начальное строковое значение скрытого поля ввода.
		/// </summary>
		public string? Value { get; }

	}

}
