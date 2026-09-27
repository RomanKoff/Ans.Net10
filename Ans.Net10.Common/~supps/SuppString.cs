// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень вариантов преобразования и форматирования регистра символов текстовых строк.
	/// </summary>
	public enum LetterCasesEnum
	{
		/// <summary>
		/// Регистр символов остается без изменений (оригинальный вид).
		/// </summary>
		Original,

		/// <summary>
		/// Все символы переводятся в нижний регистр (строчные буквы).
		/// </summary>
		Lower,

		/// <summary>
		/// Все символы переводятся в верхний регистр (ПРОПИСНЫЕ, ЗАГЛАВНЫЕ буквы).
		/// </summary>
		Upper,

		/// <summary>
		/// Первая буква всей строки переводится в верхний регистр, остальные остаются без изменений.
		/// </summary>
		FirstUpper,

		/// <summary>
		/// Первая Буква Каждого Слова В Строке Переводится В Верхний Регистр (Title Case).
		/// </summary>
		TitleCase
	}



	/// <summary>
	/// Вспомогательный класс для высокопроизводительной работы со строками, перекодирования, валидации и санитаризации.
	/// </summary>
	public static class SuppString
	{

		//	https://ru.stackoverflow.com/questions/1387144
		//	Support win1251 and koi8r:
		//		Using System.Text.Encoding.CodePages
		//		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		//	Program.cs ->
		//		SuppIO.Register_CodePagesEncodingProvider();


		/* functions */


		/// <summary>
		/// Преобразует строковые данные из кодировки Windows-1251 (Кириллица) в универсальный UTF-8.
		/// </summary>
		/// <param name="source">Исходная строка в кодировке Windows-1251.</param>
		/// <returns>Перекодированная строка в формате UTF-8, либо <see cref="string.Empty"/>, если входная строка пуста.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Convert_WINDOWS1251_UTF8(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			return _Consts.ENCODING_UTF8.GetString(
				_Consts.ENCODING_WINDOWS1251.GetBytes(source));
		}


		/// <summary>
		/// Преобразует строковые данные из кодировки KOI8-R (Кириллица) в универсальный UTF-8.
		/// </summary>
		/// <param name="source">Исходная строка в кодировке KOI8-R.</param>
		/// <returns>Перекодированная строка в формате UTF-8, либо <see cref="string.Empty"/>, если входная строка пуста.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Convert_KOI8R_UTF8(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			return _Consts.ENCODING_UTF8.GetString(
				_Consts.ENCODING_KOI8R.GetBytes(source));
		}


		/// <summary>
		/// Преобразует строковые данные из кодировки CP866 (DOS-кириллица) в универсальный UTF-8.
		/// </summary>
		/// <param name="source">Исходная строка в кодировке CP866.</param>
		/// <returns>Перекодированная строка в формате UTF-8, либо <see cref="string.Empty"/>, если входная строка пуста.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Convert_CP866_UTF8(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			return _Consts.ENCODING_UTF8.GetString(
				_Consts.ENCODING_CP866.GetBytes(source));
		}


		/// <summary>
		/// Преобразует строковые данные из западноевропейской кодировки ISO-8859-1 в универсальный UTF-8.
		/// </summary>
		/// <param name="source">Исходная строка в кодировке ISO-8859-1.</param>
		/// <returns>Перекодированная строка в формате UTF-8, либо <see cref="string.Empty"/>, если входная строка пуста.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Convert_ISO88591_UTF8(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			return _Consts.ENCODING_UTF8.GetString(
				_Consts.ENCODING_ISO88591.GetBytes(source));
		}


		/// <summary>
		/// Проверяет, является ли хотя бы одна из переданных строк непустой. Использует механизмы .NET 10 для исключения аллокаций памяти в куче.
		/// </summary>
		/// <param name="values">Высокопроизводительный фиксированный набор проверяемых строк <see cref="ReadOnlySpan{T}"/>.</param>
		/// <returns><see langword="true"/>, если найдена хотя бы одна непустая строка и не равная <see langword="null"/>; в противном случае — <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasAny(
			params ReadOnlySpan<string?> values)
		{
			foreach (var value1 in values)
				if (!string.IsNullOrEmpty(value1))
					return true;
			return false;
		}


		/// <summary>
		/// Проверяет, что абсолютно все переданные строки являются непустыми. Использует механизмы .NET 10 для исключения аллокаций памяти в куче.
		/// </summary>
		/// <param name="values">Высокопроизводительный фиксированный набор проверяемых строк <see cref="ReadOnlySpan{T}"/>.</param>
		/// <returns><see langword="true"/>, если все элементы набора гарантированно содержат текст; в противном случае — <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasAll(
			params ReadOnlySpan<string?> values)
		{
			foreach (var value1 in values)
				if (string.IsNullOrEmpty(value1))
					return false;
			return true;
		}


		/// <summary>
		/// Объединяет непустые элементы набора в одну строку, индивидуально форматируя элементы по шаблону и оборачивая итоговый результат.
		/// </summary>
		/// <param name="resultTemplate">Общий шаблон для итоговой обертки всей собранной строки (например, <c>"Результат: {0}"</c>). Если пуст — обертка не накладывается.</param>
		/// <param name="itemTemplate">Шаблон форматирования, накладываемый на каждый отдельный непустой элемент (например, <c>"[{0}]"</c>).</param>
		/// <param name="separator">Строка или символ-разделитель между склеиваемыми элементами.</param>
		/// <param name="items">Набор объединяемых строк, передаваемый без аллокаций в куче.</param>
		/// <returns>Итоговая отформатированная и склеенная строка, либо <see cref="string.Empty"/>, если результирующая коллекция была пуста.</returns>
		public static string Join(
			string resultTemplate,
			string itemTemplate,
			string separator,
			params ReadOnlySpan<string?> items)
		{
			var sb1 = new StringBuilder();
			bool f1 = true;
			bool hasTemplateItem1 = !string.IsNullOrEmpty(itemTemplate);
			foreach (var s1 in items)
				if (!string.IsNullOrEmpty(s1))
				{
					if (f1)
						f1 = false;
					else
						sb1.Append(separator);
					sb1.Append(hasTemplateItem1 ? string.Format(itemTemplate, s1) : s1);
				}
			var s2 = sb1.ToString();
			if (string.IsNullOrEmpty(s2))
				return string.Empty;
			return string.IsNullOrEmpty(resultTemplate)
				? s2 : string.Format(resultTemplate, s2);
		}


		/// <summary>
		/// Разделяет строку формата "ключ=значение" на изолированную пару ключа и значения.
		/// </summary>
		/// <remarks>
		/// Если символ разделителя <c>'='</c> полностью отсутствует в исходном тексте, метод вернет исходное значение в качестве одновременно и ключа, и значения.
		/// </remarks>
		/// <param name="definition">Исходная строка для сегментации и разделения.</param>
		/// <returns>Структура <see cref="KeyValuePair{TKey, TValue}"/> с результатами проведенного разделения.</returns>
		public static KeyValuePair<string, string> GetPair(
			string definition)
		{
			if (string.IsNullOrEmpty(definition))
				return new KeyValuePair<string, string>(string.Empty, string.Empty);
			int i1 = definition.IndexOf('=');
			if (i1 >= 0)
				return new KeyValuePair<string, string>(
					definition[..i1], definition[(i1 + 1)..]);
			return new KeyValuePair<string, string>(definition, definition);
		}


		/// <summary>
		/// Преобразует регистр символов всей строки на основе выбранного режима из перечисления <see cref="LetterCasesEnum"/>.
		/// </summary>
		/// <param name="value">Исходная строка для модификации регистра.</param>
		/// <param name="textCase">Целевой режим регистра символов.</param>
		/// <param name="forcedToLower">Принудительно переводить все остальные буквы строки в нижний регистр (актуально для режимов <see cref="LetterCasesEnum.FirstUpper"/> и <see cref="LetterCasesEnum.TitleCase"/>). По умолчанию равен <see langword="false"/>.</param>
		/// <returns>Преобразованная строка. Если исходная строка пуста, возвращает <see cref="string.Empty"/>.</returns>
		public static string GetModCase(
			string value,
			LetterCasesEnum textCase,
			bool forcedToLower = false)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			return textCase switch
			{
				LetterCasesEnum.Lower => value.ToLowerInvariant(),
				LetterCasesEnum.Upper => value.ToUpperInvariant(),
				LetterCasesEnum.FirstUpper => value.GetAsFirstUpper(forcedToLower),
				LetterCasesEnum.TitleCase => value.GetAsTitleCase(forcedToLower),
				_ => value
			};
		}


		/// <summary>
		/// Заменяет все управляющие низкоуровневые спецсимволы (с ASCII-кодом меньше 32, такие как \r, \n, \t) на обычные пробелы.
		/// </summary>
		/// <param name="value">Исходная строка для санитаризации.</param>
		/// <returns>Строка с нормализованными безопасными символами, либо <see cref="string.Empty"/>, если на входе пустая строка.</returns>
		public static string GetFixSpecChars(
			string value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			var sb1 = new StringBuilder(value.Length);
			foreach (var char1 in value)
				sb1.Append(char1 < 32 ? ' ' : char1);
			return sb1.ToString();
		}


		/// <summary>
		/// Возвращает полностью безопасную алфавитно-цифровую строку в нижнем регистре (только латиница и цифры), пригодную для кодов и ЧПУ.
		/// </summary>
		/// <remarks>
		/// Метод предварительно транслитерирует кириллицу по ГОСТ, заменяет любые посторонние спецсимволы дефисами, схлопывает их дубликаты и очищает края строки.
		/// </remarks>
		/// <param name="source">Исходная строка для трансформации.</param>
		/// <returns>Безопасная алфавитно-цифровая строка с дефисами или <see cref="string.Empty"/>.</returns>
		public static string GetSafeNumberString(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			var s1 = SuppLangRu.GetTranslitRuToEn(source.ToLowerInvariant());
			return s1
				.ReplaceByChars("-", "^0-9a-z")
				.GetReplaceRecursively("--", "-")
				.Trim('-');
		}


		/// <summary>
		/// Возвращает безопасную строку-идентификатор для кода. Заменяет пробелы и недопустимые знаки на символы подчеркивания.
		/// </summary>
		/// <remarks>
		/// Любые символы за рамками стандартной ASCII-таблицы (код больше 126) безопасно кодируются в hex-формат вида <c>x[код]</c>. Емкость внутреннего буфера рассчитывается экономично без лишних аллокаций.
		/// </remarks>
		/// <param name="source">Исходная строка.</param>
		/// <returns>Безопасная строка программного идентификатора, либо <see cref="string.Empty"/>.</returns>
		public static string GetSafeNameString(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			var s1 = SuppLangRu.GetTranslitRuToEn(source.ToLowerInvariant());
			var span1 = s1.AsSpan();
			var sb1 = _prepareSafe(span1);
			foreach (char ch1 in span1)
				switch (ch1)
				{
					case >= '0' and <= '9':
					case >= 'a' and <= 'z':
						sb1.Append(ch1);
						break;
					case > (char)126:
						sb1.Append('x').Append((int)ch1);
						break;
					default:
						sb1.Append('_');
						break;
				}
			return sb1.ToString().GetReplaceRecursively("__", "_");
		}


		/// <summary>
		/// Возвращает полностью безопасную строку для использования в файловой системе (в именах файлов или папок).
		/// </summary>
		/// <remarks>
		/// Очищает текст от запрещенных спецсимволов ОС, сохраняя знаки дефиса, подчеркивания, тильды и каретки, транслитерируя кириллицу и кодируя не-ASCII символы в формат hex.
		/// </remarks>
		/// <param name="source">Исходная строка.</param>
		/// <returns>Безопасная строка для путей файловой системы, либо <see cref="string.Empty"/>.</returns>
		public static string GetSafeFsString(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return string.Empty;
			var s1 = SuppLangRu.GetTranslitRuToEn(source.ToLowerInvariant());
			var span1 = s1.AsSpan();
			var sb1 = _prepareSafe(span1);
			foreach (char ch1 in span1)
				switch (ch1)
				{
					case >= '0' and <= '9':
					case >= 'a' and <= 'z':
					case '-':
					case '^':
					case '_':
					case '~':
						sb1.Append(ch1);
						break;
					case > (char)126:
						sb1.Append('x').Append((int)ch1);
						break;
					default:
						sb1.Append('_');
						break;
				}
			return sb1.ToString()
				.GetReplaceRecursively("__", "_")
				.GetReplaceRecursively("_-", "-")
				.GetReplaceRecursively("-_", "-")
				.GetReplaceRecursively("--", "-");
		}


		/* privates */


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static StringBuilder _prepareSafe(
			ReadOnlySpan<char> span)
		{
			int finalCapacity1 = 0;
			foreach (char ch1 in span)
			{
				if (ch1 > (char)126)
				{
					int code = (int)ch1;
					int digitCount = code switch
					{
						< 1000 => 3,
						< 10000 => 4,
						_ => 5
					};
					finalCapacity1 += 1 + digitCount;
				}
				else
					finalCapacity1 += 1;
			}
			return new StringBuilder(finalCapacity1);
		}

	}

}
