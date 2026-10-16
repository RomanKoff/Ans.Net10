// rev 2026-10-16

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
			return new HtmlString(processed1);
		}


		/// <summary>
		/// Форматирует строку по указанному шаблону и преобразует результат в структуру <see cref="HtmlString"/> 
		/// с опциональным применением экранной типографики.
		/// </summary>
		/// <param name="value">Исходное строковое значение.</param>
		/// <param name="template">Шаблон форматирования (например, "Значение: {0}").</param>
		/// <param name="useTypograf">Признак необходимости применения типографа к результирующей строке.</param>
		/// <returns>Объект <see cref="HtmlString"/> с отформатированным контентом, либо <see cref="HtmlString.Empty"/>.</returns>
		public static HtmlString ToHtml(
			this string? value,
			string? template,
			bool useTypograf = false)
		{
			if (string.IsNullOrWhiteSpace(value))
				return HtmlString.Empty;
			return value.Make(template).ToHtml(useTypograf);
		}


		/// <summary>
		/// Выполняет сложное составное форматирование строки по шаблону с поддержкой дополнительных аргументов 
		/// и преобразует результат в безопасную структуру <see cref="HtmlString"/> с применением типографики.
		/// </summary>
		/// <param name="value">Исходное строковое значение.</param>
		/// <param name="template">Шаблон форматирования, поддерживающий подстановку множества аргументов.</param>
		/// <param name="useTypograf">Признак необходимости применения типографа к результирующей строке.</param>
		/// <param name="args">Дополнительный массив аргументов для подстановки в сложный шаблон.</param>
		/// <returns>Объект <see cref="HtmlString"/> с отформатированным и оттипографленным контентом, либо <see cref="HtmlString.Empty"/>.</returns>
		public static HtmlString ToHtmlMore(
			this string? value,
			string? template,
			bool useTypograf,
			params object?[] args)
		{
			if (string.IsNullOrWhiteSpace(value))
				return HtmlString.Empty;
			return value.MakeMore(template, args).ToHtml(useTypograf);
		}

	}

}
