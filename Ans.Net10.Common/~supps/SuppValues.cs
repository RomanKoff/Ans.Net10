// rev 2026-09-26

using System.Buffers;
using System.Numerics;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечисление вариантов биологического пола человека.
	/// </summary>
	public enum GenderEnum
		: int
	{
		/// <summary>
		/// Биологический пол не указан или неизвестен.
		/// </summary>
		NotSpecified = 0,

		/// <summary>
		/// Мужской биологический пол.
		/// </summary>
		Male = 1,

		/// <summary>
		/// Женский биологический пол.
		/// </summary>
		Female = 2
	}



	/// <summary>
	/// Вспомогательный класс для валидации, подстановки дефолтных параметров 
	/// и специализированного форматирования входящих значений переменных.
	/// </summary>
	public static class SuppValues
	{

		private static readonly SearchValues<char> _digitsValues = SearchValues.Create("0123456789");


		/* functions */


		/// <summary>
		/// Возвращает исходную строку, если она не пустая; в противном случае возвращает первое непустом значение из списка альтернатив.
		/// </summary>
		/// <param name="current">Проверяемая строковая переменная.</param>
		/// <param name="defaultValues">Набор альтернативных значений по умолчанию, передаваемый без аллокаций в куче через <see cref="ReadOnlySpan{T}"/>.</param>
		/// <returns>Первая непустая строка из набора или <see langword="null"/>, если все значения оказались пустыми.</returns>
		public static string Default(
			string current,
			params ReadOnlySpan<string?> defaultValues)
		{
			if (!string.IsNullOrEmpty(current))
				return current;
			foreach (var value1 in defaultValues)
				if (!string.IsNullOrEmpty(value1))
					return value1;
			return null!;
		}


		/// <summary>
		/// Возвращает значение по умолчанию, если текущее целое число совпадает со значением-маркером отсутствия данных.
		/// </summary>
		/// <param name="current">Текущее проверяемое числовое значение.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение по умолчанию.</param>
		/// <param name="nullValue">Значение-маркер, интерпретируемое как отсутствие данных. По умолчанию равно <c>0</c>.</param>
		/// <returns>Исходное число или альтернативное значение <paramref name="defaultValue"/>.</returns>
		public static int Default(
			int current,
			int defaultValue,
			int nullValue = 0)
		{
			return (current == nullValue)
				? defaultValue : current;
		}


		/// <summary>
		/// Проверяет, является ли хотя бы один из переданных объектов непустым (не равен <see langword="null"/> и не содержит пустую строку) без выделения памяти в куче.
		/// </summary>
		/// <param name="values">Высокопроизводительный фиксированный набор проверяемых объектов произвольного типа.</param>
		/// <returns><see langword="true"/>, если в наборе найден хотя бы один заполненный и валидный объект; в противном случае — <see langword="false"/>.</returns>
		public static bool HasAny(
			params ReadOnlySpan<object?> values)
		{
			foreach (var value1 in values)
			{
				if (value1 == null)
					continue;
				if (value1 is string str1)
				{
					if (!string.IsNullOrEmpty(str1))
						return true;
				}
				else if (value1 is IFormattable
					|| value1.GetType().IsPrimitive)
					return true;
				else if (!string.IsNullOrEmpty(value1.ToString()))
					return true;
			}
			return false;
		}


		/// <summary>
		/// Проверяет, что абсолютно все переданные объекты в наборе являются непустыми (не равны <see langword="null"/> и не содержат пустых строк).
		/// </summary>
		/// <param name="values">Высокопроизводительный фиксированный набор проверяемых объектов произвольного типа.</param>
		/// <returns><see langword="true"/>, если все объекты в наборе гарантированно заполнены; в противном случае — <see langword="false"/>.</returns>
		public static bool HasAll(
			params ReadOnlySpan<object> values)
		{
			foreach (var value1 in values)
			{
				if (value1 == null)
					return false;
				if (value1 is string str1)
				{
					if (string.IsNullOrEmpty(str1))
						return false;
				}
				else if (value1 is IFormattable
					|| value1.GetType().IsPrimitive)
					continue;
				else if (string.IsNullOrEmpty(value1.ToString()))
					return false;
			}
			return true;
		}


		/// <summary>
		/// Возвращает максимальное значение из двух на основе интерфейса <see cref="IComparable{T}"/>.
		/// </summary>
		/// <typeparam name="T">Тип структуры, поддерживающий правила сравнения.</typeparam>
		/// <param name="value1">Первое сравниваемое значение.</param>
		/// <param name="value2">Второе сравниваемое значение, завернутое в nullable-контейнер.</param>
		/// <returns>Наибольшее из двух значений, либо <paramref name="value1"/>, если параметр <paramref name="value2"/> не имеет значения.</returns>
		public static T MaxValue<T>(
			T value1,
			T? value2)
			where T : struct, IComparable<T>
		{
			if (!value2.HasValue)
				return value1;
			T v2 = value2.Value;
			return value1.CompareTo(v2) > 0
				? value1 : v2;
		}


		/// <summary>
		/// Возвращает минимальное значение из двух на основе интерфейса <see cref="IComparable{T}"/>.
		/// </summary>
		/// <typeparam name="T">Тип структуры, поддерживающий правила сравнения.</typeparam>
		/// <param name="value1">Первое сравниваемое значение.</param>
		/// <param name="value2">Второе сравниваемое значение, завернутое в nullable-контейнер.</param>
		/// <returns>Наименьшее из двух значений, либо <paramref name="value1"/>, если параметр <paramref name="value2"/> не имеет значения.</returns>
		public static T MinValue<T>(
			T value1,
			T? value2)
			where T : struct, IComparable<T>
		{
			if (!value2.HasValue)
				return value1;
			T v2 = value2.Value;
			return value1.CompareTo(v2) < 0
				? value1 : v2;
		}


		/// <summary>
		/// Возвращает максимальное числовое значение из двух с использованием статических интерфейсов обобщенной математики .NET.
		/// </summary>
		/// <typeparam name="T">Тип числа, реализующий интерфейс <see cref="INumber{T}"/>.</typeparam>
		/// <param name="value1">Первое сравниваемое число.</param>
		/// <param name="value2">Второе сравниваемое число, завернутое в nullable-контейнер.</param>
		/// <returns>Наибольшее из двух чисел, вычисленное без аллокаций.</returns>
		public static T MaxNum<T>(
			T value1,
			T? value2)
			where T : struct, INumber<T>
		{
			return value2.HasValue
				? T.Max(value1, value2.Value)
				: value1;
		}


		/// <summary>
		/// Возвращает минимальное числовое значение из двух с использованием статических интерфейсов обобщенной математики .NET.
		/// </summary>
		/// <typeparam name="T">Тип числа, реализующий интерфейс <see cref="INumber{T}"/>.</typeparam>
		/// <param name="value1">Первое сравниваемое число.</param>
		/// <param name="value2">Второе сравниваемое число, завернутое в nullable-контейнер.</param>
		/// <returns>Наименьшее из двух чисел, вычисленное без аллокаций.</returns>
		public static T MinNum<T>(
			T value1,
			T? value2)
			where T : struct, INumber<T>
		{
			return value2.HasValue
				? T.Min(value1, value2.Value)
				: value1;
		}


		/// <summary>
		/// Преобразует базовые системные типы данных (.NET структуры дат, времени и логики) в их строковые фиксированные веб-эквиваленты.
		/// </summary>
		/// <param name="value">Объект для веб-сериализации.</param>
		/// <returns>Строковое нормализованное веб-представление объекта, либо <see cref="string.Empty"/>, если объект равен <see langword="null"/>.</returns>
		public static string GetStringForWeb(
			object? value)
		{
			if (value == null)
				return string.Empty;
			return value switch
			{
				DateTime dt1 => dt1.ToString("u"),
				DateOnly do1 => do1.ToString("yyyy-MM-dd"),
				TimeOnly to1 => to1.ToString("HH\\:mm\\:ss.fff"),
				bool b1 => b1.Make("true", "false"),
				_ => value.ToString() ?? string.Empty
			};
		}


		/// <summary>
		/// Выполняет высокопроизводительное извлечение исключительно цифровых символов из строки.
		/// </summary>
		/// <param name="number">Исходная строка для очистки.</param>
		/// <returns>Строка, содержащая только цифры. Если входная строка пуста, возвращается пустая строка.</returns>
		public static string GetDigitsOnly(
			string? number)
		{
			if (string.IsNullOrEmpty(number))
				return string.Empty;
			var span1 = number.AsSpan();
			int firstMatch1 = span1.IndexOfAny(_digitsValues);
			if (firstMatch1 < 0)
				return string.Empty;
			int lastMatch1 = span1.LastIndexOfAny(_digitsValues);
			if (firstMatch1 == 0
				&& lastMatch1 == span1.Length - 1
				&& _digitsCount(span1) == span1.Length)
				return number;
			var sb1 = new StringBuilder(span1.Length);
			for (int i1 = 0; i1 < span1.Length; i1++)
				if (_digitsValues.Contains(span1[i1]))
					sb1.Append(span1[i1]);
			return sb1.ToString();
		}


		/// <summary>
		/// Выполняет интеллектуальный разбор, нормализацию и форматирование сырой записи телефонного номера 
		/// с учетом добавочных кодов и строго валидированного цифрового регионального кода по умолчанию.
		/// </summary>
		/// <param name="rawPhone">Сырая текстовая запись телефонного номера (допускает наличие пробелов, дефисов, скобок и null).</param>
		/// <param name="regionCode">Строго цифровой код региона по умолчанию (например, "7812"). Не должен содержать префиксов или мусорных символов.</param>
		/// <returns>
		/// Именованный кортеж, содержащий:
		/// <list type="bullet">
		/// <item><description><c>Formatted</c> — отформатированная строка для красивого визуального отображения.</description></item>
		/// <item><description><c>Url</c> — очищенная строка для использования в HTML-ссылках <c>href="tel:..."</c>, либо <see langword="null"/>, если ссылка не требуется.</description></item>
		/// </list>
		/// </returns>
		public static (string Formatted, string? Url) ParsePhoneNumber(
			string? rawPhone,
			string regionCode)
		{
			if (string.IsNullOrWhiteSpace(rawPhone))
				return (string.Empty, null);
			string mainPart1 = rawPhone;
			string extensionPart1 = string.Empty;
			int hashIndex1 = rawPhone.IndexOf('#');
			if (hashIndex1 >= 0)
			{
				mainPart1 = rawPhone[..hashIndex1];
				extensionPart1 = GetDigitsOnly(rawPhone[hashIndex1..]);
			}
			bool hasLeadingPlus1 = mainPart1.TrimStart().StartsWith('+');
			string digits1 = GetDigitsOnly(mainPart1);
			if (!hasLeadingPlus1 && digits1.StartsWith('8') && digits1.Length == 11)
				digits1 = "7" + digits1[1..];
			if (digits1.Length > 11)
				return (string.Empty, null);
			if (digits1.Length < 7)
			{
				string formattedShort1 = string.Format(
					Resources.Common.Template_PhoneInternal, digits1);
				return (formattedShort1, null);
			}
			if (digits1.Length == 7)
				digits1 = regionCode + digits1;
			else if (digits1.Length == 10)
				digits1 = '7' + digits1;
			var formattedBuilder1 = FormatPhoneNumber(digits1);
			var urlBuilder1 = $"+{digits1}";
			if (string.IsNullOrEmpty(extensionPart1))
				return (formattedBuilder1, urlBuilder1);
			return (
				string.Format(
					Resources.Common.Template_PhonePostfix,
					formattedBuilder1,
					extensionPart1),
				$"{urlBuilder1}pp{extensionPart1}");
		}


		/// <summary>
		/// Преобразует строку из 11 чистых цифр номера телефона в канонический международный формат отображения.
		/// </summary>
		/// <param name="digits11">Строка, содержащая ровно 11 цифровых символов (например, "78121234567").</param>
		/// <returns>Отформатированная строка телефонного номера вида <c>+X-XXX-XXX-XX-XX</c> (например, "+7-812-123-45-67").</returns>
		/// <exception cref="ArgumentException">
		/// Вызывается, если длина переданной строки <paramref name="digits11"/> не равна 11 символам.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="digits11"/> равен <see langword="null"/>.
		/// </exception>		
		public static string FormatPhoneNumber(
			string digits11)
		{
			ArgumentNullException.ThrowIfNull(digits11);
			if (digits11.Length != 11)
			{
				throw new ArgumentException(
					"[Ans.Net10.Common] Номер телефона для форматирования должен состоять строго из 11 цифр.",
					nameof(digits11));
			}
			return string.Create(16, digits11, (span1, state1) =>
			{
				var src1 = state1.AsSpan();
				span1[0] = '+';
				span1[1] = src1[0];
				span1[2] = '-';
				src1[1..4].CopyTo(span1[3..6]);
				span1[6] = '-';
				src1[4..7].CopyTo(span1[7..10]);
				span1[10] = '-';
				src1[7..9].CopyTo(span1[11..13]);
				span1[13] = '-';
				src1[9..11].CopyTo(span1[14..16]);
			});
		}


		/// <summary>
		/// Форматирует высокоточное десятичное число в локализованную денежную строку с двумя знаками после запятой.
		/// </summary>
		/// <param name="amount">Исходная денежная сумма типа <see cref="decimal"/>.</param>
		/// <returns>Строка отформатированной валюты.</returns>
		public static string GetCurrencyLoc(
			decimal amount)
		{
			return string.Format("{0:N2}", amount);
		}


		/// <summary>
		/// Форматирует высокоточное десятичное число типа <see cref="decimal"/> в строковый бухгалтерский вид с разделителем рублей и копеек через знак равенства.
		/// </summary>
		/// <param name="amount">Исходная сумма для разделения.</param>
		/// <returns>Строка бухгалтерского формата вида "Рубли=Копейки".</returns>
		public static string GetCurrencyBuh(
			decimal amount)
		{
			long rub1 = (long)amount;
			long kop1 = Math.Abs((long)Math.Round(amount * 100)) % 100;
			return string.Format("{0}={1:00}", rub1, kop1);
		}


		/* privates */


		private static int _digitsCount(
			ReadOnlySpan<char> span)
		{
			int count1 = 0;
			foreach (char ch1 in span)
				if (_digitsValues.Contains(ch1))
					count1++;
			return count1;
		}

	}

}
