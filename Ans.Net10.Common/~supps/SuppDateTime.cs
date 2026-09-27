// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень временных эпох (прошлое, настоящее, будущее).
	/// </summary>
	public enum TensesEnum
	{
		/// <summary>
		/// Событие уже произошло (прошлое).
		/// </summary>
		Past,

		/// <summary>
		/// Событие происходит в текущий момент времени (настоящее).
		/// </summary>
		Present,

		/// <summary>
		/// Событие произойдет в будущем.
		/// </summary>
		Future
	}



	/// <summary>
	/// Вспомогательный класс для работы с датами, календарными вычислениями и встроенными строковыми форматами.
	/// </summary>
	public static partial class SuppDateTime
	{

		/* consts */


		/// <summary>
		/// Словарь специализированных шаблонов форматирования стандарта RFC 3339 для различных представлений времени.
		/// </summary>
		/// <value>Словарь, сопоставляющий текстовые ключи (date, datetime, datetime-local, time) с масками вывода.</value>
		public static readonly Dictionary<string, string> RTC3339FORMATS
			= new(StringComparer.Ordinal) {
				{ "date", "{0:yyyy-MM-dd}" },
				{ "datetime", @"{0:yyyy-MM-ddTHH\:mm\:ss.fffK}" },
				{ "datetime-local", @"{0:yyyy-MM-ddTHH\:mm\:ss.fff}" },
				{ "time", @"{0:HH\:mm\:ss.fff}" },
			};


		/// <summary>
		/// Специализированный строковый формат даты и времени библиотеки: "yyyy-'0'MM-dd HH:mm:ss".
		/// </summary>
		public const string ANS_DATETIME_FORMAT = "yyyy-'0'MM-dd HH:mm:ss";

		/// <summary>
		/// Специализированный строковый формат календарной даты библиотеки: "yyyy-'0'MM-dd".
		/// </summary>
		public const string ANS_DATE_FORMAT = "yyyy-'0'MM-dd";

		/// <summary>
		/// Безопасный строковый формат даты и времени для использования в именах файлов: "yyyy-'0'MM-dd_HH-mm-ss".
		/// </summary>
		public const string ANS_DATETIME_FILE_FORMAT = "yyyy-'0'MM-dd_HH-mm-ss";


		/* functions */


		/// <summary>
		/// Получает текущую локальную системную дату и время.
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий текущему моменту времени на сервере/компьютере.</value>
		public static DateTime Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => DateTime.Now;
		}


		/// <summary>
		/// Получает дату текущего календарного дня со временем, установленным на полночь (00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, представляющий сегодняшний день.</value>
		public static DateTime Today
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current.Date;
		}


		/// <summary>
		/// Получает дату начала текущего календарного года (1 января, 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/> начала текущего года.</value>
		public static DateTime CurrentYearBegin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new(Current.Year, 1, 1);
		}


		/// <summary>
		/// Получает дату начала следующего календарного года (1 января следующего года, 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/> начала следующего года.</value>
		public static DateTime NextYearBegin
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => CurrentYearBegin.AddYears(1);
		}


		/// <summary>
		/// Получает календарную дату вчерашнего дня (00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий вчерашнему числу.</value>
		public static DateTime Yesterday
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Today.AddDays(-1);
		}


		/// <summary>
		/// Получает календарную дату завтрашнего дня (00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий завтрашнему числу.</value>
		public static DateTime Tomorrow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Today.AddDays(1);
		}


		/// <summary>
		/// Получает календарную дату послезавтрашнего дня (00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий дню после завтрашнего.</value>
		public static DateTime TomorrowAfter
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Today.AddDays(2);
		}

	}

}
