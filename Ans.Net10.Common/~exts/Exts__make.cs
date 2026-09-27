// rev 2026-09-26

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для продвинутого и безопасного форматирования строк, примитивных типов, коллекций и перечислений по шаблонам.
	/// </summary>
	public static partial class Exts__make
	{

		/// <summary>
		/// Форматирует строку по шаблону. Возвращает пустую строку, если исходное значение пустое.
		/// </summary>
		/// <param name="value">Исходная строка для форматирования. Допускает значение <see langword="null"/>.</param>
		/// <param name="template">Шаблон форматирования, совместимый с <see cref="string.Format(string, object)"/>. Допускает значение <see langword="null"/>.</param>
		/// <returns>Отформатированная строка; <see cref="string.Empty"/>, если параметр <paramref name="value"/> равен <see langword="null"/> или пуст.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Make(
			this string? value,
			string? template)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			if (string.IsNullOrEmpty(template))
				return value;
			return string.Format(template, value);
		}


		/// <summary>
		/// Форматирует строку по шаблону. Возвращает пустую строку, если значение совпадает со значением-маркером пустой строки.
		/// </summary>
		/// <param name="value">Исходная строка для форматирования. Допускает значение <see langword="null"/>.</param>
		/// <param name="template">Шаблон форматирования, совместимый с <see cref="string.Format(string, object)"/>. Допускает значение <see langword="null"/>.</param>
		/// <param name="nullValue">Строковое значение, при совпадении с которым результат принудительно сбрасывается в пустую строку.</param>
		/// <returns>Отформатированная строка или <see cref="string.Empty"/>, если значение эквивалентно <paramref name="nullValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Make(
			this string? value,
			string? template,
			string? nullValue)
		{
			if (string.Equals(value, nullValue))
				return string.Empty;
			if (string.IsNullOrEmpty(template))
				return value ?? string.Empty;
			return string.Format(template, value);
		}


		/// <summary>
		/// Форматирует строку по шаблону с поддержкой внедрения дополнительных аргументов в шаблон.
		/// </summary>
		/// <param name="value">Исходная строка, которая всегда подставляется в качестве аргумента <c>{0}</c>. Допускает значение <see langword="null"/>.</param>
		/// <param name="template">Шаблон форматирования, поддерживающий несколько индексов аргументов. Допускает значение <see langword="null"/>.</param>
		/// <param name="args">Дополнительные аргументы, которые будут подставлены в шаблон начиная с индекса <c>{1}</c>.</param>
		/// <returns>Результат комплексного форматирования строки или <see cref="string.Empty"/>, если <paramref name="value"/> пуст.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string MakeMore(
			this string? value,
			string? template,
			params object?[] args)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			if (string.IsNullOrEmpty(template))
				return value;
			object?[] args1 = [value, .. args];
			return string.Format(template, args1);
		}


		/// <summary>
		/// Форматирует значимый тип (структуру) по шаблону, возвращая пустую строку в случае совпадения со значением-маркером.
		/// </summary>
		/// <typeparam name="T">Тип структуры, реализующий интерфейс <see cref="IFormattable"/>.</typeparam>
		/// <param name="value">Исходный значимый объект для форматирования.</param>
		/// <param name="template">Строковый шаблон обертки результата.</param>
		/// <param name="format">Спецификатор формата для самого объекта типа <typeparamref name="T"/>.</param>
		/// <param name="nullValue">Значение структуры, при эквивалентности которому возвращается пустая строка.</param>
		/// <param name="provider">Механизм предоставления региональных настроек. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <returns>Итоговая отформатированная строка или <see cref="string.Empty"/>.</returns>
		public static string Make<T>(
			this T value,
			string? template,
			string? format,
			T nullValue,
			IFormatProvider? provider = null)
			where T : struct, IFormattable
		{
			if (value.Equals(nullValue))
				return string.Empty;
			provider ??= CultureInfo.InvariantCulture;
			string s1 = string.IsNullOrEmpty(format)
				? value.ToString(null, provider)
				: value.ToString(format, provider);
			return s1.Make(template);
		}


		/// <summary>
		/// Форматирует структуру с автоматическим определением и пропуском дефолтных системных пустых значений.
		/// </summary>
		/// <remarks>
		/// К пустым значениям автоматически относятся: <see cref="DateTime.MinValue"/>, <see cref="DateOnly.MinValue"/>, <see cref="TimeOnly.MinValue"/>, а также дефолтное состояние структуры.
		/// </remarks>
		/// <typeparam name="T">Тип структуры, реализующий интерфейс <see cref="IFormattable"/>.</typeparam>
		/// <param name="value">Исходный значимый объект для форматирования.</param>
		/// <param name="template">Строковый шаблон обертки результата.</param>
		/// <param name="format">Спецификатор формата для объекта. По умолчанию равен <see langword="null"/>.</param>
		/// <param name="provider">Поставщик форматирования. По умолчанию равен <see langword="null"/>.</param>
		/// <returns>Отформатированная строка или <see cref="string.Empty"/> для пустых граничных значений.</returns>
		public static string Make<T>(
			this T value,
			string? template,
			string? format = null,
			IFormatProvider? provider = null)
			where T : struct, IFormattable
		{
			if (value is DateTime dateTime && dateTime == DateTime.MinValue
				|| value is DateOnly dateOnly && dateOnly == DateOnly.MinValue
				|| value is TimeOnly timeOnly && timeOnly == TimeOnly.MinValue
				|| value.Equals(default(T)))
				return string.Empty;
			return value.Make(template, format, default, provider);
		}


		/// <summary>
		/// Форматирует nullable-структуру. Если значение отсутствует, сразу возвращает пустую строку.
		/// </summary>
		/// <typeparam name="T">Базовый тип структуры, реализующий интерфейс <see cref="IFormattable"/>.</typeparam>
		/// <param name="value">Экземпляр nullable-структуры для обработки.</param>
		/// <param name="template">Строковый шаблон обертки результата.</param>
		/// <param name="format">Спецификатор формата объекта. По умолчанию равен <see langword="null"/>.</param>
		/// <param name="provider">Поставщик форматирования. По умолчанию равен <see langword="null"/>.</param>
		/// <returns>Строковый результат форматирования или <see cref="string.Empty"/>, если свойство <see cref="Nullable{T}.HasValue"/> возвращает <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Make<T>(
			this T? value,
			string? template,
			string? format = null,
			IFormatProvider? provider = null)
			where T : struct, IFormattable
		{
			return value.HasValue
				? value.Value.Make(template, format, provider)
				: string.Empty;
		}


		/// <summary>
		/// Форматирует значение перечисления по шаблону. Поддерживает автоматическое извлечение текста из атрибута <see cref="DescriptionAttribute"/>.
		/// </summary>
		/// <typeparam name="T">Тип перечисления, унаследованный от <see cref="Enum"/>.</typeparam>
		/// <param name="value">Конкретное значение перечисления.</param>
		/// <param name="template">Строковый шаблон обертки результата.</param>
		/// <returns>Описание из атрибута <see cref="DescriptionAttribute"/> или имя элемента, оформленное по шаблону.</returns>
		public static string Make<T>(
			this T value,
			string? template)
			where T : struct, Enum
		{
			string? name1 = Enum.GetName(value);
			if (string.IsNullOrEmpty(name1))
				return string.Empty;
			var field1 = typeof(T).GetField(name1);
			if (field1?.GetCustomAttribute<DescriptionAttribute>() is { Description: { Length: > 0 } desc1 })
				name1 = desc1;
			return name1.Make(template);
		}


		/// <summary>
		/// Форматирует значение перечисления по шаблону, возвращая пустую строку при равенстве значению-маркеру отсутствия данных.
		/// </summary>
		/// <typeparam name="T">Тип перечисления, унаследованный от <see cref="Enum"/>.</typeparam>
		/// <param name="value">Конкретное значение перечисления.</param>
		/// <param name="template">Строковый шаблон обертки результата.</param>
		/// <param name="nullValue">Значение перечисления, при равенстве которому возвращается пустая строка.</param>
		/// <returns>Строковый результат форматирования перечисления или пустая строка.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Make<T>(
			this T value,
			string? template,
			T nullValue)
			where T : struct, Enum
		{
			return value.Equals(nullValue)
				? string.Empty
				: value.Make(template);
		}


		/// <summary>
		/// Возвращает заданный текст для истинного состояния или альтернативный текст/пустую строку для ложного состояния.
		/// </summary>
		/// <param name="value">Логическое значение флажка.</param>
		/// <param name="trueText">Строка, возвращаемая, если флаг равен <see langword="true"/>.</param>
		/// <param name="falseText">Строка, возвращаемая, если флаг равен <see langword="false"/>. По умолчанию равен <see langword="null"/> (возвращается <see cref="string.Empty"/>).</param>
		/// <returns>Один из двух строковых вариантов в зависимости от состояния флага.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Make(
			this bool value,
			string trueText,
			string? falseText = null)
		{
			return value
				? trueText
				: (falseText ?? string.Empty);
		}


		/// <summary>
		/// Форматирует сложную строку по шаблону с аргументами только при условии, что логическое значение истинно.
		/// </summary>
		/// <param name="value">Условие применения форматирования.</param>
		/// <param name="template">Шаблон форматирования строки.</param>
		/// <param name="args">Параметры, подставляемые в шаблон.</param>
		/// <returns>Результат выполнения <see cref="string.Format(string, object?[])"/>, если флаг равен <see langword="true"/>; иначе — <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string MakeMore(
			this bool value,
			string template,
			params object?[] args)
		{
			return value
				? string.Format(template, args)
				: string.Empty;
		}


		/// <summary>
		/// Повторяет строку заданное количество раз, опционально форматируя элементы и объединяя их через разделитель.
		/// </summary>
		/// <param name="value">Повторяемая строка. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Количество повторений элемента.</param>
		/// <param name="resultTemplate">Общий шаблон обертки для результирующей объединенной строки.</param>
		/// <param name="itemTemplate">Шаблон форматирования, применяемый индивидуально к каждому повторяющемуся элементу.</param>
		/// <param name="itemsSeparator">Символ или строка-разделитель между элементами.</param>
		/// <returns>Итоговая строка из размноженных элементов или <see cref="string.Empty"/>.</returns>
		public static string MakeRepeats(
			this string? value,
			int count,
			string? resultTemplate = null,
			string? itemTemplate = null,
			string? itemsSeparator = null)
		{
			if (string.IsNullOrEmpty(value) || count < 1)
				return string.Empty;
			var item1 = value.Make(itemTemplate);
			if (count == 1)
				return item1.Make(resultTemplate);
			int separatorLength1 = itemsSeparator?.Length ?? 0;
			int estimatedCapacity1 = (item1.Length * count) + (separatorLength1 * (count - 1));
			var sb1 = new StringBuilder(estimatedCapacity1);
			for (int i1 = 0; i1 < count; i1++)
			{
				sb1.Append(item1);
				if (i1 < count - 1 && separatorLength1 > 0)
					sb1.Append(itemsSeparator);
			}
			return sb1.ToString().Make(resultTemplate);
		}


		/// <summary>
		/// Собирает единую строку из коллекции строк, автоматически пропуская пустые элементы, форматируя их и склеивая через разделитель.
		/// </summary>
		/// <param name="items">Коллекция строк. Допускает значение <see langword="null"/>.</param>
		/// <param name="resultTemplate">Общий шаблон обертки для итоговой собранной строки.</param>
		/// <param name="itemTemplate">Шаблон форматирования, накладываемый на каждый непустой элемент коллекции.</param>
		/// <param name="itemsSeparator">Строка-разделитель между склеиваемыми элементами.</param>
		/// <returns>Склеенная и отформатированная строка; <see cref="string.Empty"/>, если коллекция пуста.</returns>
		public static string MakeFromCollection(
			this IEnumerable<string?>? items,
			string? resultTemplate = null,
			string? itemTemplate = null,
			string? itemsSeparator = null)
		{
			if (items == null)
				return string.Empty;
			var sb1 = new StringBuilder();
			bool isFirst1 = true;
			foreach (var item1 in items)
			{
				if (string.IsNullOrEmpty(item1))
					continue;
				var formattedItem1 = string.IsNullOrEmpty(itemTemplate)
					? item1 : item1.Make(itemTemplate);
				if (string.IsNullOrEmpty(formattedItem1))
					continue;
				if (!isFirst1 && !string.IsNullOrEmpty(itemsSeparator))
					sb1.Append(itemsSeparator);
				sb1.Append(formattedItem1);
				isFirst1 = false;
			}
			var result1 = sb1.ToString();
			return string.IsNullOrEmpty(result1)
				? string.Empty
				: result1.Make(resultTemplate);
		}


		/// <summary>
		/// Собирает единую строку из коллекции любых объектов, извлекая из них текстовые значения с помощью кастомной функции-экстрактора.
		/// </summary>
		/// <typeparam name="T">Тип объектов в коллекции.</typeparam>
		/// <param name="items">Коллекция элементов типа <typeparamref name="T"/>. Допускает значение <see langword="null"/>.</param>
		/// <param name="itemExtractor">Делегат функции, отвечающий за преобразование объекта в строковый вид.</param>
		/// <param name="resultTemplate">Общий шаблон обертки для итоговой строки.</param>
		/// <param name="itemTemplate">Шаблон форматирования индивидуального извлеченного строкового элемента.</param>
		/// <param name="itemsSeparator">Строка-разделитель между элементами.</param>
		/// <returns>Собранная строка из свойств объектов коллекции или <see cref="string.Empty"/>.</returns>
		public static string MakeFromCollection<T>(
			this IEnumerable<T>? items,
			Func<T, string?> itemExtractor,
			string? resultTemplate = null,
			string? itemTemplate = null,
			string? itemsSeparator = null)
		{
			if (items == null || itemExtractor == null)
				return string.Empty;
			var sb1 = new StringBuilder();
			bool isFirst1 = true;
			foreach (var item1 in items)
			{
				if (item1 is null)
					continue;
				string? extracted = itemExtractor(item1);
				if (string.IsNullOrEmpty(extracted))
					continue;
				string formattedItem1 = string.IsNullOrEmpty(itemTemplate)
					? extracted : extracted.Make(itemTemplate);
				if (string.IsNullOrEmpty(formattedItem1))
					continue;
				if (!isFirst1 && !string.IsNullOrEmpty(itemsSeparator))
					sb1.Append(itemsSeparator);
				sb1.Append(formattedItem1);
				isFirst1 = false;
			}
			var result1 = sb1.ToString();
			return string.IsNullOrEmpty(result1)
				? string.Empty
				: result1.Make(resultTemplate);
		}


		/// <summary>
		/// Ищет значение по ключу в словаре, преобразует его через функцию-экстрактор и форматирует по шаблону. Если ключ не найден, форматирует сам ключ.
		/// </summary>
		/// <typeparam name="T">Тип значений, хранящихся в словаре.</typeparam>
		/// <param name="key">Ключ для поиска в словаре. Допускает значение <see langword="null"/>.</param>
		/// <param name="dictionary">Словарь-база данных. Допускает значение <see langword="null"/>.</param>
		/// <param name="itemExtractor">Функция-обработчик найденного объекта типа <typeparamref name="T"/> для перевода его в строку.</param>
		/// <param name="template">Шаблон форматирования итоговой строки.</param>
		/// <returns>Строковый результат применения шаблона к найденному значению или к самому ключу.</returns>
		public static string MakeOverDict<T>(
			this string? key,
			IDictionary<string, T>? dictionary,
			Func<T, string?> itemExtractor,
			string? template)
		{
			if (string.IsNullOrEmpty(key) || dictionary == null || itemExtractor == null)
				return string.Empty;
			string? s1 = dictionary.TryGetValue(key, out var value)
				? itemExtractor(value) : key;
			return (s1 ?? string.Empty).Make(template);
		}


		/// <summary>
		/// Ищет строковое значение по ключу в словаре и форматирует его по шаблону. Если ключ не найден, форматирует по шаблону сам ключ.
		/// </summary>
		/// <param name="key">Ключ для поиска в словаре. Допускает значение <see langword="null"/>.</param>
		/// <param name="dictionary">Словарь строковых данных. Допускает значение <see langword="null"/>.</param>
		/// <param name="template">Шаблон форматирования итоговой строки.</param>
		/// <returns>Результат применения шаблона к значению из словаря или к оригинальному ключу.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string MakeOverDict(
			this string? key,
			IDictionary<string, string?>? dictionary,
			string? template)
		{
			if (string.IsNullOrEmpty(key) || dictionary == null)
				return string.Empty;
			string? s1 = dictionary.TryGetValue(key, out var value)
				? value : key;
			return (s1 ?? string.Empty).Make(template);
		}


		/// <summary>
		/// Форматирует URL-адрес по шаблону. Автоматически подставляет протокол 'https://', если у адреса отсутствует веб-префикс или относительный путь.
		/// </summary>
		/// <param name="url">Исходный веб-адрес или путь. Допускает значение <see langword="null"/>.</param>
		/// <param name="template">Шаблон форматирования (например, для создания HTML-тега ссылки).</param>
		/// <returns>Нормализованный валидный URL-адрес, оформленный по правилам шаблона.</returns>
		public static string Make_UrlWrapper(
			this string? url,
			string? template)
		{
			if (string.IsNullOrEmpty(url))
				return string.Empty;
			if (url.StartsWith('/')
				|| url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
				|| url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
				return url.Make(template);
			return $"https://{url}".Make(template);
		}

	}

}
