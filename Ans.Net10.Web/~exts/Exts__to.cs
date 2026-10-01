// rev 2026-09-28

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для трансформации и приведения базовых типов данных к веб-ориентированным объектам и HTML-сущностям.
	/// </summary>
	public static partial class Exts__to
	{

		/// <summary>
		/// Преобразует обычную строку в безопасную для рендеринга структуру <see cref="HtmlString"/>.
		/// </summary>
		/// <param name="value">Исходная строка.</param>
		/// <returns>Экземпляр <see cref="HtmlString"/>, содержащий исходную строку, либо <see cref="HtmlString.Empty"/>.</returns>
		public static HtmlString ToHtml(
			this string? value)
		{
			return string.IsNullOrEmpty(value)
				? HtmlString.Empty
				: new HtmlString(value);
		}


		/// <summary>
		/// Преобразует обычную строку в структуру <see cref="HtmlString"/> с опциональным предварительным наложением правил экранной типографики.
		/// </summary>
		/// <param name="value">Исходная строка.</param>
		/// <param name="useTypograf">Признак необходимости применения типографа с изоляцией HTML-тегов.</param>
		/// <returns>Объект <see cref="HtmlString"/> с оттипографленным контентом, либо <see cref="HtmlString.Empty"/>.</returns>
		public static HtmlString ToHtml(
			this string? value,
			bool useTypograf)
		{
			if (string.IsNullOrEmpty(value))
				return HtmlString.Empty;
			var processed1 = useTypograf
				? SuppTypograph.GetTypografMin(value)
				: value;
			return new HtmlString(processed1 ?? string.Empty);
		}


		/// <summary>
		/// Форматирует строку по указанному шаблону с опциональной типографикой и преобразует результат в объект <see cref="HtmlString"/>.
		/// </summary>
		/// <param name="value">Исходная строка.</param>
		/// <param name="template">Строковый шаблон обертки результата (например, <c>"&lt;div&gt;{0}&lt;/div&gt;"</c>).</param>
		/// <param name="useTypograf">Признак необходимости применения типографа к исходной строке.</param>
		/// <returns>Отформатированный объект <see cref="HtmlString"/> или <see cref="HtmlString.Empty"/>, если исходная строка пуста или состоит из пробелов.</returns>
		public static HtmlString ToHtml(
			this string? value,
			string? template,
			bool useTypograf = false)
		{
			if (string.IsNullOrWhiteSpace(value))
				return HtmlString.Empty;
			var processed1 = useTypograf
				? SuppTypograph.GetTypografMin(value)
				: value;
			return processed1.Make(template).ToHtml();
		}

	}

}
