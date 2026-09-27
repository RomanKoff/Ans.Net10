// rev 2026-09-26

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс реализующий обобщенные и оптимизированные математические функции общего назначения.
	/// </summary>
	public static class SuppMath
	{

		/// <summary>
		/// Удерживает значение в заданных пределах (аналог системного <see cref="Math.Clamp(byte, byte, byte)"/>), используя операторы сравнения.
		/// </summary>
		/// <typeparam name="T">Тип сравниваемых данных, поддерживающий интерфейс <see cref="IComparisonOperators{TSelf, TOther, TResult}"/>.</typeparam>
		/// <param name="value">Проверяемое значение.</param>
		/// <param name="minLimit">Минимально допустимая граница числового диапазона.</param>
		/// <param name="maxLimit">Максимально допустимая граница числового диапазона.</param>
		/// <returns>Ограниченное значение, гарантированно не выходящее за рамки указанных лимитов.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetRestrict<T>(
			T value,
			T minLimit,
			T maxLimit)
			where T : IComparisonOperators<T, T, bool>
		{
			if (value < minLimit)
				return minLimit;
			return value > maxLimit ? maxLimit : value;
		}


		/// <summary>
		/// Округляет число с плавающей запятой до ближайшего целого значения и приводит к типу <see cref="int"/>.
		/// </summary>
		/// <remarks>
		/// Округление выполняется по правилу «от нуля» (<see cref="MidpointRounding.AwayFromZero"/>), когда половина округляется до ближайшего большего по модулю числа.
		/// </remarks>
		/// <typeparam name="T">Тип числа, реализующий интерфейс <see cref="IFloatingPoint{T}"/>.</typeparam>
		/// <param name="value">Исходное значение для округления.</param>
		/// <returns>Округленное значение, приведенное к типу <see cref="int"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundToInt<T>(
			T value)
			where T : IFloatingPoint<T>
		{
			T rounded1 = T.Round(value, MidpointRounding.AwayFromZero);
			return int.CreateChecked(rounded1);
		}


		/// <summary>
		/// Округляет число с плавающей запятой до ближайшего целого значения и приводит к типу <see cref="uint"/>.
		/// </summary>
		/// <remarks>
		/// Если переданное значение является отрицательным, метод безопасно возвращает <c>0</c>. Округление выполняется по правилу <see cref="MidpointRounding.AwayFromZero"/>.
		/// </remarks>
		/// <typeparam name="T">Тип числа, реализующий интерфейс <see cref="IFloatingPoint{T}"/>.</typeparam>
		/// <param name="value">Исходное значение для округления.</param>
		/// <returns>Округленное значение, приведенное к типу <see cref="uint"/>, либо <c>0</c>, если число было отрицательным.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RoundToUInt<T>(
			T value)
			where T : IFloatingPoint<T>
		{
			if (value < T.Zero)
				return 0;
			T rounded1 = T.Round(value, MidpointRounding.AwayFromZero);
			return uint.CreateChecked(rounded1);
		}


		/// <summary>
		/// Пропорционально переносит (масштабирует) значение из одного числового диапазона в другой для обобщенных типов с плавающей запятой.
		/// </summary>
		/// <typeparam name="T">Тип числа, реализующий интерфейс <see cref="IFloatingPoint{T}"/>.</typeparam>
		/// <param name="value">Исходное масштабируемое значение.</param>
		/// <param name="fromMin">Нижняя граница исходного диапазона.</param>
		/// <param name="fromMax">Верхняя граница исходного диапазона.</param>
		/// <param name="toMin">Нижняя граница целевого диапазона.</param>
		/// <param name="toMax">Верхняя граница целевого диапазона.</param>
		/// <param name="useCrop">Если передано значение <see langword="true"/>, результат будет жестко ограничен рамками целевого диапазона через метод <see cref="GetRestrict{T}"/>.</param>
		/// <returns>Преобразованное число, спроецированное в целевой диапазон.</returns>
		public static T Map<T>(
			T value,
			T fromMin,
			T fromMax,
			T toMin,
			T toMax,
			bool useCrop)
			where T : IFloatingPoint<T>
		{
			if (fromMax == fromMin)
				return toMin;
			T normal1 = (value - fromMin) / (fromMax - fromMin);
			T abs1 = (toMax - toMin) * normal1;
			T result1 = abs1 + toMin;
			if (useCrop)
			{
				T min1 = T.Min(toMin, toMax);
				T max1 = T.Max(toMin, toMax);
				return GetRestrict(result1, min1, max1);
			}
			return result1;
		}


		/// <summary>
		/// Пропорционально переносит (масштабирует) значение типа <see cref="int"/> из одного диапазона в другой с последующим округлением результата до целого числа.
		/// </summary>
		/// <param name="value">Исходное целочисленное значение.</param>
		/// <param name="fromMin">Нижняя граница исходного диапазона.</param>
		/// <param name="fromMax">Верхняя граница исходного диапазона.</param>
		/// <param name="toMin">Нижняя граница целевого диапазона.</param>
		/// <param name="toMax">Верхняя граница целевого диапазона.</param>
		/// <param name="useCrop">Если передано значение <see langword="true"/>, результат будет жестко ограничен рамками целевого диапазона.</param>
		/// <returns>Преобразованное и округленное целое число в целевом диапазоне.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Map(
			int value,
			int fromMin,
			int fromMax,
			int toMin,
			int toMax,
			bool useCrop)
		{
			double value1 = Map((double)value, fromMin, fromMax, toMin, toMax, useCrop);
			return RoundToInt(value1);
		}


		/// <summary>
		/// Возвращает ближайшее число, которое делится на заданный делитель без остатка в большую сторону по модулю.
		/// </summary>
		/// <typeparam name="T">Тип числа, реализующий интерфейс <see cref="INumber{T}"/>.</typeparam>
		/// <param name="value">Исходное число для проверки.</param>
		/// <param name="div">Делитель. Если равен нулю (<c>T.Zero</c>), метод вернет исходное значение <paramref name="value"/>.</param>
		/// <returns>Ближайшее число, кратное параметру <paramref name="div"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetNextDivisible<T>(
			T value,
			T div)
			where T : INumber<T>
		{
			if (div == T.Zero)
				return value;
			T rem1 = value % div;
			if (rem1 == T.Zero)
				return value;
			if (value < T.Zero)
				return value - rem1;
			return value + (div - rem1);
		}


		/// <summary>
		/// Возвращает индекс зоны или диапазона, в который попадает значение, используя оптимизированный некоррелируемый бинарный поиск по контрольным точкам.
		/// </summary>
		/// <typeparam name="T">Тип данных, реализующий интерфейс <see cref="IComparable{T}"/>.</typeparam>
		/// <param name="value">Проверяемое значение для поиска диапазона.</param>
		/// <param name="points">Набор упорядоченных по возрастанию контрольных точек, определяющих границы зон.</param>
		/// <returns>
		/// Порядковый индекс зоны (начиная с 0). Если значение находится строго до первой точки — возвращает <c>-1</c>. 
		/// Если значение выходит за рамки последней точки — возвращает индекс последней зоны (<c>points.Length - 2</c>).
		/// </returns>
		public static int GetRangeIndex<T>(
			T value,
			params ReadOnlySpan<T> points)
			where T : IComparable<T>
		{
			if (points.Length < 2 || value.CompareTo(points[0]) < 0)
				return -1;
			int index = points.BinarySearch(value);
			if (index >= 0)
				return index == points.Length - 1
					? index - 1 : index;
			int bitwiseComplement = ~index;
			return bitwiseComplement >= points.Length
				? points.Length - 2 : bitwiseComplement - 1;
		}

	}

}
