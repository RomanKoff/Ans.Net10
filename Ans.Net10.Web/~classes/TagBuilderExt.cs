// rev 2026-09-28

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Расширенный построитель HTML-тегов, предоставляющий удобные методы пакетного применения 
	/// классов, стилей, атрибутов и быстрой трансформации в безопасные HTML-строки.
	/// </summary>
	public class TagBuilderExt
		: TagBuilder
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagBuilderExt"/> с указанием имени тега и режима его рендеринга.
		/// </summary>
		/// <param name="tagName">Имя HTML-тега (например, <c>"div"</c>, <c>"span"</c>).</param>
		/// <param name="mode">Режим рендеринга тега (открытый, закрытый, самозакрывающийся).</param>
		public TagBuilderExt(
			string tagName,
			TagRenderMode mode)
			: base(tagName)
		{
			TagRenderMode = mode;
		}


		/* methods */


		/// <summary>
		/// Пакетно применяет к текущему тегу коллекции классов, инлайновых стилей и кастомных атрибутов, 
		/// используя специализированные билдеры из инфраструктуры <c>Common</c>.
		/// </summary>
		/// <param name="classes">Билдер коллекции CSS-классов тега.</param>
		/// <param name="styles">Билдер коллекции инлайновых CSS-стилей тега.</param>
		/// <param name="attributes">Билдер коллекции произвольных HTML-атрибутов.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если <paramref name="classes"/>, <paramref name="styles"/> или <paramref name="attributes"/> равны <see langword="null"/>.
		/// </exception>
		public void Apply(
			TagClassesBuilder classes,
			TagStylesBuilder styles,
			TagAttributesBuilder attributes)
		{
			ArgumentNullException.ThrowIfNull(classes);
			ArgumentNullException.ThrowIfNull(styles);
			ArgumentNullException.ThrowIfNull(attributes);
			if (classes.Items.Count > 0)
				this.ExpandClassAttribute(classes.ToString());
			if (styles.Items.Count > 0)
				this.ExpandStyleAttribute(styles.ToString());
			foreach (var item1 in attributes.Items)
				MergeAttribute(item1.Key, item1.Value);
		}


		/// <summary>
		/// Пакетно инициализирует и применяет к текущему тегу коллекции классов, стилей и атрибутов 
		/// на основе их сырых строковых определений.
		/// </summary>
		/// <param name="classes">Сырая строка CSS-классов (например, <c>"btn btn-primary"</c>). Допускает <see langword="null"/>.</param>
		/// <param name="styles">Сырая строка инлайновых CSS-стилей (например, <c>"display:none;color:red"</c>). Допускает <see langword="null"/>.</param>
		/// <param name="attributes">Сырая строка HTML-атрибутов (например, <c>"data-id=10;disabled"</c>). Допускает <see langword="null"/>.</param>
		public void Apply(
			string? classes,
			string? styles,
			string? attributes)
		{
			var classesBuilder1 = new TagClassesBuilder(classes);
			var stylesBuilder1 = new TagStylesBuilder(styles);
			var attributesBuilder1 = new TagAttributesBuilder(attributes);
			Apply(classesBuilder1, stylesBuilder1, attributesBuilder1);
		}


		/* functions */


		/// <summary>
		/// Рендерит HTML-тег со всеми его дочерними элементами и атрибутами в виде обычной строки.
		/// </summary>
		/// <returns>Строковое представление сгенерированного HTML-кода.</returns>
		public override string ToString()
		{
			using var sw1 = new StringWriter();
			WriteTo(sw1, HtmlEncoder.Default);
			return sw1.ToString();
		}


		/// <summary>
		/// Преобразует текущий сгенерированный тег в безопасную для вывода в Razor-представлениях структуру <see cref="HtmlString"/>.
		/// </summary>
		/// <returns>Экземпляр <see cref="HtmlString"/>, готовый к рендерингу без принудительного экранирования.</returns>
		public HtmlString ToHtml()
		{
			return new HtmlString(ToString());
		}

	}

}
