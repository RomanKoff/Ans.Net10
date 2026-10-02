// rev 2026-010-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для класса метаданных <see cref="CrudFace"/>,
	/// обеспечивающие безопасную трансформацию текстовых описаний полей в HTML-строки с применением экранной типографики.
	/// </summary>
	public static partial class Exts_CrudFace
	{

		/// <summary>
		/// Преобразует основное наименование (заголовок) поля в безопасную HTML-строку с наложением правил типографики.
		/// </summary>
		/// <param name="helper">Текущий экземпляр описания метаданных поля <see cref="CrudFace"/>.</param>
		/// <returns>
		/// Объект <see cref="HtmlString"/>, содержащий отформатированный заголовок поля. 
		/// Если заголовок не задан, возвращает пустую HTML-строку.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если расширяемый параметр <paramref name="helper"/> равен <see langword="null"/>.
		/// </exception>
		public static HtmlString TitleHtml(
			this CrudFace helper)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return helper.Title.ToHtml(true);
		}


		/// <summary>
		/// Преобразует сокращенное наименование поля в безопасную HTML-строку с наложением правил типографики.
		/// </summary>
		/// <param name="helper">Текущий экземпляр описания метаданных поля <see cref="CrudFace"/>.</param>
		/// <returns>
		/// Объект <see cref="HtmlString"/>, содержащий отформатированное краткое имя поля. 
		/// Если краткое имя отсутствует, возвращает пустую HTML-строку.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если расширяемый параметр <paramref name="helper"/> равен <see langword="null"/>.
		/// </exception>
		public static HtmlString ShortTitleHtml(
			this CrudFace helper)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return helper.ShortTitle.ToHtml(true);
		}


		/// <summary>
		/// Преобразует подробное всплывающее описание (подсказку) поля в безопасную HTML-строку с наложением правил типографики.
		/// </summary>
		/// <param name="helper">Текущий экземпляр описания метаданных поля <see cref="CrudFace"/>.</param>
		/// <returns>
		/// Объект <see cref="HtmlString"/>, содержащий отформатированный текст описания. 
		/// Если описание не заполнено, возвращает пустую HTML-строку.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если расширяемый параметр <paramref name="helper"/> равен <see langword="null"/>.
		/// </exception>
		public static HtmlString DescriptionHtml(
			this CrudFace helper)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return helper.Description.ToHtml(true);
		}


		/// <summary>
		/// Преобразует демонстрационный пример заполнения поля в безопасную HTML-строку с наложением правил типографики.
		/// </summary>
		/// <param name="helper">Текущий экземпляр описания метаданных поля <see cref="CrudFace"/>.</param>
		/// <returns>
		/// Объект <see cref="HtmlString"/>, содержащий отформатированный текст примера ввода. 
		/// Если пример отсутствует, возвращает пустую HTML-строку.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если расширяемый параметр <paramref name="helper"/> равен <see langword="null"/>.
		/// </exception>
		public static HtmlString SampleHtml(
			this CrudFace helper)
		{
			ArgumentNullException.ThrowIfNull(helper);
			return helper.Sample.ToHtml(true);
		}

	}

}
