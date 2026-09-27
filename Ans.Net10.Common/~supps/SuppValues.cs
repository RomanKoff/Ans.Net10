// rev 2026-09-26

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень вариантов биологического пола человека.
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			object value)
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
		/// Извлекает из входящей строки исключительно цифровые символы, полностью удаляя любые буквы, пробелы и знаки препинания.
		/// </summary>
		/// <param name="number">Входящая алфавитно-цифровая строка.</param>
		/// <returns>Строка, состоящая только из последовательности цифр, либо <see cref="string.Empty"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetDigitalOnly(
			string number)
		{
			if (string.IsNullOrEmpty(number))
				return string.Empty;
			return _Consts.G_REGEX_NOT_NUMBER().Replace(number, "");
		}


		/// <summary>
		/// Форматирует высокоточное десятичное число в локализованную денежную строку с двумя знаками после запятой.
		/// </summary>
		/// <param name="amount">Исходная денежная сумма типа <see cref="decimal"/>.</param>
		/// <returns>Строка отформатированной валюты.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

	}

}
