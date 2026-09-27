// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для типов <see cref="DateTime"/>, <see cref="DateOnly"/> и <see cref="TimeSpan"/>.
	/// </summary>
	public static partial class Exts__datetime
	{

		/*--- DateTime ---*/


		/// <summary>
		/// Преобразует текущий объект <see cref="DateTime"/> в объект <see cref="TimeOnly"/>.
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты и времени.</param>
		/// <returns>Объект <see cref="TimeOnly"/>, представляющий временную составляющую указанного объекта.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly GetTimeOnly(
			this DateTime instance)
		{
			return TimeOnly.FromDateTime(instance);
		}


		/// <summary>
		/// Преобразует текущий объект <see cref="DateTime"/> в объект <see cref="DateOnly"/>.
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты и времени.</param>
		/// <returns>Объект <see cref="DateOnly"/>, представляющий календарную дату указанного объекта.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly GetDateOnly(
			this DateTime instance)
		{
			return DateOnly.FromDateTime(instance);
		}


		/// <summary>
		/// Определяет временную эпоху для указанной даты относительно сегодняшнего дня.
		/// </summary>
		/// <param name="instance">Исходный экземпляр проверяемой даты.</param>
		/// <returns>Значение из перечисления <see cref="TensesEnum"/> (прошлое, настоящее или будущее).</returns>
		public static TensesEnum GetTenses(
			this DateTime instance)
		{
			var today1 = DateTime.Now.Date;
			var date1 = instance.Date;
			if (date1 < today1)
				return TensesEnum.Past;
			return date1 > today1
				? TensesEnum.Future : TensesEnum.Present;
		}


		/// <summary>
		/// Возвращает дату начала недели для указанной даты с учетом заданного дня начала недели.
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты.</param>
		/// <param name="startOfWeek">День недели, который считается её началом (например, <see cref="DayOfWeek.Monday"/>).</param>
		/// <returns>Новый экземпляр <see cref="DateTime"/>, соответствующий началу недели (на 00:00:00) для указанной даты.</returns>
		public static DateTime GetStartOfWeek(
			this DateTime instance,
			DayOfWeek startOfWeek)
		{
			int diff1 = (7 + (instance.DayOfWeek - startOfWeek)) % 7;
			return instance.AddDays(-1 * diff1).Date;
		}


		/// <summary>
		/// Возвращает дату начала календарного месяца (1-е число) для указанной даты.
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты.</param>
		/// <returns>Новый экземпляр <see cref="DateTime"/>, установленный на первое число текущего месяца и года.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetStartOfMonth(
			this DateTime instance)
		{
			return new DateTime(instance.Year, instance.Month, 1);
		}


		/// <summary>
		/// Проверяет равенство двух дат с точностью до указанного количества минут.
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты и времени для сравнения.</param>
		/// <param name="value2">Целевой экземпляр даты и времени, с которым производится сверка.</param>
		/// <param name="minutesDiff">Максимально допустимая разница в минутах включительно.</param>
		/// <returns><see langword="true"/>, если абсолютная разница между датами не превышает <paramref name="minutesDiff"/> минут; иначе — <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEqual(
			this DateTime instance,
			DateTime value2,
			int minutesDiff)
		{
			return Math.Abs((instance - value2).TotalMinutes) <= minutesDiff;
		}


		/// <summary>
		/// Проверяет, содержит ли текущий объект <see cref="DateTime"/> время суток (отличное от полуночи 00:00:00).
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты и времени.</param>
		/// <returns><see langword="true"/>, если у объекта установлено время, отличное от <see cref="TimeSpan.Zero"/>; иначе — <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasTimeOfDay(
			this DateTime instance)
		{
			return instance.TimeOfDay != TimeSpan.Zero;
		}


		/// <summary>
		/// Возвращает строковое представление даты в специализированном внутреннем формате "yyyy-'0'MM-dd".
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты.</param>
		/// <returns>Строка, отформатированная по правилам специализированного формата AnsDate.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetAnsDate(
			this DateTime instance)
		{
			return instance.ToString(SuppDateTime.ANS_DATE_FORMAT);
		}


		/// <summary>
		/// Возвращает строковое представление даты и времени в специализированном внутреннем формате "yyyy-'0'MM-dd HH:mm:ss".
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты и времени.</param>
		/// <returns>Строка, отформатированная по правилам специализированного формата AnsDateTime.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetAnsDateTime(
			this DateTime instance)
		{
			return instance.ToString(SuppDateTime.ANS_DATETIME_FORMAT);
		}


		/// <summary>
		/// Возвращает строковое представление даты и времени, адаптированное и безопасное для использования в именах файлов.
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты и времени.</param>
		/// <returns>Безопасная строка даты и времени, не содержащая запрещенных символов путей файловой системы.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetAnsDateTimeForFile(
			this DateTime instance)
		{
			return instance.ToString(SuppDateTime.ANS_DATETIME_FILE_FORMAT);
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Преобразует объект <see cref="DateOnly"/> в тип <see cref="DateTime"/> на начало суток (00:00:00).
		/// </summary>
		/// <param name="instance">Исходный экземпляр даты.</param>
		/// <returns>Объект <see cref="DateTime"/>, соответствующий указанной дате со временем, установленным на полночь.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTime(
			this DateOnly instance)
		{
			return instance.ToDateTime(TimeOnly.MinValue);
		}


		/// <summary>
		/// Определяет временную эпоху для объекта <see cref="DateOnly"/> относительно сегодняшнего дня.
		/// </summary>
		/// <param name="instance">Исходный экземпляр календарной даты.</param>
		/// <returns>Значение из перечисления <see cref="TensesEnum"/> (прошлое, настоящее или будущее).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TensesEnum GetTenses(
			this DateOnly instance)
		{
			return instance.GetDateTime().GetTenses();
		}


		/*--- TimeSpan ---*/


		/// <summary>
		/// Возвращает абсолютное общее количество минут во временном интервале, округленное до ближайшего целого числа.
		/// </summary>
		/// <param name="instance">Исходный временной интервал.</param>
		/// <returns>Округленное до целого значения количество минут, содержащихся в интервале.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetTotalMinutes(
			this TimeSpan instance)
		{
			return SuppMath.RoundToInt(Math.Abs(instance.TotalMinutes));
		}

	}

}
