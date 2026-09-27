// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	public static partial class SuppLangRu
	{

		/* consts */


		/// <summary>
		/// Массив символов-разделителей, используемых при парсинге ФИО для разделения фамилии, имени и отчества.
		/// </summary>
		/// <value>Массив знаков, содержащий пробел, точку и запятую.</value>
		public static readonly char[] SEP_FAMIO = [' ', '.', ','];


		/* functions */


		/// <summary>
		/// Возвращает кортеж, содержащий фамилию и инициалы, из строки с полным именем (ФИО). 
		/// Автоматически исключает национальные патронимические приставки (оглы, кызы, улы, ибн, заде, паша).
		/// </summary>
		/// <param name="fullname">Исходная строка, содержащая фамилию, имя и отчество.</param>
		/// <param name="textCase">Тип операции преобразования регистра букв. По умолчанию <see cref="LetterCasesEnum.TitleCase"/>.</param>
		/// <returns>Кортеж, состоящий из изолированной строки фамилии и строки сформированных инициалов с точками (например, <c>"А.С."</c>).</returns>
		public static (string family, string initials) GetFamilyAndInitials(
			string fullname,
			LetterCasesEnum textCase = LetterCasesEnum.TitleCase)
		{
			if (string.IsNullOrEmpty(fullname))
				return (string.Empty, string.Empty);
			var span1 = fullname.AsSpan();
			var sepsSpan1 = SEP_FAMIO.AsSpan();
			int start1 = 0;
			while (start1 < span1.Length && sepsSpan1.Contains(span1[start1]))
				start1++;
			if (start1 >= span1.Length)
				return (string.Empty, string.Empty);
			int end1 = start1;
			while (end1 < span1.Length && !sepsSpan1.Contains(span1[end1]))
				end1++;
			string familyRaw1 = span1[start1..end1].ToString();
			string family1 = SuppString.GetModCase(familyRaw1, textCase);
			var sbInitials1 = new StringBuilder();
			start1 = end1;
			while (start1 < span1.Length)
			{
				while (start1 < span1.Length && sepsSpan1.Contains(span1[start1]))
					start1++;
				if (start1 >= span1.Length)
					break;
				end1 = start1;
				while (end1 < span1.Length && !sepsSpan1.Contains(span1[end1]))
					end1++;
				var word1 = span1[start1..end1];
				if (!word1.Equals("оглы", StringComparison.OrdinalIgnoreCase)
					&& !word1.Equals("кызы", StringComparison.OrdinalIgnoreCase)
					&& !word1.Equals("улы", StringComparison.OrdinalIgnoreCase)
					&& !word1.Equals("ибн", StringComparison.OrdinalIgnoreCase)
					&& !word1.Equals("заде", StringComparison.OrdinalIgnoreCase)
					&& !word1.Equals("паша", StringComparison.OrdinalIgnoreCase))
				{
					if (word1.Length > 0)
						sbInitials1.Append(char.ToUpperInvariant(word1[0])).Append('.');
				}
				start1 = end1;
			}
			var initials1 = SuppString.GetModCase(sbInitials1.ToString(), textCase);
			return (family1, initials1);
		}


		/// <summary>
		/// Возвращает объединенную строку, содержащую фамилию и инициалы, из строки с полным именем.
		/// </summary>
		/// <param name="fullname">Исходная строка с полным ФИО (например, <c>"Эфендиев Эльчин Ильяс оглы"</c>).</param>
		/// <param name="textCase">Тип операции преобразования регистра букв. По умолчанию <see cref="LetterCasesEnum.TitleCase"/>.</param>
		/// <returns>Строка стандартного формата "Фамилия И.О." (например, <c>"Эфендиев Э.И."</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFamilyAndInitialsString(
			string fullname,
			LetterCasesEnum textCase = LetterCasesEnum.TitleCase)
		{
			var (family1, initials1) = GetFamilyAndInitials(fullname, textCase);
			return $"{family1}{initials1.Make(" {0}")}";
		}


		/// <summary>
		/// Возвращает строку, содержащую латинскую транслитерацию ФИО для использования в составе уникального буквенного идентификатора (ГОСТ Р 7.0.34-2014).
		/// </summary>
		/// <param name="family">Строка фамилии.</param>
		/// <param name="initials">Строка инициалов.</param>
		/// <returns>Безопасный латинский идентификатор в нижнем регистре (например, <c>"efendiev_ei"</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFamilyAndInitialsTranslit(
			string family,
			string initials)
		{
			var sb1 = new StringBuilder($"{family.Replace("-", "")}");
			if (!string.IsNullOrEmpty(initials))
				sb1.Append($"_{initials.Replace(".", "")}");
			return GetTranslitRuToEn(
				sb1.ToString().ToLowerInvariant());
		}


		/// <summary>
		/// Заменяет ведущую междугороднюю восьмерку в начале номера телефона на международный префикс семерки (8999... переводит в 7999...).
		/// </summary>
		/// <param name="phone">Исходная строка номера телефона для нормализации.</param>
		/// <returns>Строка номера с замененным начальным кодом страны или исходная строка, если замена не требуется.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FixTelephoneRuCityCode(
			string phone)
		{
			if (string.IsNullOrEmpty(phone))
				return string.Empty;
			return phone[0] == '8'
				? $"7{phone.AsSpan(1)}" : phone;
		}


		/// <summary>
		/// Форматирует чистую последовательность цифр в удобочитаемый телефонный номер с дефисами. 
		/// Поддерживает длины от 5 до 11 символов. Префикс "+" добавляется автоматически только для полных 11-значных номеров.
		/// </summary>
		/// <param name="number">Строка, содержащая исключительно цифры номера телефона.</param>
		/// <returns>Форматированная строка телефонного номера (например, <c>"+7-999-123-45-67"</c> или <c>"322-45-67"</c>).</returns>
		public static string GetTelephoneNumber(
			string number)
		{
			if (string.IsNullOrEmpty(number))
				return string.Empty;
			int len1 = number.Length;
			if (len1 > 11 || len1 < 5)
				return number;
			int stop1 = len1 - 2;
			int stop2 = len1 - 4;
			int stop3 = len1 - 7;
			int stop4 = len1 - 10;
			Span<char> buffer1 = stackalloc char[16];
			int p1 = 0;
			if (len1 == 11 && number[0] != '+')
				buffer1[p1++] = '+';
			buffer1[p1++] = number[0];
			for (int i1 = 1; i1 < len1; i1++)
			{
				if (i1 == stop1 || i1 == stop2 || i1 == stop3 || i1 == stop4)
					buffer1[p1++] = '-';
				buffer1[p1++] = number[i1];
			}
			return new string(buffer1[..p1]);
		}


		/// <summary>
		/// Санитаризует и очищает номер документа, заменяя все нецифровые символы дефисами с последующим рекурсивным схлопыванием дубликатов и очисткой краев.
		/// </summary>
		/// <param name="number">Исходный номер документа, содержащий пробелы, спецсимволы или буквы.</param>
		/// <returns>Очищенный и нормализованный номер документа без ведущих и концевых знаков дефиса.</returns>
		public static string GetDocNumber(
			string number)
		{
			if (string.IsNullOrEmpty(number))
				return string.Empty;
			var sb1 = new StringBuilder(number.Length);
			foreach (var ch1 in number)
				if (ch1 >= '0' && ch1 <= '9')
					sb1.Append(ch1);
				else
					sb1.Append('-');
			return sb1.ToString()
				.GetReplaceRecursively("--", "-")
				.Trim('-');
		}


		/// <summary>
		/// Выполняет динамическую подстановку значений в строку почтового адреса по словарю маркеров. 
		/// Если непосредственно после замещаемого маркера идет цифра, в текст автоматически внедряется разделитель "каб.".
		/// </summary>
		/// <param name="address">Исходная строка адреса для модификации.</param>
		/// <param name="dict">Словарь заменяемых текстовых маркеров (ключ, включая двоеточие) и их значений.</param>
		/// <returns>Модифицированная итоговая строка адреса.</returns>
		public static string GetSubstitutionAddress(
			string address,
			Dictionary<string, string> dict)
		{
			if (string.IsNullOrEmpty(address) || dict == null || dict.Count == 0)
				return address ?? string.Empty;
			foreach (var item1 in dict)
			{
				string marker1 = $"{item1.Key}:";
				int index1 = address.IndexOf(marker1, StringComparison.Ordinal);
				if (index1 > -1)
				{
					int index2 = index1 + marker1.Length;
					string s1 = address[..index1] + item1.Value;
					string s2 = address[index2..];
					address = (index2 < address.Length && char.IsAsciiDigit(address[index2]))
						? $"{s1}, каб. {s2}" : $"{s1}, {s2}";
				}
			}
			return address;
		}

	}

}
