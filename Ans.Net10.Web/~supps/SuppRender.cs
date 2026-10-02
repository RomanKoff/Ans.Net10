// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для генерации и рендеринга стандартных демонстрационных текстовых фрагментов (рыба-текст)
	/// в виде безопасных HTML-строк с автоматическим наложением правил экранной типографики.
	/// </summary>
	public static class SuppRender
	{

		/// <summary>
		/// Генерирует стандартный демонстрационный текст средней длины на русском языке
		/// и возвращает его в виде безопасной HTML-строки с примененной типографикой.
		/// </summary>
		/// <returns>Объект <see cref="HtmlString"/>, содержащий оттипографленный демонстрационный текст.</returns>
		public static HtmlString SampleRu()
			=> SuppLangRu.GetSample().ToHtml(true);


		/// <summary>
		/// Генерирует короткий демонстрационный текст на русском языке
		/// и возвращает его в виде безопасной HTML-строки с примененной типографикой.
		/// </summary>
		/// <returns>Объект <see cref="HtmlString"/>, содержащий короткий оттипографленный демонстрационный текст.</returns>
		public static HtmlString SampleSmallRu()
			=> SuppLangRu.GetSampleSmall().ToHtml(true);


		/// <summary>
		/// Генерирует ультракороткий демонстрационный текст на русском языке
		/// и возвращает его в виде безопасной HTML-строки с примененной типографикой.
		/// </summary>
		/// <returns>Объект <see cref="HtmlString"/>, содержащий ультракороткий оттипографленный демонстрационный текст.</returns>
		public static HtmlString SampleSmallerRu()
			=> SuppLangRu.GetSampleSmaller().ToHtml(true);

	}

}
