// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для генерации криптографически стойких случайных данных.
	/// </summary>
	public static class SuppRandom
	{

		/// <summary>
		/// Возвращает криптографически стойкое случайное число в диапазоне от <paramref name="min"/> до <paramref name="max"/> включительно.
		/// </summary>
		/// <remarks>
		/// Метод автоматически разворачивает границы диапазона, если начальное значение <paramref name="min"/> хронологически больше конечного <paramref name="max"/>. 
		/// Безопасно обрабатывает пограничные системные значения <see cref="int.MinValue"/> и <see cref="int.MaxValue"/> без переполнения разрядов.
		/// </remarks>
		/// <param name="min">Нижняя (минимальная) граница числового диапазона.</param>
		/// <param name="max">Верхняя (максимальная) граница числового диапазона.</param>
		/// <returns>Случайное 32-битное целое число со знаком внутри заданного закрытого диапазона.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetInt(
			int min,
			int max)
		{
			if (min > max)
				(min, max) = (max, min);
			if (max < int.MaxValue)
				return RandomNumberGenerator.GetInt32(min, max + 1);
			if (min > int.MinValue)
				return RandomNumberGenerator.GetInt32(min - 1, int.MaxValue) + 1;
			Span<byte> bytes = stackalloc byte[4];
			RandomNumberGenerator.Fill(bytes);
			return BitConverter.ToInt32(bytes);
		}


		/// <summary>
		/// Генерирует случайную строку на основе переданного набора символов-маски и диапазона длин.
		/// </summary>
		/// <remarks>
		/// Метод автоматически разворачивает границы диапазона длин при нарушении порядка и использует оптимизированные алгоритмы без выделения промежуточных сущностей в куче.
		/// </remarks>
		/// <param name="mask">Строка-маска, содержащая уникальный набор доступных для генерации символов.</param>
		/// <param name="minLength">Минимально возможная длина генерируемой строки.</param>
		/// <param name="maxLength">Максимально возможная длина генерируемой строки.</param>
		/// <returns>Случайная строка, состоящая исключительно из символов маски, длина которой находится в рамках заданных лимитов.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если строка-маска <paramref name="mask"/> пуста или равна <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если результирующая минимальная длина диапазона строго меньше 0.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetString(
			string mask,
			int minLength,
			int maxLength)
		{
			ArgumentException.ThrowIfNullOrEmpty(
				mask, nameof(mask));
			if (minLength > maxLength)
				(minLength, maxLength) = (maxLength, minLength);
			ArgumentOutOfRangeException.ThrowIfLessThan(
				minLength, 0, nameof(minLength));
			int length1 = GetInt(minLength, maxLength);
			return length1 == 0
				? string.Empty
				: RandomNumberGenerator.GetString(mask, length1);
		}

	}

}
