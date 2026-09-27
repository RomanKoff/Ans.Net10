// rev 2026-09-26

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class SuppDateTime
	{

		/// <summary>
		/// Преобразует Unix-таймстамп (в секундах) в локальное системное время.
		/// </summary>
		/// <param name="value">Значение таймстампа в секундах.</param>
		/// <returns>Объект <see cref="DateTime"/> в локальном часовом поясе.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTimeFromUnixTimeStamp(
			double value)
		{
			return DateTimeOffset.FromUnixTimeSeconds((long)value).DateTime.ToLocalTime();
		}


		/// <summary>
		/// Преобразует nullable Unix-таймстамп (в секундах) в локальное системное время.
		/// </summary>
		/// <param name="value">Значение таймстампа в секундах или <see langword="null"/>.</param>
		/// <returns>Объект <see cref="DateTime"/> в локальном часовом поясе или <see langword="null"/>, если входное значение отсутствовало.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime? GetDateTimeFromUnixTimeStamp(
			double? value)
		{
			return value == null
				? null : GetDateTimeFromUnixTimeStamp(value.Value);
		}


		/// <summary>
		/// Преобразует Java/JavaScript-таймстамп (в миллисекундах) в локальное системное время.
		/// </summary>
		/// <param name="value">Значение таймстампа в миллисекундах.</param>
		/// <returns>Объект <see cref="DateTime"/> в локальном часовом поясе.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTimeFromJavaTimeStamp(
			double value)
		{
			return DateTimeOffset.FromUnixTimeMilliseconds((long)value).DateTime.ToLocalTime();
		}


		/// <summary>
		/// Преобразует nullable Java/JavaScript-таймстамп (в миллисекундах) в локальное системное время.
		/// </summary>
		/// <param name="value">Значение таймстампа в миллисекундах или <see langword="null"/>.</param>
		/// <returns>Объект <see cref="DateTime"/> в локальном часовом поясе или <see langword="null"/>, если входное значение отсутствовало.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime? GetDateTimeFromJavaTimeStamp(
			double? value)
		{
			return value == null
				? null : GetDateTimeFromJavaTimeStamp(value.Value);
		}


		/// <summary>
		/// Возвращает дату из короткой строки в стандартном универсальном формате "yyyy-MM-dd".
		/// </summary>
		/// <param name="value">Строка с датой универсального формата.</param>
		/// <returns>Объект <see cref="DateTime"/> или <see langword="null"/>, если строка пуста, имеет некорректную длину или не соответствует формату.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime? GetDateFromUniDate(
			string value)
		{
			if (string.IsNullOrWhiteSpace(value) || value.Length != 10)
				return null;
			return DateTime.TryParseExact(
				value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
				? parsedDate : null;
		}


		/// <summary>
		/// Возвращает дату из короткой строки в специализированном внутреннем формате "yyyy-'0'MM-dd".
		/// </summary>
		/// <param name="value">Строка с датой формата AnsDate.</param>
		/// <returns>Объект <see cref="DateTime"/> или <see langword="null"/>, если строка пуста, имеет некорректную длину или не соответствует формату <see cref="ANS_DATE_FORMAT"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime? GetDateFromAnsDate(
			string value)
		{
			if (string.IsNullOrWhiteSpace(value) || value.Length != 11)
				return null;
			return DateTime.TryParseExact(
				value, ANS_DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
				? parsedDate : null;
		}

	}

}
