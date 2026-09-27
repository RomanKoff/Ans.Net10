// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Предоставляет методы для формирования человекочитаемых текстовых представлений дат и времени 
	/// на основе фиксированного снимка времени (Snapshot), создаваемого в момент инициализации экземпляра.
	/// </summary>
	public class DateTimeHelper
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DateTimeHelper"/>, фиксируя текущие дату и время.
		/// </summary>
		/// <remarks>
		/// Базируется на значении <see cref="SuppDateTime.Current"/>. Все внутренние расчетные свойства 
		/// (такие как <see cref="Today"/>, <see cref="Yesterday"/>, <see cref="Tomorrow"/>) вычисляются единожды относительно этого снимка.
		/// </remarks>
		public DateTimeHelper()
		{
			Current = SuppDateTime.Current;
			CurrentYearBegin = new DateTime(Current.Year, 1, 1);
			NextYearBegin = CurrentYearBegin.AddYears(1);
			Today = Current.Date;
			Yesterday = Today.AddDays(-1);
			Tomorrow = Today.AddDays(1);
			TomorrowAfter = Today.AddDays(2);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает зафиксированные текущие дату и время на момент создания экземпляра класса.
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, представляющий точный снимок текущего времени.</value>
		public DateTime Current { get; }


		/// <summary>
		/// Возвращает дату начала текущего календарного года (1 января, 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий первому дню текущего года.</value>
		public DateTime CurrentYearBegin { get; }


		/// <summary>
		/// Возвращает дату начала следующего календарного года (1 января следующего года, 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий первому дню следующего года.</value>
		public DateTime NextYearBegin { get; }


		/// <summary>
		/// Возвращает текущую календарную дату на момент создания снимка, без учета времени (время установлено в 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, представляющий текущий день.</value>
		public DateTime Today { get; }


		/// <summary>
		/// Возвращает календарную дату вчерашнего дня (время установлено в 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий вчерашнему дню.</value>
		public DateTime Yesterday { get; }


		/// <summary>
		/// Возвращает календарную дату завтрашнего дня (время установлено in 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий завтрашнему дню.</value>
		public DateTime Tomorrow { get; }


		/// <summary>
		/// Возвращает календарную дату послезавтрашнего дня (время установлено в 00:00:00).
		/// </summary>
		/// <value>Объект <see cref="DateTime"/>, соответствующий дню после завтра.</value>
		public DateTime TomorrowAfter { get; }


		/* functions */


		/// <summary>
		/// Возвращает форматированную человекочитаемую строку с датой и временем события, адаптированную для вывода в публикациях и логах.
		/// </summary>
		/// <remarks>
		/// Логика форматирования автоматически выбирает один из 7 сценариев в зависимости от удаленности <paramref name="datetime"/> 
		/// от текущей даты (прошлые года, текущий год, вчера, сегодня, завтра, текущий год вперед, будущие года).
		/// </remarks>
		/// <param name="datetime">Объект <see cref="DateTime"/>, представляющий дату и время проверяемого события.</param>
		/// <param name="addTime">Признак необходимости добавления компонента времени к результирующей строке. Если у <paramref name="datetime"/> отсутствует время (равно 00:00:00), флаг автоматически сбрасывается.</param>
		/// <param name="useYesterdayTodayTomorrow">Признак использования словесных подстановок (например, "сегодня", "вчера", "завтра") вместо стандартного вывода дня и месяца.</param>
		/// <returns>Форматированная человекочитаемая строка, сформированная на основе шаблонов локализации из ресурсов.</returns>
		public string GetPassed(
			DateTime datetime,
			bool addTime,
			bool useYesterdayTodayTomorrow)
		{
			if (addTime && !datetime.HasTimeOfDay())
				addTime = false;

			// Сценарий 1: Далекое будущее (следующие года)
			if (datetime >= NextYearBegin)
				return datetime.ToString(addTime
					? Resources.Common.Format_DateTime_Full
					: Resources.Common.Format_Date_Full);

			// Сценарий 2: Будущее (этот год, начиная с ПОСЛЕЗАВТРА)
			if (datetime >= TomorrowAfter)
				return datetime.ToString(addTime
					? Resources.Common.Format_DateTime_DayAndMonth
					: Resources.Common.Format_Date_DayAndMonth);

			// Сценарий 3: ЗАВТРА
			if (datetime >= Tomorrow)
				return useYesterdayTodayTomorrow
					? datetime.ToString(addTime
						? Resources.Common.Format_DateTime_Tomorrow
						: Resources.Common.Format_Date_Tomorrow)
					: datetime.ToString(addTime
						? Resources.Common.Format_DateTime_DayAndMonth
						: Resources.Common.Format_Date_DayAndMonth);

			// Сценарий 4: СЕГОДНЯ
			if (datetime >= Today)
				return useYesterdayTodayTomorrow
					? datetime.ToString(addTime
						? Resources.Common.Format_DateTime_Today
						: Resources.Common.Format_Date_Today)
					: datetime.ToString(addTime
						? Resources.Common.Format_DateTime_DayAndMonth
						: Resources.Common.Format_Date_DayAndMonth);

			// Сценарий 5: ВЧЕРА
			if (datetime >= Yesterday)
				return useYesterdayTodayTomorrow
					? datetime.ToString(addTime
						? Resources.Common.Format_DateTime_Yesterday
						: Resources.Common.Format_Date_Yesterday)
					: datetime.ToString(addTime
						? Resources.Common.Format_DateTime_DayAndMonth
						: Resources.Common.Format_Date_DayAndMonth);

			// Сценарий 6: Недавнее прошлое (этот год, до ВЧЕРА)
			if (datetime >= CurrentYearBegin)
				return datetime.ToString(addTime
					? Resources.Common.Format_DateTime_DayAndMonth
					: Resources.Common.Format_Date_DayAndMonth);

			// Сценарий 7: Далекое прошлое (предыдущие года)
			return datetime.ToString(addTime
				? Resources.Common.Format_DateTime_Full
				: Resources.Common.Format_Date_Full);
		}


		/// <summary>
		/// Возвращает форматированную человекочитаемую строку с датой и временем события с поддержкой значений <see langword="null"/>.
		/// </summary>
		/// <param name="datetime">Объект <see cref="DateTime"/>, допускающий значение <see langword="null"/>.</param>
		/// <param name="addTime">Признак необходимости добавления компонента времени к результирующей строке.</param>
		/// <param name="useYesterdayTodayTomorrow">Признак использования словесных подстановок "сегодня", "вчера", "завтра".</param>
		/// <returns>
		/// Форматированная строка, сгенерированная методом <see cref="GetPassed(DateTime, bool, bool)"/>, 
		/// либо локализованный текст по умолчанию (например, "никогда"), если <paramref name="datetime"/> равен <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetPassed(
			DateTime? datetime,
			bool addTime,
			bool useYesterdayTodayTomorrow)
		{
			return datetime.HasValue
				? GetPassed(datetime.Value, addTime, useYesterdayTodayTomorrow)
				: Resources.Common.Text_Never;
		}


		/// <summary>
		/// Возвращает форматированную человекочитаемую строку для события, представленного только датой (<see cref="DateOnly"/>), без учета времени.
		/// </summary>
		/// <param name="date">Объект <see cref="DateOnly"/>, представляющий дату проверяемого события.</param>
		/// <param name="useYesterdayTodayTomorrow">Признак использования словесных подстановок "сегодня", "вчера", "завтра".</param>
		/// <returns>Форматированная человекочитаемая строка без временного компонента.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetPassed(
			DateOnly date,
			bool useYesterdayTodayTomorrow)
		{
			return GetPassed(date.GetDateTime(), false, useYesterdayTodayTomorrow);
		}


		/// <summary>
		/// Возвращает форматированную человекочитаемую строку для события, представленного только датой, с поддержкой значений <see langword="null"/>.
		/// </summary>
		/// <param name="date">Объект <see cref="DateOnly"/>, допускающий значение <see langword="null"/>.</param>
		/// <param name="useYesterdayTodayTomorrow">Признак использования словесных подстановок "сегодня", "вчера", "завтра".</param>
		/// <returns>
		/// Форматированная строка даты, либо локализованный текст по умолчанию (например, "никогда"), если <paramref name="date"/> равен <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetPassed(
			DateOnly? date,
			bool useYesterdayTodayTomorrow)
		{
			return date.HasValue
				? GetPassed(date.Value.GetDateTime(), false, useYesterdayTodayTomorrow)
				: Resources.Common.Text_Never;
		}

	}

}
