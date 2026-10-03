// rev 2026-10-02

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web.TagHelpers
{

	/// <summary>
	/// Асинхронный Tag Helper для элемента <c>&lt;input-hidden&gt;</c>, обеспечивающий автоматическую
	/// генерацию скрытого поля ввода на основе переданного выражения модели (Model Expression).
	/// </summary>
	[HtmlTargetElement("input-hidden", Attributes = _ATTR_FOR, TagStructure = TagStructure.WithoutEndTag)]
	public class InputHiddenTagHelper
		: _TagHelper_Base
	{

		private const string _ATTR_FOR = "for";


		/// <summary>
		/// Получает или задает контекст текущего выполняемого Razor-представления.
		/// Данное свойство заполняется автоматически и не привязывается к HTML-атрибутам.
		/// </summary>
		[ViewContext]
		[HtmlAttributeNotBound]
		public ViewContext ViewContext { get; set; } = null!;


		/* attributes */


		/// <summary>
		/// Получает или задает выражение модели, определяющее свойство DTO или сущности,
		/// к которому привязывается скрытое поле ввода.
		/// </summary>
		[HtmlAttributeName(_ATTR_FOR)]
		public ModelExpression For { get; set; } = null!;


		/* methods */


		/// <summary>
		/// Асинхронно обрабатывает логику работы скрытого поля, настраивая атрибуты типа, 
		/// идентификатора, имени и текущего значения на основе метаданных привязки.
		/// </summary>
		/// <param name="context">Контекст выполнения, содержащий информацию о текущем HTML-теге.</param>
		/// <param name="output">Выходной контекст, используемый для формирования результирующего HTML-кода.</param>
		/// <returns>Задача, представляющая асинхронную операцию выполнения генерации тега.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="output"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Вызывается, если метаданные свойства модели не инициализированы или атрибут 'for' указан неверно.
		/// </exception>
		public override Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			output.TagMode = TagMode.SelfClosing;
			output.TagName = "input";
			if (For.Metadata == null)
				throw new InvalidOperationException(
					"[Ans.Net10.Web] <input-hidden /> 'for' attribute required.");
			output.Attributes.Add("type", "hidden");
			string name1 = For.Name;
			output.Attributes.Add("id", name1);
			output.Attributes.Add("name", name1);
			string? value1 = For.GetModelValueString();
			output.AddAttributeIfPresent("value", value1);
			return base.ProcessAsync(context, output);
		}

	}

}
