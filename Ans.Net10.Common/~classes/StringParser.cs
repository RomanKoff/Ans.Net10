// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Класс для безопасного парсинга строк с разделителями
	/// и извлечения типизированных данных по индексам.
	/// </summary>
	public class StringParser
	{

		private readonly string[] _items;
		private readonly int _count;


		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="StringParser"/>,
		/// используя указанный символ в качестве разделителя.
		/// </summary>
		/// <param name="source">Исходная строка для парсинга.</param>
		/// <param name="separator">Символ-разделитель элементов.</param>
		public StringParser(
			string? source,
			char separator)
		{
			if (string.IsNullOrEmpty(source))
			{
				_items = [];
				_count = 0;
				return;
			}
			_items = source.Split(separator);
			_count = _items.Length;
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="StringParser"/>,
		/// используя строку-разделитель или символ '|' по умолчанию.
		/// </summary>
		/// <param name="source">Исходная строка для парсинга.</param>
		/// <param name="separator">
		/// Строка-разделитель элементов. Если передано значение <see langword="null"/>, используется символ '|'.
		/// </param>
		public StringParser(
			string? source,
			string? separator = null)
		{
			if (string.IsNullOrEmpty(source))
			{
				_items = [];
				_count = 0;
				return;
			}
			_items = separator == null
				? source.Split('|')
				: source.Split(separator);
			_count = _items.Length;
		}


		/* functions */


		/// <summary>
		/// Безопасно возвращает строковое значение по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="defaultValue">Значение по умолчанию, если индекс вне границ диапазона или элемент пуст.</param>
		/// <returns>
		/// Строковое значение элемента, если индекс корректен и элемент не пуст; 
		/// в противном случае — значение <paramref name="defaultValue"/>.
		/// </returns>
		public string? Get(
			int index,
			string? defaultValue = null)
		{
			if (index >= 0 && index < _count)
			{
				var item1 = _items[index];
				return string.IsNullOrEmpty(item1)
					? defaultValue : item1;
			}
			return defaultValue;
		}


		/*--- int ---*/


		/// <summary>
		/// Возвращает целочисленное значение по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <returns>
		/// Числовое значение типа <see cref="int"/>, если конвертация прошла успешно; 
		/// в противном случае — значение <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int? GetInt(
			int index)
		{
			var value1 = Get(index);
			return value1?.ToInt();
		}


		/// <summary>
		/// Возвращает целочисленное значение по указанному индексу или заданное значение по умолчанию.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="defaultValue">Значение, возвращаемое при невозможности конвертации или выходе за границы выборки.</param>
		/// <returns>
		/// Преобразованное целое число, либо значение <paramref name="defaultValue"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetInt(
			int index,
			int defaultValue)
		{
			var value1 = Get(index);
			return value1 != null
				? value1.ToInt(defaultValue) : defaultValue;
		}


		/*--- bool ---*/


		/// <summary>
		/// Возвращает логическое значение по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <returns>
		/// Значение <see langword="true"/>, если строковое содержимое ячейки эквивалентно "1", "+" или "true" (без учета регистра); 
		/// во всех остальных случаях — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetBool(
			int index)
		{
			var value = Get(index);
			return value != null && value.ToBool();
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Возвращает значение даты и времени по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <returns>
		/// Объект <see cref="DateTime"/>, если преобразование прошло успешно; 
		/// в противном случае — <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DateTime? GetDateTime(
			int index)
		{
			var value = Get(index);
			return value?.ToDateTime();
		}


		/// <summary>
		/// Возвращает значение даты и времени по указанному индексу или заданное значение по умолчанию.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="defaultValue">Значение, возвращаемое по умолчанию в случае ошибки парсинга.</param>
		/// <returns>
		/// Преобразованное значение даты и времени, либо <paramref name="defaultValue"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DateTime GetDateTime(
			int index,
			DateTime defaultValue)
		{
			return GetDateTime(index) ?? defaultValue;
		}


		/// <summary>
		/// Возвращает отформатированную строку даты и времени по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="format">Стандартная или настраиваемая строка формата даты и времени.</param>
		/// <returns>
		/// Строковое представление даты и времени в заданном формате, 
		/// или <see langword="null"/>, если конвертация исходного элемента не удалась.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetDateTime(
			int index,
			string format)
		{
			return GetDateTime(index)?.ToString(format);
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Возвращает значение даты по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <returns>
		/// Объект <see cref="DateOnly"/>, если преобразование прошло успешно; 
		/// в противном случае — <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DateOnly? GetDateOnly(
			int index)
		{
			var value = Get(index);
			return value?.ToDateOnly();
		}


		/// <summary>
		/// Возвращает значение даты по указанному индексу или заданное значение по умолчанию.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="defaultValue">Значение, возвращаемое по умолчанию в случае ошибки парсинга.</param>
		/// <returns>
		/// Преобразованное значение даты, либо <paramref name="defaultValue"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DateOnly GetDateOnly(
			int index,
			DateOnly defaultValue)
		{
			return GetDateOnly(index) ?? defaultValue;
		}


		/// <summary>
		/// Возвращает отформатированную строку даты по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="format">Стандартная или настраиваемая строка формата даты.</param>
		/// <returns>
		/// Строковое представление даты в заданном формате, 
		/// или <see langword="null"/>, если конвертация исходного элемента не удалась.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetDateOnly(
			int index,
			string format)
		{
			return GetDateOnly(index)?.ToString(format);
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Возвращает значение времени по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <returns>
		/// Объект <see cref="TimeOnly"/>, если преобразование прошло успешно; 
		/// в противном случае — <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TimeOnly? GetTimeOnly(
			int index)
		{
			var value = Get(index);
			return value?.ToTimeOnly();
		}


		/// <summary>
		/// Возвращает значение времени по указанному индексу или заданное значение по умолчанию.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="defaultValue">Значение, возвращаемое по умолчанию в случае ошибки парсинга.</param>
		/// <returns>
		/// Преобразованное значение времени, либо <paramref name="defaultValue"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TimeOnly GetTimeOnly(
			int index,
			TimeOnly defaultValue)
		{
			return GetTimeOnly(index) ?? defaultValue;
		}


		/// <summary>
		/// Возвращает отформатированную строку времени по указанному индексу.
		/// </summary>
		/// <param name="index">Индекс элемента (начиная с 0).</param>
		/// <param name="format">Стандартная или настраиваемая строка формата времени.</param>
		/// <returns>
		/// Строковое представление времени в заданном формате, 
		/// или <see langword="null"/>, если конвертация исходного элемента не удалась.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetTimeOnly(
			int index,
			string format)
		{
			return GetTimeOnly(index)?.ToString(format);
		}

	}

}
