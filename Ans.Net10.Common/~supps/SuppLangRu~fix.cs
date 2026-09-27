// rev 2026-09-26

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	public static partial class SuppLangRu
	{

		private static readonly SearchValues<char> _umlautSearch = SearchValues.Create("ёЁ");


		/// <summary>
		/// Заменяет букву 'ё' и 'Ё' на 'е' и 'Е'.
		/// </summary>
		/// <param name="value">Исходный символ для проверки.</param>
		/// <returns>Символ 'е' или 'Е', если на входе была буква с умлаутом; в противном случае — исходный символ <paramref name="value"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char GetFixUmlautRu(
			char value)
		{
			return value switch
			{
				'ё' => 'е',
				'Ё' => 'Е',
				_ => value
			};
		}


		/// <summary>
		/// Возвращает новую строку, в которой все буквы 'ё' и 'Ё' заменены на 'е' и 'Е'. 
		/// Если в тексте отсутствуют символы умлаута, оптимизировано возвращает исходную ссылку без выделения памяти в куче.
		/// </summary>
		/// <param name="source">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <returns>Очищенная от умлаутов строка; <see cref="string.Empty"/>, если входная строка пуста или равна <see langword="null"/>.</returns>
		public static string GetFixUmlautRu(
			string? source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			if (source.AsSpan().IndexOfAny(_umlautSearch) == -1)
				return source;
			return string.Create(source.Length, source, static (span1, src1) =>
			{
				for (int i1 = 0; i1 < span1.Length; i1++)
					span1[i1] = GetFixUmlautRu(src1[i1]);
			});
		}


		/// <summary>
		/// Выполняет быструю замену всех букв 'ё' и 'Ё' на 'е' и 'Е' непосредственно в исходном объекте <see cref="StringBuilder"/> (модификация по месту).
		/// </summary>
		/// <param name="source">Исходный экземпляр <see cref="StringBuilder"/> для модификации. Допускает значение <see langword="null"/>.</param>
		/// <returns>Тот же экземпляр <see cref="StringBuilder"/> с выполненными заменами, либо <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StringBuilder? FixUmlautRu(
			StringBuilder? source)
		{
			if (source == null || source.Length == 0)
				return source;
			return source
				.Replace('ё', 'е')
				.Replace('Ё', 'Е');
		}


		/// <summary>
		/// Возвращает НОВЫЙ экземпляр <see cref="StringBuilder"/>, в котором все буквы 'ё' и 'Ё' заменены на 'е' и 'Е'. Исходный объект остается неизменным.
		/// </summary>
		/// <param name="source">Исходный базовый экземпляр <see cref="StringBuilder"/>. Допускает значение <see langword="null"/>.</param>
		/// <returns>Новый независимый объект <see cref="StringBuilder"/> с выполненными заменами.</returns>
		public static StringBuilder GetFixUmlautRu(
			StringBuilder? source)
		{
			if (source == null)
				return new StringBuilder();
			var sb1 = new StringBuilder(source.Length);
			foreach (ReadOnlyMemory<char> chunk1 in source.GetChunks())
			{
				var span1 = chunk1.Span;
				for (int i1 = 0; i1 < span1.Length; i1++)
					sb1.Append(GetFixUmlautRu(span1[i1]));
			}
			return sb1;
		}


		/// <summary>
		/// Заменяет кириллический знак номера на международный типографический эквивалент (№ в Nº).
		/// </summary>
		/// <param name="source">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <returns>Строка с замененным знаком номера, либо <see cref="string.Empty"/>, если входная строка пуста или равна <see langword="null"/>.</returns>
		public static string GetFixNumberRu(
			string? source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			return source.Replace("№", "Nº");
		}


		/// <summary>
		/// Возвращает экземпляр <see cref="StringBuilder"/>, в котором кириллический знак номера заменен на международный эквивалент (№ в Nº). 
		/// Если знак «№» полностью отсутствует, в целях оптимизации возвращается исходный объект без выделения дополнительной памяти.
		/// </summary>
		/// <param name="source">Исходный экземпляр <see cref="StringBuilder"/>. Допускает значение <see langword="null"/>.</param>
		/// <returns>Экземпляр <see cref="StringBuilder"/> с примененными правилами замены.</returns>
		public static StringBuilder GetFixNumberRu(
			StringBuilder? source)
		{
			if (source == null)
				return new StringBuilder();
			int i1 = 0;
			foreach (ReadOnlyMemory<char> chunk1 in source.GetChunks())
				i1 += chunk1.Span.Count('№');
			if (i1 == 0)
				return source;
			var sb1 = new StringBuilder(source.Length + i1);
			foreach (ReadOnlyMemory<char> chunk1 in source.GetChunks())
				sb1.Append(chunk1.Span);
			return sb1.Replace("№", "Nº");
		}

	}

}
