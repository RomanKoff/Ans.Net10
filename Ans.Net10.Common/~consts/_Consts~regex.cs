// rev 2026-09-25

using System.Text.RegularExpressions;

namespace Ans.Net10.Common
{

	public static partial class _Consts
	{

		/// <summary>
		/// Шаблон регулярного выражения для проверки стандартных строковых имен
		/// (строчные буквы, цифры, дефис, подчеркивание, точка).
		/// </summary>
		public const string REGEX_NAME
			= @"^([a-z_][0-9a-z._-]+)$";

		/// <summary>
		/// Строгий шаблон регулярного выражения для проверки имен (без точек).
		/// </summary>
		public const string REGEX_NAME_STRICT
			= @"^([a-z_][0-9a-z_-]+)$";

		/// <summary>
		/// Шаблон для проверки имен переменных (разрешен любой регистр и подчеркивание).
		/// </summary>
		public const string REGEX_VARNAME
			= @"^([a-zA-Z_][0-9a-zA-Z_]+)$";

		/// <summary>
		/// Строгий шаблон регулярного выражения для проверки имен переменных в нижнем регистре.
		/// </summary>
		public const string REGEX_VARNAME_STRICT
			= @"^([a-z_][0-9a-z_]+)$";

		/// <summary>
		/// Шаблон для проверки путей внутренних идентификаторов (через слэш, любой регистр).
		/// </summary>
		public const string REGEX_IPATH
			= @"^(([a-zA-Z_][0-9a-zA-Z_-]+)/){0,}([a-zA-Z_][0-9a-zA-Z_-]+)$";

		/// <summary>
		/// Строгий шаблон для проверки путей внутренних идентификаторов в нижнем регистре.
		/// </summary>
		public const string REGEX_IPATH_STRICT
			= @"^(([a-z_][0-9a-z_-]+)/){0,}([a-z_][0-9a-z_-]+)$";

		/// <summary>
		/// Базовый шаблон регулярного выражения для проверки адресов электронной почты.
		/// </summary>
		public const string REGEX_EMAIL
			= @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

		/// <summary>
		/// Строгий шаблон регулярного выражения стандарта проверки адресов электронной почты.
		/// </summary>
		public const string REGEX_EMAIL_STRICT
			= @"^(?("")(""[^""]+""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

		/// <summary>
		/// Шаблон для проверки имен узлов (строчные буквы, цифры, дефис, подчеркивание).
		/// </summary>
		public const string REGEX_NODENAME
			= @"^([0-9a-z_-]+)$";

		/// <summary>
		/// Шаблон для проверки системных имен страниц (идентичен REGEX_NODENAME).
		/// </summary>
		public const string REGEX_PAGENAME
			= REGEX_NODENAME;

		/// <summary>
		/// Шаблон регулярного выражения для поиска URL-адресов.
		/// </summary>
		public const string REGEX_URL
			= @"(https?://)[-_./:\?=&%#a-zA-Z0-9]+";

		/// <summary>
		/// Шаблон регулярного выражения для валидации и поиска IP-адресов версии 4.
		/// </summary>
		public const string REGEX_IP4
			= @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";			

		/// <summary>
		/// Шаблон регулярного выражения для базовой валидации символьного состава паролей.
		/// </summary>
		public const string REGEX_PASSWORD
			= @"^([0-9a-zA-Z `\'"":;,\.<>+=\~_\?\!\@#\$%\^\&\*\(\)\[\]\{\}/\\\|-]+)$";

		/// <summary>
		/// Шаблон для проверки ФИО на русском и английском языках (с поддержкой дефисов).
		/// </summary>
		public const string REGEX_FIO_RU
			= @"^[A-Za-zА-ЯЁа-яё-]{1,23}( [A-Za-zА-ЯЁа-яё-]{1,23})+$";

		/// <summary>
		/// Шаблон для извлечения имен HTML/XML тегов.
		/// </summary>
		public const string REGEX_TAGS
			= @"(?<=</?)([^ >/]+)";

		/// <summary>
		/// Шаблон регулярного выражения для поиска хэштегов (знак # и алфавитно-цифровой набор).
		/// </summary>
		public const string REGEX_SHARP_TAGS
			= @"#[-_a-zA-Zа-яА-Я0-9]+";

		/// <summary>
		/// Шаблон регулярного выражения для поиска ссылок, строго привязанных к началу строки.
		/// </summary>
		public const string REGEX_LINKS
			= @"^(https?://)[-_./:\\?=&%#a-zA-Z0-9]+";



		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для проверки стандартных строковых имен.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_NAME"/>.</returns>
		[GeneratedRegex(REGEX_NAME)]
		public static partial Regex G_REGEX_NAME();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для строгой проверки имен (без точек).
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_NAME_STRICT"/>.</returns>
		[GeneratedRegex(REGEX_NAME_STRICT)]
		public static partial Regex G_REGEX_NAME_STRICT();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для проверки имен переменных.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_VARNAME"/>.</returns>
		[GeneratedRegex(REGEX_VARNAME)]
		public static partial Regex G_REGEX_VARNAME();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для строгой проверки имен переменных в нижнем регистре.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_VARNAME_STRICT"/>.</returns>
		[GeneratedRegex(REGEX_VARNAME_STRICT)]
		public static partial Regex G_REGEX_VARNAME_STRICT();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для проверки путей внутренних идентификаторов в любом регистре.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_IPATH"/>.</returns>
		[GeneratedRegex(REGEX_IPATH)]
		public static partial Regex G_REGEX_IPATH();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для строгой проверки путей внутренних идентификаторов в нижнем регистре.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_IPATH_STRICT"/>.</returns>
		[GeneratedRegex(REGEX_IPATH_STRICT)]
		public static partial Regex G_REGEX_IPATH_STRICT();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для регистронезависимой проверки адресов электронной почты.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_EMAIL"/>.</returns>
		[GeneratedRegex(REGEX_EMAIL, RegexOptions.IgnoreCase)]
		public static partial Regex G_REGEX_EMAIL();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для строгого регистронезависимого соответствия адресов электронной почты.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_EMAIL_STRICT"/>.</returns>
		[GeneratedRegex(REGEX_EMAIL_STRICT, RegexOptions.IgnoreCase)]
		public static partial Regex G_REGEX_EMAIL_STRICT();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для проверки имен узлов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_NODENAME"/>.</returns>
		[GeneratedRegex(REGEX_NODENAME)]
		public static partial Regex G_REGEX_NODENAME();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для проверки системных имен страниц.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_PAGENAME"/>.</returns>
		[GeneratedRegex(REGEX_PAGENAME)]
		public static partial Regex G_REGEX_PAGENAME();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска и извлечения URL-адресов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_URL"/>.</returns>
		[GeneratedRegex(REGEX_URL)]
		public static partial Regex G_REGEX_URL();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для валидации и поиска IP-адресов версии 4.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_IP4"/>.</returns>
		[GeneratedRegex(REGEX_IP4)]
		public static partial Regex G_REGEX_IP4();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для валидации базового символьного состава паролей.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_PASSWORD"/>.</returns>
		[GeneratedRegex(REGEX_PASSWORD)]
		public static partial Regex G_REGEX_PASSWORD();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для проверки ФИО на русском и английском языках.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_FIO_RU"/>.</returns>
		[GeneratedRegex(REGEX_FIO_RU)]
		public static partial Regex G_REGEX_FIO_RU();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для извлечения имен HTML/XML тегов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_TAGS"/>.</returns>
		[GeneratedRegex(REGEX_TAGS)]
		public static partial Regex G_REGEX_TAGS();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска хэштегов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_SHARP_TAGS"/>.</returns>
		[GeneratedRegex(REGEX_SHARP_TAGS)]
		public static partial Regex G_REGEX_SHARP_TAGS();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска ссылок в начале строки.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> на основе шаблона <see cref="REGEX_LINKS"/>.</returns>
		[GeneratedRegex(REGEX_LINKS)]
		public static partial Regex G_REGEX_LINKS();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска исключительно цифровых символов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> для сопоставления с одиночными цифрами.</returns>
		[GeneratedRegex(@"\d")]
		public static partial Regex G_REGEX_ONLY_NUMBER();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска любых нецифровых символов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> для сопоставления со всеми символами, кроме цифр.</returns>
		[GeneratedRegex(@"\D")]
		public static partial Regex G_REGEX_NOT_NUMBER();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска последовательностей из двух и более пробельных символов.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> для сопоставления со множественными пробелами.</returns>
		[GeneratedRegex(@"[\s]{2,}")]
		public static partial Regex G_REGEX_MULTISPACE();

		/// <summary>
		/// Возвращает скомпилированное регулярное выражение для поиска коротких чисел от 1 до 3 знаков, включая знаки препинания и разделители.
		/// </summary>
		/// <returns>Экземпляр <see cref="Regex"/> для сопоставления с короткими числовыми диапазонами.</returns>
		[GeneratedRegex(@"([0-9.,_-]{1,3})")]
		public static partial Regex G_REGEX_SMALLNUMBER();

	}

}
