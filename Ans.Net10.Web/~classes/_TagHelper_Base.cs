// rev 2026-10-01

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Базовый класс для создания кастомных высокопроизводительных асинхронных компонентов Razor Tag Helper.
	/// </summary>
	/// <remarks>
	/// Инкапсулирует логику управления сквозными атрибутами стиля, классов, заголовков и асинхронного дочернего содержимого.
	/// </remarks>
	public abstract class _TagHelper_Base
		: TagHelper
	{

		/* virtuals */


		/// <summary>
		/// Виртуальный метод для асинхронного внедрения разметки или атрибутов непосредственно перед выводом основного тела тега.
		/// </summary>
		public virtual Task MakeBeforeAsync(
			TagHelperOutput output)
		{
			return Task.CompletedTask;
		}


		/// <summary>
		/// Виртуальный метод для асинхронного внедрения разметки или атрибутов сразу после вывода основного тела тега.
		/// </summary>
		public virtual Task MakeAfterAsync(
			TagHelperOutput output)
		{
			return Task.CompletedTask;
		}


		/* properties */


		/// <summary>
		/// Определяет, требуется ли полностью подавлять вывод тега, если дочернее содержимое пусто.
		/// </summary>
		public bool UseAutoContent { get; set; }


		/// <summary>
		/// CSS-классы, принудительно внедряемые в начало списка атрибута <c>class</c>.
		/// </summary>
		public string? ClassBefore { get; set; }


		/// <summary>
		/// Основной список CSS-классов компонента.
		/// </summary>
		public string? Class { get; set; }


		/// <summary>
		/// CSS-классы, принудительно внедряемые в конец списка атрибута <c>class</c>.
		/// </summary>
		public string? ClassAfter { get; set; }


		/// <summary>
		/// Инлайновые стили элемента (атрибут <c>style</c>).
		/// </summary>
		public string? Style { get; set; }


		/// <summary>
		/// Всплывающая подсказка элемента (атрибут <c>title</c>).
		/// </summary>
		public string? Title { get; set; }


		/// <summary>
		/// Строковое HTML-содержимое по умолчанию, используемое если тело тега в разметке пусто.
		/// </summary>
		public string? DefaultContent { get; set; } = null;


		/// <summary>
		/// Принудительное HTML-содержимое, полностью перекрывающее внутреннее тело тега.
		/// </summary>
		public string? CustomContent { get; set; } = null;


		/* methods */


		/// <summary>
		/// Асинхронно обрабатывает логику работы компонента, собирает атрибуты и управляет рендерингом содержимого.
		/// </summary>
		/// <inheritdoc />
		public override async Task ProcessAsync(
			TagHelperContext context,
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(output);
			var content1 = await GetContentAsync(output);
			if (UseAutoContent && string.IsNullOrWhiteSpace(content1))
			{
				output.TagName = null;
				output.Content.Clear();
				return;
			}
			output.AddAttributeIfPresent("class", ClassBefore, Class, ClassAfter);
			output.AddAttributeIfPresent("style", Style);
			output.AddAttributeIfPresent("title", Title);
			await MakeBeforeAsync(output);
			output.Content.AppendHtml(content1 ?? string.Empty);
			await MakeAfterAsync(output);
		}


		/* functions */


		/// <summary>
		/// Асинхронно извлекает результирующее строковое HTML-содержимое тега на основе кастомных перекрытий или тела разметки.
		/// </summary>
		/// <param name="output">Выходной контекст формирования тега.</param>
		/// <returns>Задача, результатом которой является итоговая строка HTML-контента, либо <see langword="null"/>.</returns>
		public async Task<string?> GetContentAsync(
			TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(output);
			if (CustomContent != null)
				return CustomContent;
			var childContentContext = await output.GetChildContentAsync();
			var content1 = childContentContext.GetContent();
			return string.IsNullOrWhiteSpace(content1)
				? DefaultContent : content1;
		}

	}

}
