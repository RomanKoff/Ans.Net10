// rev 2026-10-02

using Ans.Net10.Common;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper для автоматической генерации HTML-разметки <c>&lt;span&gt;</c>,
	/// содержащей имя, полную информационную версию и описание текущей сборки веб-библиотеки.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="InfoVersionTagHelper"/> с внедрением контекста.
	/// </remarks>
	/// <param name="current">Текущий оркестровый контекст выполнения HTTP-запроса.</param>
	[HtmlTargetElement("info-version", TagStructure = TagStructure.WithoutEndTag)]
	public class InfoVersionTagHelper(
		CurrentContext current)
		: _AnsTagHelper_Base(current)
	{

		/// <summary>
		/// Асинхронно обрабатывает Tag Helper, настраивая имя тега, режим рендеринга
		/// и принудительно подставляя текстовые метаданные версии сборки в качестве содержимого.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="output"/> равен <see langword="null"/>.
		/// </exception>
		/// <remarks>
		/// Метод переводит элемент в парный режим <see cref="TagMode.StartTagAndEndTag"/>, преобразует его в тег <c>span</c>
		/// и полностью перекрывает внутреннее тело разметки значением свойства <see cref="_TagHelper_Base.CustomContent"/>, 
		/// форматируя данные с помощью метода расширения <see cref="Exts__make.Make(string, string)"/>.
		/// </remarks>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "span";
			CustomContent = $"{LibWebInfo.Name} {LibWebInfo.FullVersion}{LibWebInfo.Description.Make(" ({0})")}";
			return base.ProcessAsync(context, output);
		}

	}

}
