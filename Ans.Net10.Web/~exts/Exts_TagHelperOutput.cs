// rev 2026-09-30

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="TagHelperOutput"/>, упрощающие манипуляцию HTML-атрибутами 
	/// и содержимым внутри компонентов Tag Helper.
	/// </summary>
	public static partial class Exts_TagHelperOutput
	{

		/* methods */


		/// <summary>
		/// Безопасно добавляет значение к существующему HTML-атрибуту через разделитель, либо создает его, если он отсутствует.
		/// </summary>
		/// <param name="output">Контекст вывода текущего Tag Helper'а.</param>
		/// <param name="name">Имя HTML-атрибута (например, "class" или "style").</param>
		/// <param name="value">Добавляемое строковое значение.</param>
		/// <param name="separator">Строка-разделитель, используемая при конкатенации (например, пробел или точка с запятой).</param>
		public static void AddValueToAttribute(
			this TagHelperOutput output,
			string name,
			string value,
			string separator)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentException.ThrowIfNullOrEmpty(name);
			if (output.Attributes.TryGetAttribute(name, out var existingAttribute1))
				output.Attributes.SetAttribute(
					name, $"{existingAttribute1.Value}{separator}{value}");
			else
				output.Attributes.Add(name, value);
		}


		/// <summary>
		/// Безопасно добавляет HTML-атрибут в коллекцию, если результирующее строковое значение, 
		/// собранное из переданного массива элементов, не является пустым.
		/// </summary>
		/// <remarks>
		/// Метод автоматически игнорирует и отбрасывает элементы массива, которые равны <see langword="null"/>, 
		/// пустые или состоят исключительно из пробелов, исключая появление лишних разделительных пробелов в разметке.
		/// </remarks>
		/// <param name="output">Текущий экземпляр контекста формирования тега.</param>
		/// <param name="name">Системное имя добавляемого HTML-атрибута (например, <c>"class"</c> или <c>"style"</c>).</param>
		/// <param name="values">Массив строк, составляющих значение атрибута. Пустые элементы отбрасываются.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="output"/> равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Вызывается, если имя атрибута <paramref name="name"/> равно <see langword="null"/> или является пустой строкой.</exception>
		public static void AddAttributeIfPresent(
			this TagHelperOutput output,
			string name,
			params string?[]? values)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentException.ThrowIfNullOrEmpty(name);
			if (values == null || values.Length == 0)
				return;
			var cleanValues1 = values
				.Where(x => !string.IsNullOrWhiteSpace(x));
			var s1 = string.Join(" ", cleanValues1);
			if (!string.IsNullOrEmpty(s1))
				output.Attributes.Add(name, s1);
		}


		/// <summary>
		/// Записывает готовую HTML-строку напрямую во внутренний контент вывода Tag Helper'а без повторного экранирования.
		/// </summary>
		/// <param name="output">Контекст вывода текущего Tag Helper'а.</param>
		/// <param name="encoded">Форматированная HTML-строка.</param>
		public static void AppendHtml(
			this TagHelperOutput output,
			string encoded)
		{
			ArgumentNullException.ThrowIfNull(output);
			output.Content.AppendHtml(encoded);
		}


		/* functions */


		/// <summary>
		/// Асинхронно извлекает и материализует полное строковое содержимое дочерних элементов (внутренний HTML) текущего Tag Helper'а.
		/// </summary>
		/// <param name="output">Контекст вывода текущего Tag Helper'а.</param>
		/// <returns>Поток-задача, возвращающая текстовое содержимое дочернего контента.</returns>
		public async static Task<string> GetChildContentAsync(
			this TagHelperOutput output)
		{
			ArgumentNullException.ThrowIfNull(output);
			var content1 = await output.GetChildContentAsync();
			return content1.GetContent();
		}

	}

}
