// rev 2026-09-26

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для работы с английским языком и правилами грамматической плюрализации.
	/// </summary>
	public static class SuppLangEn
	{

		/// <summary>
		/// Возвращает грамматически корректную форму английского существительного в зависимости от числового значения.
		/// </summary>
		/// <param name="count">Количество элементов для анализа.</param>
		/// <param name="form1">Форма слова для единственного числа (например, <c>"apple"</c>).</param>
		/// <param name="formOther">Опциональная кастомная форма множественного числа. Если передано значение <see langword="null"/>, к форме <paramref name="form1"/> автоматически добавляется суффикс <c>"s"</c>.</param>
		/// <returns>Строка, содержащая корректную форму слова в зависимости от значения <paramref name="count"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetPlural(
			int count,
			string form1,
			string? formOther = null)
		{
			return count == 1
				? form1 : formOther ?? $"{form1}s";
		}


		/// <summary>
		/// Возвращает грамматически корректную форму английского существительного, подставленную в указанный строковый шаблон.
		/// </summary>
		/// <param name="template">Шаблон форматирования (например, <c>"Found {0} {1}"</c>), где параметр <c>{0}</c> — число, а <c>{1}</c> — обработанное слово. Допускает значение <see langword="null"/>.</param>
		/// <param name="count">Количество элементов.</param>
		/// <param name="form1">Форма слова для единственного числа.</param>
		/// <param name="formOther">Опциональная форма множественного числа.</param>
		/// <returns>Форматированная по шаблону строка, либо <see cref="string.Empty"/>, если параметр <paramref name="template"/> пуст или равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetPlural(
			string? template,
			int count,
			string form1,
			string? formOther = null)
		{
			if (string.IsNullOrEmpty(template))
				return string.Empty;
			var s1 = GetPlural(count, form1, formOther);
			return string.Format(template, count, s1);
		}


		/// <summary>
		/// Переводит английское английское слово из единственного числа во множественное на основе комплексного анализа суффиксов, окончаний, исключений и заимствований
		/// (https://engblog.ru/app/uploads/2009/12/Plurals-in-English-table.pdf).
		/// </summary>
		/// <remarks>
		/// Метод учитывает неисчисляемые существительные, чередование гласных, суффиксы сложных слов через дефис, 
		/// а также специфические греческие и латинские формы (например, <c>"criterion"</c> в <c>"criteria"</c>, <c>"crisis"</c> в <c>"crises"</c>).
		/// </remarks>
		/// <param name="word">Исходное слово в единственном числе.</param>
		/// <returns>Слово во множественном числе с гарантированным сохранением исходного стиля регистра символов (нижний регистр, TitleCase или UPPERCASE).</returns>
		public static string ToPlural(
			string word)
		{
			if (string.IsNullOrWhiteSpace(word))
				return word;

			// Обработка сложных слов через дефис
			if (word.Contains('-'))
				return _getPluralizeHyphenatedWord(word);

			var lowerWord1 = word.ToLowerInvariant();

			// 1. Неисчисляемые и совпадающие формы
			if (_UNCOUNTABLE_AND_MATCHING.Contains(lowerWord1))
				return _getMatchCase(word, word);

			// 2. Стандартные исключения (существительные, изменяющиеся не по правилам)
			if (_IRREGULARS.TryGetValue(lowerWord1, out var irregular1))
				return _getMatchCase(word, irregular1);

			// 3. Исключения на -о и -f/-fe из таблицы
			if (_SPECIAL_O_EXCEPTIONS.TryGetValue(lowerWord1, out var oException1))
				return _getMatchCase(word, oException1);

			if (_SPECIAL_F_EXCEPTIONS.TryGetValue(lowerWord1, out var fException1))
				return _getMatchCase(word, fException1);

			// 4. Латинские и греческие заимствования
			if (_LATIN_US_TO_I.TryGetValue(lowerWord1, out var latinUs1))
				return _getMatchCase(word, latinUs1);

			if (_LATIN_A_TO_AE.TryGetValue(lowerWord1, out var latinA1))
				return _getMatchCase(word, latinA1);

			if (_LATIN_UM_TO_A.TryGetValue(lowerWord1, out var latinUm1))
				return _getMatchCase(word, latinUm1);

			if (_LATIN_EXIX_TO_CES.TryGetValue(lowerWord1, out var latinExIx1))
				return _getMatchCase(word, latinExIx1);

			if (_GREEK_IS_TO_ES.TryGetValue(lowerWord1, out var greekIs1))
				return _getMatchCase(word, greekIs1);

			if (_GREEK_ON_TO_A.TryGetValue(lowerWord1, out var greekOn1))
				return _getMatchCase(word, greekOn1);

			// 5. Правила суффиксов на основе окончаний (используем высокоэффективный Span)
			var span1 = lowerWord1.AsSpan();

			if (span1.EndsWith("ss")
				|| span1.EndsWith("sh")
				|| span1.EndsWith("ch")
				|| span1.EndsWith("tch")
				|| span1.EndsWith("x")
				|| span1.EndsWith("z")
				|| span1.EndsWith("s"))
				return _getMatchCase(word, string.Concat(word, "es"));

			if (span1.EndsWith("y"))
			{
				if (span1.EndsWith("ay")
					|| span1.EndsWith("ey")
					|| span1.EndsWith("oy")
					|| span1.EndsWith("uy"))
					return _getMatchCase(word, string.Concat(word, "s"));
				return _getMatchCase(word, string.Concat(word.AsSpan(0, word.Length - 1), "ies"));
			}

			if (span1.EndsWith("o"))
				return _getMatchCase(word, string.Concat(word, "es"));

			if (span1.EndsWith("fe"))
				return _getMatchCase(word, string.Concat(word.AsSpan(0, word.Length - 2), "ves"));

			if (span1.EndsWith("f"))
				return _getMatchCase(word, string.Concat(word.AsSpan(0, word.Length - 1), "ves"));

			return _getMatchCase(word, string.Concat(word, "s"));
		}


		/* privates */


		private static string _getPluralizeHyphenatedWord(
			string word)
		{
			var a1 = word.Split('-');
			if (a1.Length == 0)
				return word;
			var s1 = a1[0].ToLowerInvariant();
			if (s1 == "man" || s1 == "woman")
			{
				a1[0] = ToPlural(a1[0]);
				a1[^1] = ToPlural(a1[^1]);
				return string.Join("-", a1);
			}
			if (a1.Length > 1
				&& (a1[1].Equals("in", StringComparison.OrdinalIgnoreCase)
				|| a1[1].Equals("by", StringComparison.OrdinalIgnoreCase)))
			{
				a1[0] = ToPlural(a1[0]);
				return string.Join("-", a1);
			}
			a1[^1] = ToPlural(a1[^1]);
			return string.Join("-", a1);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string _getMatchCase(
			string original,
			string result)
		{
			if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(result))
				return result;
			// Если оригинальное слово всё в ВЕРХНЕМ регистре (например, "BOOK")
			if (char.IsUpper(original[0]) && original.Length > 1 && char.IsUpper(original[1]))
				return result.ToUpperInvariant();
			// Если оригинальное слово с заглавной буквы (например, "Book")
			if (char.IsUpper(original[0]))
			{
				// Оптимизированное zero-allocation изменение первого символа на заглавный через string.Create
				return string.Create(result.Length, result, (span, state) =>
				{
					state.AsSpan().CopyTo(span);
					span[0] = char.ToUpperInvariant(span[0]);
				});
			}
			return result;
		}


		/* privates data */


		private static readonly HashSet<string> _UNCOUNTABLE_AND_MATCHING = new(StringComparer.OrdinalIgnoreCase)
		{
			"fish", "deer", "sheep", "trout", "swine", "aircraft", "means",
			"advice", "education", "hair", "information", "knowledge", "luck",
			"luggage", "money", "music", "news", "progress", "seaside", "shopping",
			"traffic", "trouble", "weather", "work"
		};


		private static readonly Dictionary<string, string> _IRREGULARS = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "man", "men" },
			{ "woman", "women" },
			{ "child", "children" },
			{ "ox", "oxen" },
			{ "foot", "feet" },
			{ "tooth", "teeth" },
			{ "goose", "geese" },
			{ "mouse", "mice" },
			{ "louse", "lice" },
			{ "brother", "brethren" }
		};


		private static readonly Dictionary<string, string> _SPECIAL_O_EXCEPTIONS = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "piano", "pianos" },
			{ "kilo", "kilos" },
			{ "photo", "photos" },
			{ "video", "videos" },
			{ "flamingo", "flamingos" },
			{ "volcano", "volcanos" }
		};


		private static readonly Dictionary<string, string> _SPECIAL_F_EXCEPTIONS = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "chief", "chiefs" },
			{ "roof", "roofs" },
			{ "safe", "safes" },
			{ "cliff", "cliffs" },
			{ "belief", "beliefs" },
			{ "scarf", "scarfs" },
			{ "wharf", "wharfs" },
			{ "dwarf", "dwarfs" },
			{ "hoof", "hoofs" }
		};


		private static readonly Dictionary<string, string> _LATIN_US_TO_I = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "stimulus", "stimuli" },
			{ "genius", "genii" },
			{ "syllabus", "syllabi" },
			{ "radius", "radii" },
			{ "cactus", "cacti" },
			{ "nucleus", "nuclei" }
		};


		private static readonly Dictionary<string, string> _LATIN_A_TO_AE = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "larva", "larvae" },
			{ "alumna", "alumnae" },
			{ "formula", "formulae" }
		};


		private static readonly Dictionary<string, string> _LATIN_UM_TO_A = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "datum", "data" },
			{ "curriculum", "curricula" },
			{ "bacterium", "bacteria" },
			{ "symposium", "symposia" },
			{ "memorandum", "memoranda" },
			{ "medium", "media" }
		};


		private static readonly Dictionary<string, string> _LATIN_EXIX_TO_CES = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "index", "indices" },
			{ "appendix", "appendices" },
			{ "codex", "codices" }
		};


		private static readonly Dictionary<string, string> _GREEK_IS_TO_ES = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "thesis", "theses" },
			{ "crisis", "crises" },
			{ "analysis", "analyses" }
		};


		private static readonly Dictionary<string, string> _GREEK_ON_TO_A = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "criterion", "criteria" },
			{ "phenomenon", "phenomena" }
		};

	}

}