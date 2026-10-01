// rev 2026-10-01

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации блочного элемента <c>&lt;div&gt;</c> 
	/// с поддержкой автоматического заполнения пустых блоков.
	/// </summary>
	public class DivTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DivTag"/> с заданным внутренним содержимым.
		/// </summary>
		/// <remarks>
		/// Если переданное содержимое <paramref name="inner"/> равно <see langword="null"/> или является пустым, 
		/// блок автоматически заполняется HTML-сущностью неразрывного пробела (<c>&amp;nbsp;</c>) для предотвращения визуального схлопывания элемента.
		/// </remarks>
		/// <param name="inner">Внутреннее HTML или текстовое содержимое блока.</param>
		public DivTag(
			string? inner)
			: base("div", TagRenderMode.Normal)
		{
			Inner = inner ?? string.Empty;
			InnerHtml.AppendHtml(
				string.IsNullOrEmpty(Inner)
					? "&nbsp;" : Inner);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает исходное внутреннее содержимое блочного элемента.
		/// </summary>
		/// <value>Строка с контентом, переданным при инициализации.</value>
		public string Inner { get; }

	}

}
