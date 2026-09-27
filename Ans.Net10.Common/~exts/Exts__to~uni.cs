// rev 2026-09-26

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class Exts__to
	{

		// --- Целочисленные типы (int, uint, long)


		/// <summary>
		/// Преобразует строковое значение в nullable-версию 32-битного целого числа со знаком.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="int"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int? ToInt(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<int>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в 32-битное целое число со знаком. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="int"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt(
			this string? value,
			int defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в nullable-версию 32-битного целого числа без знака.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="uint"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint? ToUInt(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<uint>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в 32-битное целое число без знака. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="uint"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ToUInt(
			this string? value,
			uint defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в nullable-версию 64-битного целого числа со знаком.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="long"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long? ToLong(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<long>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в 64-битное целое число со знаком. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="long"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToLong(
			this string? value,
			long defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		// --- Типы с плавающей запятой и фиксированной точностью (double, float, decimal)


		/// <summary>
		/// Преобразует строковое значение в nullable-версию числа с плавающей запятой двойной точности.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="double"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double? ToDouble(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<double>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в число с плавающей запятой двойной точности. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="double"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(
			this string? value,
			double defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в nullable-версию числа с плавающей запятой одинарной точности.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="float"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float? ToFloat(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<float>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в число с плавающей запятой одинарной точности. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="float"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ToFloat(
			this string? value,
			float defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в nullable-версию десятичного числа с фиксированной точностью.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="decimal"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal? ToDecimal(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<decimal>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое значение в десятичное число с фиксированной точностью. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Число типа <see cref="decimal"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal ToDecimal(
			this string? value,
			decimal defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		// --- Дата и время (DateTime, DateOnly, TimeOnly)


		/// <summary>
		/// Преобразует строковое представление даты и времени в nullable-версию объекта <see cref="DateTime"/>.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Объект <see cref="DateTime"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime? ToDateTime(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<DateTime>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое представление даты и времени в объект <see cref="DateTime"/>. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Объект <see cref="DateTime"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ToDateTime(
			this string? value,
			DateTime defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое представление даты в nullable-версию календарного объекта <see cref="DateOnly"/>.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Объект <see cref="DateOnly"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly? ToDateOnly(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<DateOnly>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое представление даты в календарный объект <see cref="DateOnly"/>. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Объект <see cref="DateOnly"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly ToDateOnly(
			this string? value,
			DateOnly defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое представление времени в nullable-версию объекта <see cref="TimeOnly"/>.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Объект <see cref="TimeOnly"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly? ToTimeOnly(
			this string? value,
			IFormatProvider? provider = null)
			=> _parse<TimeOnly>(value, provider ?? CultureInfo.InvariantCulture);


		/// <summary>
		/// Преобразует строковое представление времени в объект <see cref="TimeOnly"/>. В случае ошибки возвращает указанное дефолтное значение.
		/// </summary>
		/// <param name="value">Исходная строка для парсинга. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение на случай ошибки парсинга.</param>
		/// <param name="provider">Поставщик региональных настроек. Если равен <see langword="null"/>, применяется <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Объект <see cref="TimeOnly"/> или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly ToTimeOnly(
			this string? value,
			TimeOnly defaultValue,
			IFormatProvider? provider = null)
			=> _parse(value, defaultValue, provider ?? CultureInfo.InvariantCulture);



		// --- Логический тип (bool)


		/// <summary>
		/// Преобразует строковое выражение в логическое значение. 
		/// Возвращает <see langword="true"/>, если строка эквивалентна маркерам <c>"1"</c>, <c>"+"</c> или <c>"true"</c> (регистронезависимо).
		/// </summary>
		/// <param name="value">Исходная строка для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Логическое значение <see cref="bool"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBool(
			this string? value)
		{
			if (value == null)
				return false;
			var span1 = value.AsSpan().Trim();
			if (span1.IsEmpty)
				return false;
			if (span1.Equals("1", StringComparison.Ordinal))
				return true;
			if (span1.Equals("+", StringComparison.Ordinal))
				return true;
			return span1.Equals("true", StringComparison.OrdinalIgnoreCase);
		}


		/* privates */


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T? _parse<T>(
			string? value,
			IFormatProvider provider)
			where T : struct, ISpanParsable<T>
		{
			if (value == null)
				return null;
			var span1 = value.AsSpan().Trim();
			if (span1.IsEmpty)
				return null;
			return T.TryParse(span1, provider, out var result1)
				? result1 : null;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T _parse<T>(
			string? value,
			T defaultValue,
			IFormatProvider provider)
			where T : struct, ISpanParsable<T>
		{
			return _parse<T>(value, provider) ?? defaultValue;
		}

	}

}
