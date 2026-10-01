// rev 2026-10-01

using Ans.Net10.Common;
using Ans.Net10.Common.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации интеллектуального элемента подписи поля <c>&lt;label&gt;</c>.
	/// </summary>
	/// <remarks>
	/// Автоматически рендерит иконку обязательности заполнения поля, интерактивную ссылку на справочную документацию 
	/// и точечные блоки сообщений об ошибках валидации на основе переданной мета-модели.
	/// </remarks>
	public class LabelFieldTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="LabelFieldTag"/>, динамически формируя внутреннюю HTML-разметку подписи.
		/// </summary>
		/// <param name="target">Идентификатор целевого HTML-элемента ввода (атрибут <c>id</c>), к которому привязывается метка.</param>
		/// <param name="isRequired">Признак обязательности заполнения поля. Если <see langword="true"/>, выводится маркер восклицательного знака.</param>
		/// <param name="face">Объект метаданных отображения поля ввода формы.</param>
		/// <param name="errors">Опциональный массив текстовых сообщений об ошибках валидации, относящихся к данному полю.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="target"/> или <paramref name="face"/> равен <see langword="null"/>.</exception>
		public LabelFieldTag(
			string target,
			bool isRequired,
			CrudFace face,
			string[]? errors)
			: base("label", TagRenderMode.Normal)
		{
			ArgumentNullException.ThrowIfNull(target);
			ArgumentNullException.ThrowIfNull(face);
			AddCssClass("form-label");
			MergeAttribute("for", target);
			if (isRequired)
				InnerHtml.AppendHtml(
					$"<i class=\"bi-exclamation-circle text-danger me-1\" title=\"{Form.Text_RequiredField}\"></i>");
			if (face.HasHelpLink)
				InnerHtml.AppendHtml(
					$"<a class=\"link-info me-1\" target=\"_blank\" href=\"{face.HelpLink}\" title=\"{Form.Text_Help}\"><i class=\"bi-question-circle\"></i></a>");
			InnerHtml.AppendHtml(face.Title);
			if (errors?.Length > 0)
				InnerHtml.AppendHtml(
					errors.MakeFromCollection(
						null, "<div class=\"text-danger\">{0}</div>", null));
		}


		/* properties */


		/// <summary>
		/// Контекст текущего выполняемого Razor-представления.
		/// </summary>
		[ViewContext]
		[HtmlAttributeNotBound]
		public ViewContext? ViewContext { get; set; }

	}

}
