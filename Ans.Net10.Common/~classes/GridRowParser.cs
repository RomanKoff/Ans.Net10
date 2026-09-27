// rev 2026-09-25

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Предоставляет методы безопасного, отказоустойчивого и типизированного извлечения данных 
	/// из ячеек отдельной записи (строки) формата GRID по их порядковому индексу.
	/// </summary>
	public sealed class GridRowParser
	{

		private readonly string[] _fields;
		private readonly IFormatProvider _provider;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="GridRowParser"/> на основе массива сырых строковых ячеек.
		/// </summary>
		/// <param name="fields">Массив строковых полей, извлеченных из текущей строки GRID-записи.</param>
		/// <param name="provider">
		/// Поставщик форматирования (<see cref="IFormatProvider"/>), используемый для парсинга чисел и дат.
		/// Если передан <see langword="null"/> — по умолчанию применяется <see cref="CultureInfo.InvariantCulture"/>.
		/// </param>
		public GridRowParser(
			string[] fields,
			IFormatProvider? provider)
		{
			_fields = fields;
			_provider = provider ?? CultureInfo.InvariantCulture;
		}


		/* functions */


		/// <summary>
		/// Безопасно возвращает строковое значение ячейки по указанному индексу с автоматическим удалением пробелов по краям.
		/// </summary>
		/// <param name="index">Порядковый индекс поля в строке записи (индексация начинается с 0).</param>
		/// <returns>
		/// Очищенная от пробелов строка ячейки. Если индекс находится вне границ массива или поле содержит 
		/// пустое значение, возвращается <see cref="string.Empty"/>. Метод никогда не возвращает <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetText(
			int index)
		{
			if (index < 0 || index >= _fields.Length)
				return string.Empty;

			string val1 = _fields[index];
			return string.IsNullOrEmpty(val1)
				? string.Empty
				: val1.Trim();
		}


		/// <summary>
		/// Возвращает целочисленное значение по указанному индексу с использованием заданного провайдера формата.
		/// </summary>
		/// <remarks>
		/// Для конвертации строки вызывается метод расширения <c>ToInt()</c>.
		/// </remarks>
		/// <param name="index">Порядковый индекс поля в строке записи (индексация начинается с 0).</param>
		/// <returns>
		/// Преобразованное 32-битное целое число со знаком (<see cref="int"/>). Если индекс находится вне границ, 
		/// поле пусто или конвертация завершилась ошибкой формата, возвращается значение <c>0</c>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetInt(
			int index)
		{
			string val1 = GetText(index);
			return string.IsNullOrEmpty(val1)
				? 0
				: val1.ToInt(0, _provider);
		}


		/// <summary>
		/// Возвращает вещественное значение с плавающей запятой двойной точности по указанному индексу с использованием заданного провайдера формата.
		/// </summary>
		/// <remarks>
		/// Для конвертации строки вызывается метод расширения <c>ToDouble()</c>.
		/// </remarks>
		/// <param name="index">Порядковый индекс поля в строке записи (индексация начинается с 0).</param>
		/// <returns>
		/// Преобразованное число типа <see cref="double"/>. Если индекс находится вне границ, 
		/// поле пусто или конвертация завершилась ошибкой формата, возвращается значение <c>0.0</c>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double GetReal(
			int index)
		{
			string val1 = GetText(index);
			return string.IsNullOrEmpty(val1)
				? 0.0
				: val1.ToDouble(0.0, _provider);
		}


		/// <summary>
		/// Возвращает значение даты и времени по указанному индексу с использованием заданного провайдера формата.
		/// </summary>
		/// <remarks>
		/// Для конвертации строки в дату вызывается метод расширения <c>ToDateTime()</c>.
		/// </remarks>
		/// <param name="index">Порядковый индекс поля в строке записи (индексация начинается с 0).</param>
		/// <returns>
		/// Объект <see cref="DateTime"/>, если преобразование прошло успешно; в противном случае — <see langword="null"/> 
		/// (если значение отсутствует, некорректно или индекс находится вне границ диапазона).
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DateTime? GetDate(
			int index)
		{
			string val1 = GetText(index);
			return string.IsNullOrEmpty(val1)
				? null
				: val1.ToDateTime(_provider);
		}


		/// <summary>
		/// Возвращает логическое значение по указанному индексу.
		/// </summary>
		/// <remarks>
		/// Значение вычисляется через метод расширения <c>ToBool()</c>.
		/// </remarks>
		/// <param name="index">Порядковый индекс поля в строке записи (индексация начинается с 0).</param>
		/// <returns>
		/// Значение <see langword="true"/>, если строковое содержимое ячейки эквивалентно <c>"1"</c> или <c>"true"</c> 
		/// (без учета регистра и пробелов); во всех остальных случаях (включая выход индекса за границы) — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool GetBool(
			int index)
		{
			string val1 = GetText(index);
			return !string.IsNullOrEmpty(val1) && val1.ToBool();
		}

	}

}
