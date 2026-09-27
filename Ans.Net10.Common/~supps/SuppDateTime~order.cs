// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class SuppDateTime
	{

		/// <summary>
		/// Возвращает наибольшее значение из двух дат.
		/// </summary>
		/// <param name="value1">Первое базовое значение даты и времени для сравнения.</param>
		/// <param name="value2">Второе значение для сравнения. Допускает значение <see langword="null"/>.</param>
		/// <returns>Максимальная из двух дат. Если <paramref name="value2"/> равен <see langword="null"/>, возвращается <paramref name="value1"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Max(
			DateTime value1,
			DateTime? value2)
		{
			return SuppValues.MaxValue(value1, value2);
		}


		/// <summary>
		/// Возвращает наибольшее значение из двух календарных дат типа <see cref="DateOnly"/>.
		/// </summary>
		/// <param name="value1">Первое базовое значение календарной даты для сравнения.</param>
		/// <param name="value2">Второе значение для сравнения. Допускает значение <see langword="null"/>.</param>
		/// <returns>Максимальная календарная дата. Если <paramref name="value2"/> равен <see langword="null"/>, возвращается <paramref name="value1"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly Max(
			DateOnly value1,
			DateOnly? value2)
		{
			return SuppValues.MaxValue(value1, value2);
		}


		/// <summary>
		/// Возвращает наибольшее значение из двух временных меток типа <see cref="TimeOnly"/>.
		/// </summary>
		/// <param name="value1">Первое базовое значение времени суток для сравнения.</param>
		/// <param name="value2">Второе значение для сравнения. Допускает значение <see langword="null"/>.</param>
		/// <returns>Максимальное время суток. Если <paramref name="value2"/> равен <see langword="null"/>, возвращается <paramref name="value1"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly Max(
			TimeOnly value1,
			TimeOnly? value2)
		{
			return SuppValues.MaxValue(value1, value2);
		}


		/// <summary>
		/// Возвращает наименьшее значение из двух дат.
		/// </summary>
		/// <param name="value1">Первое базовое значение даты и времени для сравнения.</param>
		/// <param name="value2">Второе значение для сравнения. Допускает значение <see langword="null"/>.</param>
		/// <returns>Минимальная из двух дат. Если <paramref name="value2"/> равен <see langword="null"/>, возвращается <paramref name="value1"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Min(
			DateTime value1,
			DateTime? value2)
		{
			return SuppValues.MinValue(value1, value2);
		}


		/// <summary>
		/// Возвращает наименьшее значение из двух календарных дат типа <see cref="DateOnly"/>.
		/// </summary>
		/// <param name="value1">Первое базовое значение календарной даты для сравнения.</param>
		/// <param name="value2">Второе значение для сравнения. Допускает значение <see langword="null"/>.</param>
		/// <returns>Минимальная календарная дата. Если <paramref name="value2"/> равен <see langword="null"/>, возвращается <paramref name="value1"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly Min(
			DateOnly value1,
			DateOnly? value2)
		{
			return SuppValues.MinValue(value1, value2);
		}


		/// <summary>
		/// Возвращает наименьшее значение из двух временных меток типа <see cref="TimeOnly"/>.
		/// </summary>
		/// <param name="value1">Первое базовое значение времени суток для сравнения.</param>
		/// <param name="value2">Второе значение для сравнения. Допускает значение <see langword="null"/>.</param>
		/// <returns>Минимальное время суток. Если <paramref name="value2"/> равен <see langword="null"/>, возвращается <paramref name="value1"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly Min(
			TimeOnly value1,
			TimeOnly? value2)
		{
			return SuppValues.MinValue(value1, value2);
		}

	}

}
