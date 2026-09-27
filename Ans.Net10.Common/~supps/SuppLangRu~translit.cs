// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	public static partial class SuppLangRu
	{

		/// <summary>
		/// Транслитерирует строку русского текста латинским алфавитом в соответствии
		/// с требованиями ГОСТ Р 7.0.34-2014 (https://www.ifap.ru/library/gost/70342014.pdf).
		/// </summary>
		/// <param name="text">Исходный русский текст для транслитерации. Допускает значение <see langword="null"/>.</param>
		/// <returns>Транслитерированная латиницей строка; <see cref="string.Empty"/>, если на входе пустая строка или <see langword="null"/>.</returns>
		public static string GetTranslitRuToEn(
			string? text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			var sb1 = new StringBuilder(text.Length * 2);
			_translitSpan(text.AsSpan(), sb1);
			return sb1.ToString();
		}


		/// <summary>
		/// Выполняет высокопроизводительную транслитерацию текста из одного экземпляра <see cref="StringBuilder"/> в другой чанками без промежуточных аллокаций памяти в куче.
		/// </summary>
		/// <param name="source">Исходный экземпляр <see cref="StringBuilder"/> с кириллическим текстом.</param>
		/// <param name="destination">Целевой экземпляр <see cref="StringBuilder"/> для записи результата транслитерации.</param>
		public static void TranslitRuToEn(
			StringBuilder source,
			StringBuilder destination)
		{
			if (source == null || destination == null)
				return;
			foreach (var chunk1 in source.GetChunks())
				_translitSpan(chunk1.Span, destination);
		}


		/// <summary>
		/// Преобразует произвольный текст в безопасный нижнерегистровый URL-slug, транслитерируя кириллицу и заменяя разделители на одиночные дефисы.
		/// </summary>
		/// <remarks>
		/// Метод автоматически удаляет дубликаты дефисов, а также зачищает дефисы на концах результирующей строки. 
		/// Твердый (ъ) и мягкий (ь) знаки при этом полностью игнорируются.
		/// </remarks>
		/// <param name="text">Исходный текст для формирования идентификатора. Допускает значение <see langword="null"/>.</param>
		/// <returns>Безопасная строка для использования в URL-адресах (slug) или <see cref="string.Empty"/>.</returns>
		public static string ToSlug(
			string? text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
			var sb1 = new StringBuilder(text.Length * 2);
			bool lastWasDash1 = false;
			foreach (char ch1 in text)
			{
				char ch2 = char.ToLowerInvariant(ch1);
				string? s1 = _getRepl(ch2);
				if (s1 != null)
				{
					if (s1.Length > 0)
					{
						sb1.Append(s1);
						lastWasDash1 = false;
					}
					continue;
				}
				if ((ch2 >= 'a' && ch2 <= 'z') || (ch2 >= '0' && ch2 <= '9'))
				{
					sb1.Append(ch2);
					lastWasDash1 = false;
					continue;
				}
				if (ch2 is ' ' or '-' or '_' or '/' or '\\' or '.' or ',' or ':' or ';')
				{
					if (!lastWasDash1 && sb1.Length > 0)
					{
						sb1.Append('-');
						lastWasDash1 = true;
					}
				}
			}
			if (sb1.Length > 0 && sb1[^1] == '-')
				sb1.Length--;
			return sb1.ToString();
		}


		/* privates */


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string? _getRepl(
			char ch)
		{
			return ch switch
			{
				'а' => "a",
				'б' => "b",
				'в' => "v",
				'г' => "g",
				'д' => "d",
				'е' => "e",
				'ё' => "yo",
				'ж' => "zh",
				'з' => "z",
				'и' => "i",
				'й' => "y",
				'к' => "k",
				'л' => "l",
				'м' => "m",
				'н' => "n",
				'о' => "o",
				'п' => "p",
				'р' => "r",
				'с' => "s",
				'т' => "t",
				'у' => "u",
				'ф' => "f",
				'х' => "kh",
				'ц' => "ts",
				'ч' => "ch",
				'ш' => "sh",
				'щ' => "shh",
				'ъ' => string.Empty,
				'ы' => "y",
				'ь' => string.Empty,
				'э' => "e",
				'ю' => "yu",
				'я' => "ya",
				_ => null
			};
		}


		private static void _translitSpan(
			ReadOnlySpan<char> span,
			StringBuilder sb)
		{
			int len1 = span.Length;
			for (int i1 = 0; i1 < len1; i1++)
			{
				char ch1 = span[i1];
				bool isUpper1 = char.IsUpper(ch1);
				char ch2 = char.ToLowerInvariant(ch1);
				string? repl1 = _getRepl(ch2);
				if (repl1 == null)
				{
					sb.Append(ch1);
					continue;
				}
				if (isUpper1)
				{
					bool nextIsUpper1 = (i1 + 1 < len1) && char.IsUpper(span[i1 + 1]);
					bool prevIsUpper1 = (i1 > 0) && char.IsUpper(span[i1 - 1]);
					if (nextIsUpper1 || prevIsUpper1 || repl1.Length == 1)
						sb.Append(repl1.ToUpperInvariant());
					else
					{
						sb.Append(char.ToUpperInvariant(repl1[0]));
						if (repl1.Length > 1)
							sb.Append(repl1.AsSpan(1));
					}
				}
				else
					sb.Append(repl1);
			}
		}

	}

}
