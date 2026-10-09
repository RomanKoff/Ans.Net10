// rev 2026-09-26

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для сложной обработки, разделения, усечения и извлечения тегов из строковых данных.
	/// </summary>
	public static partial class Exts_String
	{

		/// <summary>
		/// Разделяет строку на фиксированное количество элементов. 
		/// Недостающие элементы заполняются пустой строкой, лишние — отсекаются.
		/// </summary>
		/// <param name="instance">Исходная строка для разделения. Допускает значение <see langword="null"/>.</param>
		/// <param name="separator">Строка-разделитель (маркер разделения сегментов).</param>
		/// <param name="count">Требуемое фиксированное количество элементов в возвращаемом массиве.</param>
		/// <returns>Массив строк строго заданной длины <paramref name="count"/>.</returns>
		public static string[] SplitFix(
			this string? instance,
			string separator,
			int count)
		{
			if (count <= 0)
				return [];
			if (string.IsNullOrEmpty(instance))
			{
				var result = new string[count];
				Array.Fill(result, string.Empty);
				return result;
			}
			var a0 = instance.Split(separator, count + 1, StringSplitOptions.TrimEntries);
			var a1 = new string[count];
			int copyCount = Math.Min(a0.Length, count);
			Array.Copy(a0, a1, copyCount);
			if (copyCount < count)
				Array.Fill(a1, string.Empty, copyCount, count - copyCount);
			return a1;
		}


		/// <summary>
		/// Разделяет строку на фиксированное количество элементов по указанному символу-разделителю.
		/// Выполняет очистку краев сегментов от пробелов. Недостающие элементы заполняются пустой строкой, лишние — отсекаются.
		/// </summary>
		/// <remarks>
		/// Данная перегрузка оптимизирована для платформ .NET 10 и не выделяет в куче временных 
		/// промежуточных массивов строк, выполняя трансформацию сегментов по месту.
		/// </remarks>
		/// <param name="instance">Исходная строка для разделения. Допускает значение <see langword="null"/>.</param>
		/// <param name="separator">Символ-разделитель сегментов.</param>
		/// <param name="count">Требуемое фиксированное количество элементов в возвращаемом массиве.</param>
		/// <returns>Массив строк строго заданной длины <paramref name="count"/>.</returns>
		public static string[] SplitFix(
			this string? instance,
			char separator,
			int count)
		{
			if (count <= 0)
				return [];
			var result1 = new string[count];
			if (string.IsNullOrEmpty(instance))
			{
				Array.Fill(result1, string.Empty);
				return result1;
			}
			var span1 = instance.AsSpan();
			int currentIdx1 = 0;
			for (int i1 = 0; i1 < count; i1++)
			{
				if (currentIdx1 > span1.Length)
				{
					result1[i1] = string.Empty;
					continue;
				}
				var slice1 = span1[currentIdx1..];
				if (i1 == count - 1)
				{
					result1[i1] = slice1.Trim().ToString();
					break;
				}
				int nextSep1 = slice1.IndexOf(separator);
				if (nextSep1 >= 0)
				{
					result1[i1] = slice1[..nextSep1].Trim().ToString();
					currentIdx1 += nextSep1 + 1;
				}
				else
				{
					result1[i1] = slice1.Trim().ToString();
					currentIdx1 = span1.Length + 1;
				}
			}
			return result1;
		}


		/// <summary>
		/// Удаляет повторяющуюся подстроку в НАЧАЛЕ строки или заменяет её на альтернативную подстроку.
		/// </summary>
		/// <param name="instance">Исходная строка для обработки.</param>
		/// <param name="trimString">Целевая подстрока в начале, которую необходимо удалить или заменить.</param>
		/// <param name="replacement">Опциональная строка замены. Если равен <see langword="null"/>, подстрока удаляется.</param>
		/// <returns>Результирующая модифицированная строка.</returns>
		public static string GetTrimStart(
			this string instance,
			string trimString,
			string? replacement = null)
		{
			return instance.ReplaceStart(trimString, replacement);
		}


		/// <summary>
		/// Удаляет повторяющуюся подстроку в КОНЦЕ строки или заменяет её на альтернативную подстроку.
		/// </summary>
		/// <param name="instance">Исходная строка для обработки.</param>
		/// <param name="trimString">Целевая подстрока в конце, которую необходимо удалить или заменить.</param>
		/// <param name="replacement">Опциональная строка замены. Если равен <see langword="null"/>, подстрока удаляется.</param>
		/// <returns>Результирующая модифицированная строка.</returns>
		public static string GetTrimEnd(
			this string instance,
			string trimString,
			string? replacement = null)
		{
			return instance.ReplaceEnd(trimString, replacement);
		}


		/// <summary>
		/// Возвращает из строки подстроку, находящуюся между подстроками <paramref name="before"/> и <paramref name="after"/>.
		/// </summary>
		/// <param name="instance">Исходная строка. Допускает значение <see langword="null"/>.</param>
		/// <param name="before">Подстрока, идущая непосредственно перед целевым фрагментом.</param>
		/// <param name="after">Подстрока, идущая непосредственно после целевого фрагмента.</param>
		/// <param name="comparisonType">Правила языкового и регистрового сравнения строк. По умолчанию используется <see cref="StringComparison.InvariantCulture"/>.</param>
		/// <returns>Найденная изолированная подстрока или <see cref="string.Empty"/>, если условия поиска не выполнены.</returns>
		public static string GetTag(
			this string? instance,
			string before,
			string after,
			StringComparison comparisonType = StringComparison.InvariantCulture)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			int i1 = string.IsNullOrEmpty(before)
				? 0 : instance.IndexOf(before, comparisonType);
			if (i1 < 0)
				return string.Empty;
			i1 += string.IsNullOrEmpty(before) ? 0 : before.Length;
			int i2 = string.IsNullOrEmpty(after)
				? instance.Length
				: instance.IndexOf(after, i1, comparisonType);
			if (i2 < 0)
				return string.Empty;
			return instance[i1..i2];
		}

	}

}
