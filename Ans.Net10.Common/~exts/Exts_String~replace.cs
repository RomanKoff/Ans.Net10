// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Ans.Net10.Common
{

	public static partial class Exts_String
	{

		/// <summary>
		/// Производит замену одной подстроки на другую рекурсивно до тех пор, пока заменяемая подстрока полностью не исчезнет из текста.
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <param name="oldValue">Подстрока, которую необходимо найти и заменить.</param>
		/// <param name="newValue">Подстрока, на которую производится замена.</param>
		/// <returns>Результирующая строка после выполнения всех этапов рекурсивных замен.</returns>
		public static string GetReplaceRecursively(
			this string? instance,
			string oldValue,
			string newValue)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(oldValue))
				return instance ?? string.Empty;
			if (newValue != null && newValue.Contains(oldValue))
				return instance.Replace(oldValue, newValue);
			var sb1 = new StringBuilder(instance);
			int len1;
			do
			{
				len1 = sb1.Length;
				sb1.Replace(oldValue, newValue);
			}
			while (sb1.Length != len1);
			return sb1.ToString();
		}


		/// <summary>
		/// Производит массовую замену подстрок на основе переданного словаря соответствий.
		/// </summary>
		/// <param name="instance">Исходная строка для обработки.</param>
		/// <param name="dict">Словарь соответствий, где ключом является заменяемый фрагмент, а значением — новая подстрока.</param>
		/// <returns>Строка со всеми выполненными пакетными заменами.</returns>
		public static string ReplaceFromDict(
			this string instance,
			Dictionary<string, string> dict)
		{
			if (string.IsNullOrEmpty(instance) || dict == null || dict.Count == 0)
				return instance;
			var sb1 = new StringBuilder(instance);
			foreach (var item1 in dict)
				if (!string.IsNullOrEmpty(item1.Key))
					sb1.Replace(item1.Key, item1.Value);
			return sb1.ToString();
		}


		/// <summary>
		/// Возвращает альтернативную строку, если исходная строка полностью эквивалентна сравниваемой. В противном случае возвращает оригинал.
		/// </summary>
		/// <param name="instance">Исходная строка для проверки.</param>
		/// <param name="compared">Строка, с которой производится проверка на точное равенство.</param>
		/// <param name="newest">Новая строка, возвращаемая в случае успешного совпадения.</param>
		/// <returns>Результат условной эквивалентной замены строки.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ReplaceIfEqual(
			this string instance,
			string compared,
			string newest)
		{
			return instance == compared
				? newest : instance;
		}


		/// <summary>
		/// Заменяет любые символы, содержащиеся в строке-паттерне, заданной строкой-маской.
		/// </summary>
		/// <param name="instance">Исходная строка для сканирования символов.</param>
		/// <param name="mask">Строка-маска, вставляемая вместо каждого обнаруженного целевого символа.</param>
		/// <param name="charsPattern">Строка, содержащая сплошной набор заменяемых индивидуальных символов.</param>
		/// <returns>Строка, в которой все целевые символы заменены на маску.</returns>
		public static string ReplaceByChars(
			this string instance,
			string mask,
			string charsPattern)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(charsPattern))
				return instance;
			var sb1 = new StringBuilder(instance.Length);
			var span1 = charsPattern.AsSpan();
			foreach (var c1 in instance)
				if (span1.Contains(c1))
					sb1.Append(mask);
				else
					sb1.Append(c1);
			return sb1.ToString();
		}


		/// <summary>
		/// Заменяет любые символы из указанного массива заданной строкой-маской. Высокопроизводительная перегрузка без аллокаций.
		/// </summary>
		/// <param name="instance">Исходная строка для сканирования символов.</param>
		/// <param name="mask">Строка-маска, вставляемая вместо каждого обнаруженного целевого символа.</param>
		/// <param name="chars">Массив заменяемых символов.</param>
		/// <returns>Строка, в которой целевые символы из массива заменены маской.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ReplaceByChars(
			this string instance,
			string mask,
			params char[] chars)
		{
			if (string.IsNullOrEmpty(instance) || chars == null || chars.Length == 0)
				return instance;
			var sb1 = new StringBuilder(instance.Length);
			var span1 = chars.AsSpan();
			foreach (var c1 in instance)
				if (span1.Contains(c1))
					sb1.Append(mask);
				else
					sb1.Append(c1);
			return sb1.ToString();
		}


		/// <summary>
		/// Производит замену хэштегов в строке на основе переданного словаря соответствий имен и их подстановок.
		/// </summary>
		/// <param name="instance">Исходная строка, содержащая хэштеги.</param>
		/// <param name="dictionary">Словарь соответствия чистых имен хэштегов (без знака префикса) и их полных текстовых замен.</param>
		/// <param name="marker">Символ-префикс разметки хэштега. По умолчанию равен <c>'#'</c>.</param>
		/// <returns>Строка с выполненными заменами найденных хэштегов.</returns>
		public static string ReplaceHashtagsFromDict(
			this string instance,
			Dictionary<string, string> dictionary,
			char marker = '#')
		{
			if (string.IsNullOrEmpty(instance) || dictionary == null || dictionary.Count == 0)
				return instance;
			string marker1 = Regex.Escape(marker.ToString());
			return Regex.Replace(instance, $"{marker1}([\\w\\d]+)", x =>
			{
				var hash1 = x.Groups[1].Value;
				return dictionary.TryGetValue(hash1, out var replacement1)
					? replacement1 : x.Value;
			});
		}


		/// <summary>
		/// Находит непрерывно повторяющуюся строку-маску в НАЧАЛЕ строки и заменяет все её вхождения указанной альтернативной подстрокой.
		/// </summary>
		/// <param name="instance">Исходная строка для анализа.</param>
		/// <param name="mask">Повторяющаяся подстрока-маска, непрерывную цепочку которой нужно локализовать в самом начале.</param>
		/// <param name="replacement">Опциональная строка, на которую заменяется каждое изолированное вхождение маски.</param>
		/// <returns>Результирующая строка с заменой серии масок в начале текста.</returns>
		public static string ReplaceStart(
			this string instance,
			string mask,
			string? replacement = null)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(mask))
				return instance;
			int sourceLen1 = instance.Length;
			int maskLen1 = mask.Length;
			int curr1 = 0;
			var sourceSpan1 = instance.AsSpan();
			var maskSpan1 = mask.AsSpan();
			while (curr1 + maskLen1 <= sourceLen1
				&& sourceSpan1.Slice(curr1, maskLen1).SequenceEqual(maskSpan1))
				curr1 += maskLen1;
			if (curr1 == 0)
				return instance;
			int repeats1 = curr1 / maskLen1;
			var sb1 = new StringBuilder(sourceLen1 + (repeats1 * (replacement?.Length ?? 0)));
			if (!string.IsNullOrEmpty(replacement))
				for (int i1 = 0; i1 < repeats1; i1++)
					sb1.Append(replacement);
			sb1.Append(sourceSpan1[curr1..]);
			return sb1.ToString();
		}


		/// <summary>
		/// Находит непрерывно повторяющуюся строку-маску в КОНЦЕ строки и заменяет все её вхождения указанной альтернативной подстрокой.
		/// </summary>
		/// <param name="instance">Исходная строка для анализа.</param>
		/// <param name="mask">Повторяющаяся подстрока-маска, непрерывную цепочку которой нужно локализовать в самом конце.</param>
		/// <param name="replacement">Опциональная строка, на которую заменяется каждое изолированное вхождение маски.</param>
		/// <returns>Результирующая строка с заменой серии масок в конце текста.</returns>
		public static string ReplaceEnd(
			this string instance,
			string mask,
			string? replacement = null)
		{
			if (string.IsNullOrEmpty(instance) || string.IsNullOrEmpty(mask))
				return instance;
			int sourceLen1 = instance.Length;
			int maskLen1 = mask.Length;
			int curr1 = sourceLen1;
			var sourceSpan1 = instance.AsSpan();
			var maskSpan1 = mask.AsSpan();
			while (curr1 - maskLen1 >= 0
				&& sourceSpan1.Slice(curr1 - maskLen1, maskLen1).SequenceEqual(maskSpan1))
				curr1 -= maskLen1;
			if (curr1 == sourceLen1)
				return instance;
			int repeats1 = (sourceLen1 - curr1) / maskLen1;
			var sb1 = new StringBuilder(curr1 + (repeats1 * (replacement?.Length ?? 0)));
			sb1.Append(sourceSpan1[..curr1]);
			if (!string.IsNullOrEmpty(replacement))
				for (int i1 = 0; i1 < repeats1; i1++)
					sb1.Append(replacement);
			return sb1.ToString();
		}

	}

}
