// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации стандартного текстового поля ввода 
	/// <c>&lt;input type="text" /&gt;</c> со встроенным самозакрывающимся режимом рендеринга.
	/// </summary>
	public class InputTextTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="InputTextTag"/>, формируя атрибуты идентификатора, имени и типа поля ввода.
		/// </summary>
		/// <param name="name">Системное имя и уникальный идентификатор HTML-элемента (атрибуты <c>id</c> и <c>name</c>).</param>
		/// <param name="value">Опциональное начальное текстовое значение поля ввода (атрибут <c>value</c>).</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public InputTextTag(
			string name,
			string? value)
			: base("input", TagRenderMode.SelfClosing)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			Value = value;
			MergeAttribute("id", Name);
			MergeAttribute("name", Name);
			MergeAttribute("type", "text");
			if (!string.IsNullOrEmpty(Value))
				MergeAttribute("value", Value);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя и идентификатор текстового поля ввода.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает начальное строковое значение текстового поля ввода.
		/// </summary>
		public string? Value { get; }

	}

}
