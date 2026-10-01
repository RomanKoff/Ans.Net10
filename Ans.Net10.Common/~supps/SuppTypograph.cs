// rev 2026-09-28

using System.Collections.Frozen;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс, реализующий функции экранной типографики, нормализации пробелов,
	/// экранирования HTML-тегов и маскирования текста для русского языка.
	/// </summary>
	public static class SuppTypograph
	{

		private static readonly FrozenDictionary<string, string> _DICT_HTML2TEXT
			= new Dictionary<string, string> {
				{ "&", "&amp;" },
				{ ">", "&gt;" },
				{ "<", "&lt;" },
			}.ToFrozenDictionary();

		private static readonly FrozenDictionary<string, string> _DICT_TEXT2HTML
			= new Dictionary<string, string> {
				{ "&amp;", "&" },
				{ "&gt;", ">" },
				{ "&lt;", "<" },
			}.ToFrozenDictionary();

		private static readonly FrozenDictionary<string, string> _DICT_TYPOGRAFFIX
			= new Dictionary<string, string> {
				{ " г.", "&nbsp;г." },
				{ " .", "." },
				{ " ,", "," },
				{ " :", ":" },
				{ " ;", ";" },
				{ " - ", " —&nbsp;" },
				{ " -", "-" },
				{ "- ", "-" },
				{ "« ", "«" },
				{ " »", "»" },
				{ "” ", "”" },
				{ " “", "“" },
				{ "„ ", "„" },
				{ "( ", "(" },
				{ " )", ")" },
				{ "[ ", "[" },
				{ " ]", "]" },
				{ " …", "…" },
			}.ToFrozenDictionary();


		/* methods */


		/// <summary>
		/// Выполняет нормализацию текста в рамках одной строки: схлопывает пробелы и обрезает края.
		/// </summary>
		/// <param name="sb">Модифицируемый экземпляр <see cref="StringBuilder"/>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="sb"/> равен <see langword="null"/>.</exception>
		public static void FixTextLine(
			StringBuilder sb)
		{
			ArgumentNullException.ThrowIfNull(sb);
			SuppStringBuilder.FixSpecChars(sb);
			sb.ReplaceRecursively("  ", " ", false);
			sb.Trim([' ']);
		}


		/// <summary>
		/// Выполняет глубокую очистку многострочного текстового блока: нормализует пробелы,
		/// знаки табуляции, переносы строк и удаляет лишние пустые абзацы.
		/// </summary>
		/// <param name="sb">Модифицируемый экземпляр <see cref="StringBuilder"/>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="sb"/> равен <see langword="null"/>.</exception>
		public static void FixTextBox(
			StringBuilder sb)
		{
			ArgumentNullException.ThrowIfNull(sb);
			sb.Replace("\r", "");
			sb.ReplaceRecursively("  ", " ", false);
			sb.Replace(" \t", "\t");
			sb.Replace("\t ", "\t");
			sb.Replace(" \n", "\n");
			sb.Replace("\n ", "\n");
			sb.ReplaceRecursively("\n\n\n", "\n\n", false);
			sb.Trim(['\n']);
		}


		/* functions */


		/// <summary>
		/// Заменяет спецсимволы разметки их безопасными HTML-сущностями.
		/// </summary>
		/// <param name="value">Исходная строка.</param>
		/// <returns>Строка с экранированными символами или <see langword="null"/>.</returns>
		public static string? GetHtml2Text(
			string? value)
		{
			return value?.ReplaceFromDict(_DICT_HTML2TEXT);
		}


		/// <summary>
		/// Восстанавливает экранированные HTML-сущности в их исходные символы.
		/// </summary>
		/// <param name="value">Строка с HTML-сущностями.</param>
		/// <returns>Декодированная строка или <see langword="null"/>.</returns>
		public static string? GetText2Html(
			string? value)
		{
			return value?.ReplaceFromDict(_DICT_TEXT2HTML);
		}


		/// <summary>
		/// Экранирует HTML-теги в тексте с сохранением исходной структуры абзацев и отступов, 
		/// заменяя ведущие пробелы табуляцией.
		/// </summary>
		/// <param name="value">Исходный текст.</param>
		/// <returns>Форматированная строка или <see langword="null"/>.</returns>
		public static string? GetHtml2TextAndSaveStruct(
			string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			var content1 = GetHtml2Text(value.TrimEnd());
			if (string.IsNullOrEmpty(content1))
				return null;
			var lines1 = content1
				.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)
				.SkipWhile(string.IsNullOrWhiteSpace)
				.ToList();
			var firstLine1 = lines1.FirstOrDefault();
			if (firstLine1 == null)
				return null;
			var len1 = firstLine1.TakeWhile(char.IsWhiteSpace).Count();
			var tab1 = len1 > 0
				? firstLine1[..len1] : " ";
			var sb1 = new StringBuilder();
			foreach (var line1 in lines1)
				if (line1.Length >= len1)
					sb1.AppendLine(line1[len1..]
						.ReplaceStart(tab1, "&Tab;"));
				else
					sb1.AppendLine(line1);
			return sb1.ToString();
		}


		/// <summary>
		/// Выполняет первичное наложение правил автотипографики на строку (замена дефисов на тире, привязка союзов).
		/// </summary>
		/// <param name="value">Исходная строка.</param>
		/// <returns>Строка с примененными заменами или <see langword="null"/>.</returns>
		public static string? GetTypografFix(
			string? value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var cleanSpaces1 = _Consts.G_REGEX_MULTISPACE().Replace(value, " ");
			return cleanSpaces1?.ReplaceFromDict(_DICT_TYPOGRAFFIX);
		}


		/// <summary>
		/// Анализирует длину слов в строке и интеллектуально связывает короткие слова (менее 4 символов) 
		/// с последующими через неразрывный пробел (&amp;nbsp;).
		/// </summary>
		/// <param name="value">Исходное текстовое поле.</param>
		/// <returns>Текст с расставленными неразрывными пробелами или <see langword="null"/>.</returns>
		public static string? GetTypografElem(
			string? value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var fixedText1 = GetTypografFix(value);
			if (string.IsNullOrEmpty(fixedText1) || fixedText1 == " ")
				return null;
			var fStartSpace1 = fixedText1.StartsWith(' ');
			var fEndSpace1 = fixedText1.Length > 1 && fixedText1.EndsWith(' ');
			var words1 = fixedText1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			string res1;
			if (words1.Length == 1)
				res1 = words1[0];
			else
			{
				var sb1 = new StringBuilder();
				for (int i1 = 0; i1 < words1.Length - 1; i1++)
				{
					var currentWord1 = words1[i1];
					sb1.Append(currentWord1);
					sb1.Append(currentWord1.Length < 4 ? "&nbsp;" : " ");
				}
				var interim1 = sb1.ToString();
				var lastWord1 = words1[^1];
				res1 = (lastWord1.Length < 4
					&& interim1.Length > 0
					&& interim1[^1] == ' ')
					? $"{interim1[..^1]}&nbsp;{lastWord1}"
					: $"{interim1}{lastWord1}";
			}
			return $"{fStartSpace1.Make(" ")}{res1}{fEndSpace1.Make(" ")}";
		}


		/// <summary>
		/// Запускает полный цикл экранной типографики текста с изоляцией разметки HTML.
		/// </summary>
		/// <param name="value">Исходный HTML или простой текст.</param>
		/// <returns>Обработанный текст или <see langword="null"/>.</returns>
		public static string? GetTypografMin(
			string? value)
		{
			if (string.IsNullOrEmpty(value))
				return null;
			var helper1 = new TypografHelper(value);
			return helper1.Result;
		}


		/// <summary>
		/// Преобразует строку, нормализуя пробелы и удаляя управляющие символы.
		/// </summary>
		/// <param name="value">Исходная строка.</param>
		/// <returns>Нормализованная строка.</returns>
		public static string GetFixTextLine(
			string? value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			var sb1 = new StringBuilder(value);
			FixTextLine(sb1);
			return sb1.ToString();
		}


		/// <summary>
		/// Преобразует многострочный текстовый блок, нормализуя абзацы и пробельные разделители.
		/// </summary>
		/// <param name="value">Исходный текстовый блок.</param>
		/// <returns>Очищенная многострочная строка.</returns>
		public static string GetFixTextBox(
			string? value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			var sb1 = new StringBuilder(value);
			FixTextBox(sb1);
			return sb1.ToString();
		}

	}

}