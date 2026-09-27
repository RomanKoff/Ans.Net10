// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	public static partial class Exts__to
	{

		/// <summary>
		/// Разбивает последовательность на блоки (массивы) заданного размера.
		/// </summary>
		/// <typeparam name="T">Тип элементов обрабатываемой последовательности.</typeparam>
		/// <param name="source">Исходная последовательность элементов. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Максимальное количество элементов в одном результирующем блоке.</param>
		/// <returns>Последовательность массивов, содержащих элементы исходной коллекции, разбитые на батчи.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T[]> ToGrid<T>(
			this IEnumerable<T>? source,
			int count)
		{
			if (source == null || count < 1)
				return [];
			return source.Chunk(count);
		}


		/// <summary>
		/// Преобразует массив строк в массив целых чисел. Если элемент не является валидным числом, вместо него записывается 0.
		/// </summary>
		/// <param name="source">Исходный массив строк. Допускает значение <see langword="null"/>.</param>
		/// <returns>Массив целых чисел типа <see cref="int"/>.</returns>
		public static int[] ToIntArray(
			this string[]? source)
		{
			if (source == null || source.Length == 0)
				return [];
			var a1 = new int[source.Length];
			for (var i1 = 0; i1 < source.Length; i1++)
				a1[i1] = source[i1].ToInt(0);
			return a1;
		}


		/// <summary>
		/// Разбивает строку по разделителям <see cref="SEP_ITEMS2"/> и преобразует элементы в массив целых чисел без выделения промежуточных строк в куче.
		/// </summary>
		/// <param name="source">Исходная строка, содержащая текстовые числа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Массив целых чисел. Если исходная строка пуста или равна <see langword="null"/>, возвращается пустой массив.</returns>
		public static int[] ToIntArray(
			this string? source)
		{
			if (string.IsNullOrEmpty(source))
				return [];
			var span1 = source.AsSpan();
			int count1 = 1;
			int offset1 = 0;
			while (true)
			{
				var i1 = span1[offset1..].IndexOfAny(SEP_ITEMS2);
				if (i1 == -1)
					break;
				count1++;
				offset1 += i1 + 1;
			}
			var a1 = new int[count1];
			int i2 = 0;
			foreach (Range segment1 in span1.SplitAny(SEP_ITEMS2))
				a1[i2++] = int.TryParse(span1[segment1], out int val1)
					? val1 : 0;
			return a1;
		}


		/// <summary>
		/// Разбивает строку по разделителям <see cref="SEP_ITEMS2"/>, очищая элементы от пробелов и полностью игнорируя пустые сегменты, после чего преобразует их в массив целых чисел.
		/// </summary>
		/// <param name="source">Исходная строка с числами. Допускает значение <see langword="null"/>.</param>
		/// <returns>Массив целых чисел без пустых элементов.</returns>
		public static int[] ToIntArrayTrim(
			this string? source)
		{
			if (string.IsNullOrWhiteSpace(source))
				return [];
			var span1 = source.AsSpan();
			var a1 = new List<int>();
			foreach (Range segment1 in span1.SplitAny(SEP_ITEMS2))
			{
				var chunk1 = span1[segment1].Trim();
				if (chunk1.IsEmpty)
					continue;
				a1.Add(int.TryParse(chunk1, out int val1) ? val1 : 0);
			}
			return [.. a1];
		}

	}

}
