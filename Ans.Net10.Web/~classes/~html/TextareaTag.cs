// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации многострочного поля ввода текста 
	/// <c>&lt;textarea&gt;&lt;/textarea&gt;</c> с парным режимом рендеринга.
	/// </summary>
	public class TextareaTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TextareaTag"/>, формируя атрибуты идентификатора, имени и внутреннего содержимого.
		/// </summary>
		/// <param name="name">Системное имя и уникальный идентификатор HTML-элемента (атрибуты <c>id</c> и <c>name</c>).</param>
		/// <param name="value">Опциональное начальное текстовое значение многострочного поля ввода.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public TextareaTag(
			string name,
			string? value)
			: base("textarea", TagRenderMode.Normal)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			Value = value;
			MergeAttribute("id", Name);
			MergeAttribute("name", Name);
			InnerHtml.AppendHtml(Value ?? string.Empty);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя и идентификатор многострочного поля ввода.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает начальное текстовое значение многострочного поля ввода.
		/// </summary>
		public string? Value { get; }

	}

}
