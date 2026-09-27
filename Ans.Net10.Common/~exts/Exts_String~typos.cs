// rev 2026-09-26

using System.Globalization;
using System.Text;

namespace Ans.Net10.Common
{

	public static partial class Exts_String
	{

		/// <summary>
		/// Возвращает результат безопасного экранирования и преобразования символов строки. 
		/// Заменяет символы управления с кодом меньше 33 на символ подчёркивания <c>'_'</c>, а символы с кодом больше 126 — на их кодовый эквивалент вида <c>~код~</c>.
		/// </summary>
		/// <param name="instance">Исходная строка для обработки. Допускает значение <see langword="null"/>.</param>
		/// <returns>Безопасное текстовое представление строки, пригодное для вывода в лог или терминал.</returns>
		public static string GetSafeText(
			this string? instance)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			var sb1 = new StringBuilder(instance.Length);
			foreach (Rune rune1 in instance.EnumerateRunes())
			{
				int code1 = rune1.Value;
				if (code1 < 33)
					sb1.Append('_');
				else if (code1 > 126)
					sb1.Append('~').Append(code1).Append('~');
				else
					sb1.Append((char)code1);
			}
			return sb1.ToString();
		}


		/// <summary>
		/// Схлопывает последовательности из нескольких подряд идущих горизонтальных пробелов в один обычный пробел. 
		/// Переносы строк (\r, \n) и знаки горизонтальной табуляции (\t) при этом полностью сохраняются. Дополнительно удаляет пробелы в начале и в конце строки (Trim).
		/// </summary>
		/// <param name="instance">Исходная строка для нормализации пробельных разделителей. Допускает значение <see langword="null"/>.</param>
		/// <returns>Строка с нормализованными пробелами или <see cref="string.Empty"/>, если исходная строка пуста.</returns>
		public static string CollapseSpaces(
			this string? instance)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			var span1 = instance.AsSpan().Trim();
			if (span1.IsEmpty)
				return string.Empty;
			if (span1.IndexOf("  ", StringComparison.Ordinal) == -1)
				return span1.ToString();
			var sb1 = new StringBuilder(span1.Length);
			bool lastWasSpace1 = false;
			foreach (Rune rune1 in span1.EnumerateRunes())
			{
				if (Rune.GetUnicodeCategory(rune1) == UnicodeCategory.SpaceSeparator)
				{
					if (!lastWasSpace1)
					{
						sb1.Append(' ');
						lastWasSpace1 = true;
					}
				}
				else
				{
					sb1.Append(rune1);
					lastWasSpace1 = false;
				}
			}
			return sb1.ToString();
		}


		/// <summary>
		/// Проверяет, содержит ли строка символы, не входящие в белый список разрешенных знаков. 
		/// Разрешены: буквы всех регистров, цифры, стандартные знаки препинания, базовые символы (№, §, %, @, ?, *, $, €, ₽), пробелы и переносы строк.
		/// </summary>
		/// <param name="instance">Исходная строка для сканирования. Допускает значение <see langword="null"/>.</param>
		/// <returns>
		/// <see langword="true"/>, если в строке найден хотя бы один запрещенный спецсимвол; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		public static bool HasForbiddenSymbols(
			this string? instance)
		{
			if (string.IsNullOrEmpty(instance))
				return false;
			foreach (Rune rune1 in instance.EnumerateRunes())
			{
				int code1 = rune1.Value;
				var category1 = Rune.GetUnicodeCategory(rune1);
				if (category1 is UnicodeCategory.UppercaseLetter
					or UnicodeCategory.LowercaseLetter
					or UnicodeCategory.TitlecaseLetter
					or UnicodeCategory.ModifierLetter
					or UnicodeCategory.OtherLetter
					or UnicodeCategory.DecimalDigitNumber)
					continue;
				if (category1 is UnicodeCategory.DashPunctuation
					or UnicodeCategory.OpenPunctuation
					or UnicodeCategory.ClosePunctuation
					or UnicodeCategory.InitialQuotePunctuation
					or UnicodeCategory.FinalQuotePunctuation
					or UnicodeCategory.OtherPunctuation)
					continue;
				if (category1 is UnicodeCategory.SpaceSeparator)
					continue;
				if (category1 == UnicodeCategory.Control && code1 is 10 or 13 or 9)
					continue;
				if (category1 is UnicodeCategory.MathSymbol
					or UnicodeCategory.CurrencySymbol
					or UnicodeCategory.OtherSymbol)
				{
					if (code1 is 43 or 61 or 60 or 62 or 37 or 38 or 64 or 36
						or 0x2116 or 0x00A7 or 0x00A9 or 0x00AE or 0x20AC or 0x20BD or 0x00A3)
						continue;
				}
				return true;
			}
			return false;
		}


		/// <summary>
		/// Удаляет из строки все запрещенные спецсимволы, оставляя только разрешенный «белый список».
		/// </summary>
		/// <param name="instance">Исходная строка.</param>
		/// <returns>Очищенная строка без запрещенных символов.</returns>
		public static string ClearForbiddenSymbols(
			this string? instance)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			if (!instance.HasForbiddenSymbols())
				return instance;
			var sb1 = new StringBuilder(instance.Length);
			foreach (Rune rune1 in instance.EnumerateRunes())
			{
				int code1 = rune1.Value;
				var category1 = Rune.GetUnicodeCategory(rune1);
				if (category1 is UnicodeCategory.UppercaseLetter
					or UnicodeCategory.LowercaseLetter
					or UnicodeCategory.TitlecaseLetter
					or UnicodeCategory.ModifierLetter
					or UnicodeCategory.OtherLetter
					or UnicodeCategory.DecimalDigitNumber)
				{
					sb1.Append(rune1);
					continue;
				}
				if (category1 is UnicodeCategory.DashPunctuation
					or UnicodeCategory.OpenPunctuation
					or UnicodeCategory.ClosePunctuation
					or UnicodeCategory.InitialQuotePunctuation
					or UnicodeCategory.FinalQuotePunctuation
					or UnicodeCategory.OtherPunctuation)
				{
					sb1.Append(rune1);
					continue;
				}
				if (category1 is UnicodeCategory.SpaceSeparator)
				{
					sb1.Append(rune1);
					continue;
				}
				if (category1 == UnicodeCategory.Control && code1 is 10 or 13 or 9)
				{
					sb1.Append(rune1);
					continue;
				}
				if (category1 is UnicodeCategory.MathSymbol
					or UnicodeCategory.CurrencySymbol
					or UnicodeCategory.OtherSymbol)
				{
					if (code1 is 43 or 61 or 60 or 62 or 37 or 38 or 64 or 36
						or 0x2116 or 0x00A7 or 0x00A9 or 0x00AE or 0x20AC or 0x20BD or 0x00A3)
					{
						sb1.Append(rune1);
						continue;
					}
				}
			}
			return sb1.ToString();
		}


		/// <summary>
		/// Проверяет, содержит ли строка эмодзи, смайлики, пиктограммы или их модификаторы.
		/// </summary>
		/// <param name="instance">Исходная строка.</param>
		/// <returns><see langword="true"/>, если найден хотя бы один символ эмодзи; иначе <see langword="false"/>.</returns>
		public static bool HasEmoji(
			this string? instance)
		{
			if (string.IsNullOrEmpty(instance))
				return false;
			foreach (Rune rune1 in instance.EnumerateRunes())
			{
				int code1 = rune1.Value;
				var category1 = Rune.GetUnicodeCategory(rune1);
				if (category1 == UnicodeCategory.OtherSymbol)
				{
					if (code1 == 0x2116 || code1 == 0x00A7)
						continue;
					return true;
				}
				if (code1 is (>= 0x1F300 and <= 0x1F9FF)
					or (>= 0x1F600 and <= 0x1F64F)
					or (>= 0x1F680 and <= 0x1F6FF)
					or (>= 0x2600 and <= 0x27BF)
					or (>= 0x1F1E6 and <= 0x1F1FF)
					or (>= 0x1F004 and <= 0x1F0CF)
					or (>= 0x1FA00 and <= 0x1FAFF))
					return true;
				if (code1 is 0x200D
					or (>= 0x1F3FB and <= 0x1F3FF)
					or 0xFE0F)
					return true;
			}
			return false;
		}


		/// <summary>
		/// Полностью удаляет любые эмодзи, смайлики, пиктограммы и их модификаторы из текста.
		/// </summary>
		/// <param name="instance">Исходный текст.</param>
		/// <returns>Очищенная строка без эмодзи.</returns>
		public static string ClearEmoji(
			this string? instance)
		{
			if (string.IsNullOrEmpty(instance))
				return string.Empty;
			var sb1 = new StringBuilder(instance.Length);
			foreach (Rune rune1 in instance.EnumerateRunes())
			{
				int code1 = rune1.Value;
				var category1 = Rune.GetUnicodeCategory(rune1);
				if (category1 == UnicodeCategory.OtherSymbol)
				{
					if (code1 == 0x2116 || code1 == 0x00A7)
						sb1.Append(rune1);
					continue;
				}
				if (code1 is (>= 0x1F300 and <= 0x1F9FF)
					or (>= 0x1F600 and <= 0x1F64F)
					or (>= 0x1F680 and <= 0x1F6FF)
					or (>= 0x2600 and <= 0x27BF)
					or (>= 0x1F1E6 and <= 0x1F1FF)
					or (>= 0x1F004 and <= 0x1F0CF)
					or (>= 0x1FA00 and <= 0x1FAFF))
					continue;
				if (code1 is 0x200D
					or (>= 0x1F3FB and <= 0x1F3FF)
					or 0xFE0F)
					continue;
				sb1.Append(rune1);
			}
			return sb1.ToString();
		}

	}

}
