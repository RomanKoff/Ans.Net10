// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class SuppDateTime
	{

		/// <summary>
		/// Возвращает ленивую коллекцию дней от указанной начальной до конечной даты включительно.
		/// </summary>
		/// <param name="start">Начальная дата диапазона (время отсекается до 00:00:00).</param>
		/// <param name="end">Конечная дата диапазона (время отсекается до 00:00:00).</param>
		/// <param name="useMod">Флаг, разрешающий автоматический разворот границ диапазона, если начальная дата <paramref name="start"/> хронологически позже конечной <paramref name="end"/>.</param>
		/// <returns>Последовательность календарных дней в виде объектов <see cref="DateTime"/>.</returns>
		public static IEnumerable<DateTime> GetDays(
			DateTime start,
			DateTime end,
			bool useMod)
		{
			var s1 = start.Date;
			var e1 = end.Date;
			if (s1 > e1)
			{
				if (useMod)
					(s1, e1) = (e1, s1);
				else
					yield break;
			}
			for (var d1 = s1; d1 <= e1; d1 = d1.AddDays(1))
				yield return d1;
		}


		/// <summary>
		/// Возвращает ленивую коллекцию дней от начальной до конечной даты включительно на основе типа <see cref="DateOnly"/>.
		/// </summary>
		/// <param name="start">Начальная календарная дата диапазона.</param>
		/// <param name="end">Конечная календарная дата диапазона.</param>
		/// <param name="useMod">Флаг, разрешающий автоматический разворот границ диапазона, если начальная дата <paramref name="start"/> хронологически позже конечной <paramref name="end"/>.</param>
		/// <returns>Последовательность календарных дней в виде объектов <see cref="DateTime"/> со временем на 00:00:00.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<DateTime> GetDays(
			DateOnly start,
			DateOnly end,
			bool useMod)
		{
			return GetDays(
				start.GetDateTime(), end.GetDateTime(), useMod);
		}

	}

}
