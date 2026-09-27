// rev 2026-09-26

using System.Buffers;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для оптимизированной работы с объектами <see cref="StringBuilder"/>.
	/// </summary>
	public static class SuppStringBuilder
	{

		/// <summary>
		/// Набор управляющих спецсимволов ASCII (с кодами от 0 до 31), признанных невалидными для стандартного текстового вывода.
		/// </summary>
		/// <value>Экземпляр <see cref="SearchValues{Char}"/>, оптимизированный для быстрого поиска управляющих символов.</value>
		public static readonly SearchValues<char> INVALID_CHARS = SearchValues.Create(
			"\0\x01\x02\x03\x04\x05\x06\a\b\t\n\v\f\r\x0e\x0f\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a\x1b\x1c\x1d\x1e\x1f");


		/* methods */


		/// <summary>
		/// Высокопроизводительно заменяет все управляющие спецсимволы (с ASCII-кодом меньше 32) внутри <see cref="StringBuilder"/> на обычные пробелы.
		/// </summary>
		/// <remarks>
		/// Метод сначала выполняет ленивую проверку чанков через <see cref="StringBuilder.GetChunks"/>, и только при наличии невалидных символов производит 
		/// замену в арендованном из пула <see cref="ArrayPool{Char}.Shared"/> массиве, минимизируя нагрузку на сборщик мусора (GC).
		/// </remarks>
		/// <param name="sb">Модифицируемый экземпляр <see cref="StringBuilder"/>.</param>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="sb"/> равен <see langword="null"/>.</exception>
		public static void FixSpecChars(
			StringBuilder sb)
		{
			ArgumentNullException.ThrowIfNull(sb);
			if (sb.Length == 0)
				return;
			bool hasInvalid1 = false;
			foreach (var chunk1 in sb.GetChunks())
				if (chunk1.Span.IndexOfAny(INVALID_CHARS) != -1)
				{
					hasInvalid1 = true;
					break;
				}
			if (!hasInvalid1)
				return;
			int len1 = sb.Length;
			var rented1 = ArrayPool<char>.Shared.Rent(len1);
			try
			{
				sb.CopyTo(0, rented1, 0, len1);
				var span1 = rented1.AsSpan(0, len1);
				int i1;
				while ((i1 = span1.IndexOfAny(INVALID_CHARS)) != -1)
					span1[i1] = ' ';
				sb.Clear();
				sb.Append(span1);
			}
			finally
			{
				ArrayPool<char>.Shared.Return(rented1);
			}
		}

	}

}
