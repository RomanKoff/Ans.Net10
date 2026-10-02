// rev 2026-10-02

using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечисление кодировок текстовых файлов поддерживаемых инфраструктурой библиотеки.
	/// </summary>
	public enum EncodingsEnum
	{
		/// <summary>
		/// Кодировка UTF-8.
		/// </summary>
		UTF8,

		/// <summary>
		/// Кодировка Windows-1251 (Кириллица).
		/// </summary>
		WINDOWS1251,

		/// <summary>
		/// Кодировка KOI8-R (Кириллица).
		/// </summary>
		KOI8R,

		/// <summary>
		/// Кодировка CP866 (DOS-кириллица).
		/// </summary>
		CP866,

		/// <summary>
		/// Кодировка ISO-8859-1 (Западноевропейская).
		/// </summary>
		ISO88591
	}




	/// <summary>
	/// Вспомогательный класс для работы с операциями ввода-вывода (IO), хэшированием файлов и безопасными именами.
	/// </summary>
	public static partial class SuppIO
	{

		/* consts */


		/// <summary>
		/// Возвращает экземпляр кодировки UTF-8.
		/// </summary>
		public static readonly Encoding ENCODING_UTF8
			= Encoding.UTF8;


		/// <summary>
		/// Возвращает экземпляр кодировки Windows-1251 (Кириллица).
		/// </summary>
		public static readonly Encoding ENCODING_WINDOWS1251
			= Encoding.GetEncoding(1251);


		/// <summary>
		/// Возвращает экземпляр кодировки KOI8-R (Кириллица).
		/// </summary>
		public static readonly Encoding ENCODING_KOI8R
			= Encoding.GetEncoding(20866);


		/// <summary>
		/// Возвращает экземпляр кодировки CP866 (DOS-кириллица).
		/// </summary>
		public static readonly Encoding ENCODING_CP866
			= Encoding.GetEncoding(866);


		/// <summary>
		/// Возвращает экземпляр кодировки ISO-8859-1 (Западноевропейская).
		/// </summary>
		public static readonly Encoding ENCODING_ISO88591
			= Encoding.GetEncoding(28591);


		/// <summary>
		/// Список имен файлов, зарезервированных операционной системой Windows и запрещенных для использования.
		/// </summary>
		public static readonly string[] FORBIDDEN_FILE_NAMES =
		[
			"con", "prn", "aux", "nul",
			"com0", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
			"lpt0", "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"
		];


		/// <summary>
		/// Дефолтный объект информации о контенте для неопознанных бинарных данных.
		/// </summary>
		public static readonly ContentInfo CONTENTINFO_BIN
			= new("*", "application/octet-stream", ContentGroupEnum.Bin);


		/// <summary>
		/// Замороженный (Frozen) оптимизированный словарь сопоставления расширений файлов и метаданных типов контента.
		/// </summary>
		/// <remarks>
		/// Ключом словаря является расширение файла (обязательно включая ведущую точку, например, <c>".jpg"</c>). 
		/// Поиск по ключам производится в регистронезависимом режиме (<see cref="StringComparer.OrdinalIgnoreCase"/>).
		/// </remarks>
		/// <value>
		/// Оптимизированный для чтения словарь <see cref="FrozenDictionary{TKey, TValue}"/>, сопоставляющий 
		/// строковые расширения с объектами <see cref="ContentInfo"/>.
		/// </value>
		public static readonly FrozenDictionary<string, ContentInfo> CONTENTINFOS = new ContentInfo[]
		{
			// Archive
			new(".apk", "application/vnd.android.package-archive", ContentGroupEnum.Archive),
			new(".gtar", "application/x-gtar", ContentGroupEnum.Archive),
			new(".gz", "application/x-gzip", ContentGroupEnum.Archive),
			new(".tar", "application/x-tar", ContentGroupEnum.Archive),
			new(".tgz", "application/x-compressed", ContentGroupEnum.Archive),
			new(".z", "application/x-compress", ContentGroupEnum.Archive),
			new(".zip", "application/zip", ContentGroupEnum.Archive),
			// Document
			new(".accdb", "application/msaccess", ContentGroupEnum.Document),
			new(".ai", "application/postscript", ContentGroupEnum.Document),
			new(".doc", "application/msword", ContentGroupEnum.Document),
			new(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ContentGroupEnum.Document),
			new(".dot", "application/msword", ContentGroupEnum.Document),
			new(".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template", ContentGroupEnum.Document),
			new(".dvi", "application/x-dvi", ContentGroupEnum.Document),
			new(".eps", "application/postscript", ContentGroupEnum.Document),
			new(".hlp", "application/winhlp", ContentGroupEnum.Document),
			new(".latex", "application/x-latex", ContentGroupEnum.Document),
			new(".mdb", "application/x-msaccess", ContentGroupEnum.Document),
			new(".mpp", "application/vnd.ms-project", ContentGroupEnum.Document),
			new(".pdf", "application/pdf", ContentGroupEnum.Document),
			new(".pot", "application/vnd.ms-powerpoint", ContentGroupEnum.Document),
			new(".potx", "application/vnd.openxmlformats-officedocument.presentationml.template", ContentGroupEnum.Document),
			new(".pps", "application/vnd.ms-powerpoint", ContentGroupEnum.Document),
			new(".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow", ContentGroupEnum.Document),
			new(".ppt", "application/vnd.ms-powerpoint", ContentGroupEnum.Document),
			new(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation", ContentGroupEnum.Document),
			new(".ps", "application/postscript", ContentGroupEnum.Document),
			new(".pub", "application/x-mspublisher", ContentGroupEnum.Document),
			new(".rtf", "application/rtf", ContentGroupEnum.Document),
			new(".tex", "application/x-tex", ContentGroupEnum.Document),
			new(".wcm", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wdb", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wks", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wps", "application/vnd.ms-works", ContentGroupEnum.Document),
			new(".wri", "application/x-mswrite", ContentGroupEnum.Document),
			new(".xla", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xlc", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xlm", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xls", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ContentGroupEnum.Document),
			new(".xlt", "application/vnd.ms-excel", ContentGroupEnum.Document),
			new(".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template", ContentGroupEnum.Document),
			new(".xlw", "application/vnd.ms-excel", ContentGroupEnum.Document),
			// Image                        	   
			new(".bmp", "image/bmp", ContentGroupEnum.Image, isWebImage: true),
			new(".cmx", "image/x-cmx", ContentGroupEnum.Image),
			new(".cod", "image/cis-cod", ContentGroupEnum.Image),
			new(".gif", "image/gif", ContentGroupEnum.Image, isWebImage: true),
			new(".ico", "image/x-icon", ContentGroupEnum.Image),
			new(".ief", "image/ief", ContentGroupEnum.Image),
			new(".jfif", "image/pipeg", ContentGroupEnum.Image),
			new(".jpe", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true),
			new(".jpeg", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true),
			new(".jpg", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true),
			new(".pbm", "image/x-portable-bitmap", ContentGroupEnum.Image, isWebImage: true),
			new(".pgm", "image/x-portable-graymap", ContentGroupEnum.Image),
			new(".png", "image/png", ContentGroupEnum.Image, isWebImage: true),
			new(".pnm", "image/x-portable-anymap", ContentGroupEnum.Image),
			new(".ppm", "image/x-portable-pixmap", ContentGroupEnum.Image),
			new(".ras", "image/x-cmu-raster", ContentGroupEnum.Image),
			new(".rgb", "image/x-rgb", ContentGroupEnum.Image),
			new(".svg", "image/svg+xml", ContentGroupEnum.Image, isWebImage: true),
			new(".svgz", "image/svg+xml", ContentGroupEnum.Image, isWebImage: true),
			new(".tif", "image/tiff", ContentGroupEnum.Image, isWebImage: true),
			new(".tiff", "image/tiff", ContentGroupEnum.Image, isWebImage: true),
			new(".wbmp", "image/vnd.wap.wbmp", ContentGroupEnum.Image),
			new(".webp", "image/webp", ContentGroupEnum.Image, isWebImage: true),
			new(".xbm", "image/x-xbitmap", ContentGroupEnum.Image),
			new(".xpm", "image/x-xpixmap", ContentGroupEnum.Image),
			new(".xwd", "image/x-xwindowdump", ContentGroupEnum.Image),
			// Text                         	   
			new(".323", "text/h323", ContentGroupEnum.Text),
			new(".bas", "text/plain", ContentGroupEnum.Text),
			new(".c", "text/plain", ContentGroupEnum.Text),
			new(".cs", "text/plain", ContentGroupEnum.Text),
			new(".cshtml", "text/plain", ContentGroupEnum.Text),
			new(".css", "text/css", ContentGroupEnum.Text),
			new(".csv", "text/csv", ContentGroupEnum.Text),
			new(".etx", "text/x-setext", ContentGroupEnum.Text),
			new(".h", "text/plain", ContentGroupEnum.Text),
			new(".htc", "text/x-component", ContentGroupEnum.Text),
			new(".htm", "text/html", ContentGroupEnum.Text),
			new(".html", "text/html", ContentGroupEnum.Text),
			new(".htt", "text/webviewhtml", ContentGroupEnum.Text),
			new(".js", "application/javascript", ContentGroupEnum.Text),
			new(".json", "application/json", ContentGroupEnum.Text),
			new(".less", "text/css", ContentGroupEnum.Text),
			new(".rss", "application/rss+xml", ContentGroupEnum.Text),
			new(".rtx", "text/richtext", ContentGroupEnum.Text),
			new(".sass", "text/css", ContentGroupEnum.Text),
			new(".scss", "text/css", ContentGroupEnum.Text),
			new(".sct", "text/scriptlet", ContentGroupEnum.Text),
			new(".shtml", "text/html", ContentGroupEnum.Text),
			new(".stm", "text/html", ContentGroupEnum.Text),
			new(".tsv", "text/tab-separated-values", ContentGroupEnum.Text),
			new(".txt", "text/plain", ContentGroupEnum.Text),
			new(".uls", "text/iuls", ContentGroupEnum.Text),
			new(".vb", "text/plain", ContentGroupEnum.Text),
			new(".vcf", "text/x-vcard", ContentGroupEnum.Text),
			new(".xml", "application/xml", ContentGroupEnum.Text),
			// Audio                        	   
			new(".aif", "audio/x-aiff", ContentGroupEnum.Audio),
			new(".aifc", "audio/x-aiff", ContentGroupEnum.Audio),
			new(".aiff", "audio/x-aiff", ContentGroupEnum.Audio),
			new(".au", "audio/basic", ContentGroupEnum.Audio),
			new(".flac", "audio/flac", ContentGroupEnum.Audio),
			new(".m3u", "audio/x-mpegurl", ContentGroupEnum.Audio),
			new(".m4a", "audio/mp4", ContentGroupEnum.Audio),
			new(".mid", "audio/mid", ContentGroupEnum.Audio),
			new(".mp3", "audio/mpeg", ContentGroupEnum.Audio),
			new(".ogg", "audio/ogg", ContentGroupEnum.Audio),
			new(".ra", "audio/x-pn-realaudio", ContentGroupEnum.Audio),
			new(".ram", "audio/x-pn-realaudio", ContentGroupEnum.Audio),
			new(".rmi", "audio/mid", ContentGroupEnum.Audio),
			new(".snd", "audio/basic", ContentGroupEnum.Audio),
			new(".wav", "audio/x-wav", ContentGroupEnum.Audio),
			// Video                        	   
			new(".asf", "video/x-ms-asf", ContentGroupEnum.Video),
			new(".asr", "video/x-ms-asf", ContentGroupEnum.Video),
			new(".asx", "video/x-ms-asf", ContentGroupEnum.Video),
			new(".avi", "video/x-msvideo", ContentGroupEnum.Video),
			new(".f4v", "video/mp4", ContentGroupEnum.Video),
			new(".flv", "video/x-flv", ContentGroupEnum.Video),
			new(".lsf", "video/x-la-asf", ContentGroupEnum.Video),
			new(".lsx", "video/x-la-asf", ContentGroupEnum.Video),
			new(".mkv", "video/x-matroska", ContentGroupEnum.Video),
			new(".mov", "video/quicktime", ContentGroupEnum.Video),
			new(".movie", "video/x-sgi-movie", ContentGroupEnum.Video),
			new(".mp2", "video/mpeg", ContentGroupEnum.Video),
			new(".mp4", "video/mp4", ContentGroupEnum.Video),
			new(".mpa", "video/mpeg", ContentGroupEnum.Video),
			new(".mpe", "video/mpeg", ContentGroupEnum.Video),
			new(".mpeg", "video/mpeg", ContentGroupEnum.Video),
			new(".mpg", "video/mpeg", ContentGroupEnum.Video),
			new(".mpv2", "video/mpeg", ContentGroupEnum.Video),
			new(".ogv", "video/ogg", ContentGroupEnum.Video),
			new(".qt", "video/quicktime", ContentGroupEnum.Video),
			new(".webm", "video/webm", ContentGroupEnum.Video),
			// Fonts                        	   
			new(".eot", "application/vnd.ms-fontobject", ContentGroupEnum.Font),
			new(".otf", "font/otf", ContentGroupEnum.Font),
			new(".ttf", "font/ttf", ContentGroupEnum.Font),
			new(".woff", "font/woff", ContentGroupEnum.Font),
			new(".woff2", "font/woff2", ContentGroupEnum.Font)
		}.ToFrozenDictionary(x => x.Extension, StringComparer.OrdinalIgnoreCase);


		/* functions */


		/// <summary>
		/// Возвращает системный объект кодировки <see cref="Encoding"/> на основе выбранного значения из <see cref="EncodingsEnum"/>.
		/// </summary>
		/// <param name="encoding">Вариант кодировки из перечисления.</param>
		/// <returns>Экземпляр класса <see cref="Encoding"/>, соответствующий выбранному типу.</returns>
		public static Encoding GetEncoding(
			EncodingsEnum encoding)
		{
			return encoding switch
			{
				EncodingsEnum.WINDOWS1251 => ENCODING_WINDOWS1251,
				EncodingsEnum.KOI8R => ENCODING_KOI8R,
				EncodingsEnum.CP866 => ENCODING_CP866,
				EncodingsEnum.ISO88591 => ENCODING_ISO88591,
				_ => ENCODING_UTF8
			};
		}


		/// <summary>
		/// Генерация уникального имени: если файл с указанным именем существует, рекурсивно добавляет суффикс "_" перед расширением.
		/// </summary>
		/// <param name="file">Информационный объект исходного файла, используемый для определения целевой директории.</param>
		/// <param name="newName">Желаемое новое имя файла с расширением.</param>
		/// <returns>Строка, содержащая уникальный полный путь к файлу.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="file"/> равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Вызывается, если <paramref name="newName"/> пустая строка.</exception>
		public static string GetNewName(
			FileInfo file,
			string newName)
		{
			ArgumentNullException.ThrowIfNull(file);
			ArgumentException.ThrowIfNullOrEmpty(newName);
			var path1 = Path.Combine(file.DirectoryName ?? string.Empty, newName);
			if (!File.Exists(path1))
				return path1;
			var nameOnly1 = Path.GetFileNameWithoutExtension(path1);
			var ext1 = Path.GetExtension(path1);
			return GetNewName(file, $"{nameOnly1}_{ext1}");
		}


		/// <summary>
		/// Возвращает расширение файла из указанного пути в нижнем регистре.
		/// </summary>
		/// <param name="path">Путь к файлу или имя файла.</param>
		/// <param name="hasDot">Признак необходимости сохранения точки перед расширением (например, <c>".txt"</c> вместо <c>"txt"</c>).</param>
		/// <returns>Строка расширения в нижнем регистре. Если расширение отсутствует, возвращает пустую строку.</returns>
		/// <exception cref="ArgumentException">Вызывается, если <paramref name="path"/> пустая строка.</exception>
		public static string GetFileExtension(
			string path,
			bool hasDot)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			var s1 = Path.GetExtension(path).ToLowerInvariant();
			if (string.IsNullOrEmpty(s1))
				return string.Empty;
			return hasDot
				? s1 : s1[1..];
		}


		/// <summary>
		/// Разделяет имя файла на две части: имя без расширения и само расширение (включая точку).
		/// </summary>
		/// <param name="filename">Имя файла для разделения.</param>
		/// <returns>Массив из двух элементов: <c>[имя_без_расширения, расширение]</c>. Если входная строка равна <see langword="null"/>, возвращает пустой массив.</returns>
		public static string[] GetFilenameHalfs(
			string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return [];
			int i1 = filename.LastIndexOf('.');
			return i1 == -1
				? [filename, string.Empty]
				: [filename[..i1], filename[i1..]];
		}


		/// <summary>
		/// Регистронезависимый быстрый поиск метаданных контента <see cref="ContentInfo"/> по расширению файла.
		/// </summary>
		/// <param name="extension">Расширение файла (с ведущей точкой или без неё).</param>
		/// <returns>Объект метаданных контента; если расширение неизвестно, возвращается дефолтный бинарный тип <see cref="CONTENTINFO_BIN"/>.</returns>
		public static ContentInfo GetContentInfoFromExtension(
			string extension)
		{
			if (string.IsNullOrEmpty(extension))
				return CONTENTINFO_BIN;
			string key1 = extension[0] == '.'
				? extension
				: string.Concat(".", extension);
			return CONTENTINFOS.TryGetValue(key1, out var info1)
				? info1
				: CONTENTINFO_BIN;
		}


		/// <summary>
		/// Поиск метаданных контента <see cref="ContentInfo"/> по полному пути или имени файла.
		/// </summary>
		/// <param name="path">Путь к файлу или его имя.</param>
		/// <returns>Объект метаданных контента.</returns>
		public static ContentInfo GetContentInfoFromPath(
			string path)
		{
			return GetContentInfoFromExtension(
				GetFileExtension(path, true));
		}


		/// <summary>
		/// Асинхронно считывает весь текстовый контент из файла по указанному пути, используя выбранную кодировку.
		/// </summary>
		/// <param name="path">Путь к файлу для чтения.</param>
		/// <param name="encoding">Кодировка текста (по умолчанию <see cref="EncodingsEnum.UTF8"/>).</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Поток-задача, возвращающая текстовое содержимое файла.</returns>
		/// <exception cref="ArgumentException">Вызывается, если <paramref name="path"/> пустая строка.</exception>
		public static async Task<string> FileReadAsync(
			string path,
			EncodingsEnum encoding = EncodingsEnum.UTF8,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			using var fs1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			using var sr1 = new StreamReader(
				fs1, GetEncoding(encoding));
			return await sr1.ReadToEndAsync(cancellationToken);
		}


		/// <summary>
		/// Асинхронно считывает начало файла из открытого потока до указанного размера и возвращает его в виде Base64-строки для быстрого анализа сигнатур.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="size">Максимальное количество байт для чтения. По умолчанию равно 255.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Строка в формате Base64, представляющая начало файла.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="stream"/> равен <see langword="null"/>.</exception>
		public static async Task<string> GetFileBeginAsync(
			Stream stream,
			int size = 255,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(stream);
			if (size <= 0)
				return string.Empty;
			byte[] buffer1 = new byte[size];
			int i1 = await stream.ReadAsync(
				buffer1.AsMemory(0, size), cancellationToken);
			if (i1 <= 0)
				return string.Empty;
			return Convert.ToBase64String(buffer1, 0, i1);
		}


		/// <summary>
		/// Асинхронно считывает начало файла по указанному пути на диске до указанного размера и возвращает его в виде Base64-строки.
		/// </summary>
		/// <param name="path">Путь к файлу.</param>
		/// <param name="size">Максимальное количество байт для чтения. По умолчанию равно 255.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Строка в формате Base64, представляющая начало файла.</returns>
		/// <exception cref="ArgumentException">Вызывается, если <paramref name="path"/> пустая строка.</exception>
		public static async Task<string> GetFileBeginAsync(
			string path,
			int size = 255,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			using var stream1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			return await GetFileBeginAsync(stream1, size, cancellationToken);
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA1 хэш для указанного потока файла с использованием байтовой соли без рантайм-аллокаций.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static async Task<string> GetFileSHA1Async(
			Stream stream,
			byte[] salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(stream);
			ArgumentNullException.ThrowIfNull(salt);
			byte[] hash1 = await HMACSHA1.HashDataAsync(salt, stream, cancellationToken);
			return Convert.ToHexString(hash1).ToLowerInvariant();
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA1 хэш для указанного потока файла с использованием строковой соли.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static Task<string> GetFileSHA1Async(
			Stream stream,
			string salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(salt);
			return GetFileSHA1Async(
				stream, Encoding.Unicode.GetBytes(salt), cancellationToken);
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA1 хэш для файла по указанному пути с использованием байтовой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static async Task<string> GetFileSHA1Async(
			string path,
			byte[] salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			using var stream1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			return await GetFileSHA1Async(stream1, salt, cancellationToken);
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA1 хэш для файла по указанному пути с использованием строковой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static Task<string> GetFileSHA1Async(
			string path,
			string salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(salt);
			return GetFileSHA1Async(
				path, Encoding.Unicode.GetBytes(salt), cancellationToken);
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA256 хэш для указанного потока файла с использованием байтовой соли без рантайм-аллокаций.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static async Task<string> GetFileSHA256Async(
			Stream stream,
			byte[] salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(stream);
			ArgumentNullException.ThrowIfNull(salt);
			byte[] hash1 = await HMACSHA256.HashDataAsync(salt, stream, cancellationToken);
			return Convert.ToHexString(hash1).ToLowerInvariant();
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA256 хэш для указанного потока файла с использованием строковой соли.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static Task<string> GetFileSHA256Async(
			Stream stream,
			string salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(salt);
			return GetFileSHA256Async(
				stream, Encoding.Unicode.GetBytes(salt), cancellationToken);
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA256 хэш для файла по указанному пути с использованием байтовой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static async Task<string> GetFileSHA256Async(
			string path,
			byte[] salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			using var stream1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			return await GetFileSHA256Async(stream1, salt, cancellationToken);
		}


		/// <summary>
		/// Асинхронно вычисляет HMAC-SHA256 хэш для файла по указанному пути с использованием строковой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static Task<string> GetFileSHA256Async(
			string path,
			string salt,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(salt);
			return GetFileSHA256Async(
				path, Encoding.Unicode.GetBytes(salt), cancellationToken);
		}


		/// <summary>
		/// Возвращает время последнего изменения файла с автоматическим приведением к типу <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <param name="filename">Полный или относительный путь к файлу.</param>
		/// <returns>Объект структуры <see cref="DateTimeOffset"/>.</returns>
		public static DateTimeOffset GetFileLastModified(
			string filename)
		{
			return new DateTimeOffset(
				File.GetLastWriteTimeUtc(filename));
		}


		/// <summary>
		/// Возвращает максимальное (самое последнее) время модификации файлов, находящихся непосредственно в указанном каталоге.
		/// </summary>
		/// <param name="directory">Информационный объект исследуемого каталога <see cref="DirectoryInfo"/>.</param>
		/// <returns>Объект <see cref="DateTime"/>, соответствующий дате самого свежего файла.</returns>
		public static DateTime GetLastWriteTimeFiles(
			DirectoryInfo directory)
		{
			ArgumentNullException.ThrowIfNull(directory);
			var date1 = directory.LastWriteTime;
			foreach (var item1 in directory.GetFiles())
				if (item1.LastWriteTime > date1)
					date1 = item1.LastWriteTime;
			return date1;
		}


		/// <summary>
		/// Возвращает строковое представление размера данных в КБ, округленное в большую сторону (например, для вывода в метаданных скачивания).
		/// </summary>
		/// <param name="length">Размер исследуемого контента в байтах.</param>
		/// <returns>Строка отформатированного размера с суффиксом КБ.</returns>
		public static string GetLengthOfKB(
			long length)
		{
			long l1 = 1;
			if (length >= 1024)
				l1 = (length + 1023) / 1024;
			return l1.ToString(Resources.Common.Format_LengthKB).TrimStart();
		}


		/// <summary>
		/// Возвращает строковое представление размера данных в КБ для 32-битного целочисленного аргумента.
		/// </summary>
		/// <param name="length">Размер контента в байтах.</param>
		/// <returns>Строка отформатированного размера с суффиксом КБ.</returns>
		public static string GetLengthOfKB(
			int length)
		{
			return GetLengthOfKB((long)length);
		}


		/// <summary>
		/// Экранирует зарезервированные операционной системой Windows имена файлов (например, <c>CON</c>, <c>PRN</c>, <c>AUX</c>), оборачивая их в символы подчеркивания.
		/// </summary>
		/// <param name="name">Проверяемое имя файла или его расширение.</param>
		/// <returns>Защищенное строковое имя файла.</returns>
		public static string FixForbiddenFileName(
			string name)
		{
			return name.Length < 5 && FORBIDDEN_FILE_NAMES.Contains(name)
				? $"_{name}_" : name;
		}


		/// <summary>
		/// Возвращает нормализованное безопасное расширение файла в нижнем регистре без точки, приводя редкие вариации к стандартным (<c>jpeg</c> в <c>jpg</c>).
		/// </summary>
		/// <param name="filename">Имя файла или путь.</param>
		/// <returns>Нормализованное строковое расширение без ведущей точки.</returns>
		public static string GetSafeFileExtension(
			string filename)
		{
			var s1 = GetFileExtension(filename, false);
			var s2 = FixForbiddenFileName(s1);
			var s3 = SuppString.GetSafeFsString(s2);
			return s3 switch
			{
				"jpeg" => "jpg",
				"jpe" => "jpg",
				"mpeg" => "mpg",
				"tiff" => "tif",
				_ => s3
			};
		}


		/// <summary>
		/// Возвращает безопасное имя файла без расширения. Если имя превышает лимит в 80 символов, сокращает его и подмешивает уникальный хэш от оригинального имени.
		/// </summary>
		/// <param name="filename">Исходное имя файла или путь.</param>
		/// <returns>Очищенная строка безопасного имени.</returns>
		public static string GetSafeFileNameWithoutExtension(
			string filename)
		{
			var s1 = Path.GetFileNameWithoutExtension(filename);
			var s2 = FixForbiddenFileName(s1);
			var s3 = SuppString.GetSafeFsString(s2);
			if (s3.Length > 80)
				s3 = $"{s3[..38]}{SuppCrypto.ComputeSha256(s1)}";
			return s3;
		}


		/// <summary>
		/// Формирует полностью безопасное и валидное имя файла с расширением, очищенное от запрещенных спецсимволов и системных ограничений Windows.
		/// </summary>
		/// <param name="filename">Исходное сырое имя файла.</param>
		/// <returns>Полностью безопасное имя файла, готовое к записи на диск.</returns>
		public static string GetSafeFilename(
			string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return string.Empty;
			var ext1 = GetSafeFileExtension(filename);
			var name1 = GetSafeFileNameWithoutExtension(filename);
			return $"{name1}.{ext1}";
		}


		/// <summary>
		/// Быстрая проверка: содержит ли указанный путь запрещенные символы файловой системы.
		/// </summary>
		/// <param name="path">Проверяемый путь.</param>
		/// <returns><see langword="true"/>, если в пути обнаружены невалидные знаки; в противном случае — <see langword="false"/>.</returns>
		public static bool HasInvalidPathChars(
			string path)
		{
			return path.IndexOfAny(Path.GetInvalidPathChars()) >= 0;
		}


		/// <summary>
		/// Быстрая проверка: содержит ли указанное имя файла невалидные знаки.
		/// </summary>
		/// <param name="filename">Проверяемое имя файла.</param>
		/// <returns><see langword="true"/>, если имя содержит запрещенные символы; в противном случае — <see langword="false"/>.</returns>
		public static bool HasInvalidFileNameChars(
			string filename)
		{
			return filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
		}


		/// <summary>
		/// Выполняет полное глубокое побайтовое сравнение содержимого двух файлов на диске в асинхронном неблокирующем режиме. 
		/// Различия в путях или именах файлов игнорируются.
		/// </summary>
		/// <param name="file1">Первый сравниваемый файл.</param>
		/// <param name="file2">Второй сравниваемый файл.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом которой является <see langword="true"/>, если содержимое файлов абсолютно побайтово совпадает; в противном случае — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если один из параметров равен <see langword="null"/>.</exception>
		public static async Task<bool> IsFilesEqualFullAsync(
			FileInfo file1,
			FileInfo file2,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(file1);
			ArgumentNullException.ThrowIfNull(file2);
			if (file1.Length != file2.Length)
				return false;
			if (string.Equals(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase))
				return true;
			const int _BUF_SIZE1 = 4096;
			var options1 = new FileStreamOptions
			{
				Mode = FileMode.Open,
				Access = FileAccess.Read,
				Share = FileShare.Read,
				Options = FileOptions.Asynchronous
			};
			using var stream1 = new FileStream(file1.FullName, options1);
			using var stream2 = new FileStream(file2.FullName, options1);
			byte[] buffer1 = new byte[_BUF_SIZE1];
			byte[] buffer2 = new byte[_BUF_SIZE1];
			while (true)
			{
				int count1 = await stream1.ReadAsync(buffer1.AsMemory(0, _BUF_SIZE1), cancellationToken).ConfigureAwait(false);
				int count2 = await stream2.ReadAsync(buffer2.AsMemory(0, _BUF_SIZE1), cancellationToken).ConfigureAwait(false);
				if (count1 != count2)
					return false;
				if (count1 == 0)
					break;
				if (!buffer1.AsSpan(0, count1).SequenceEqual(buffer2.AsSpan(0, count2)))
					return false;
			}
			return true;
		}


		/// <summary>
		/// Выполняет быстрое «ленивое» асинхронное сравнение двух файлов по их метаданным (размеру и дате изменения). 
		/// Если они совпадают, возвращает <see langword="true"/>; иначе перепроверяет побайтово через <see cref="IsFilesEqualFullAsync"/>.
		/// </summary>
		/// <param name="file1">Первый сравниваемый файл.</param>
		/// <param name="file2">Второй сравниваемый файл.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом которой является <see langword="true"/>, если файлы идентичны по метаданным или содержимому; в противном случае — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если один из параметров равен <see langword="null"/>.</exception>
		public static async Task<bool> IsFilesEqualLazyAsync(
			FileInfo file1,
			FileInfo file2,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(file1);
			ArgumentNullException.ThrowIfNull(file2);

			if (file1.Length == file2.Length && file1.LastWriteTimeUtc == file2.LastWriteTimeUtc)
				return true;
			return await IsFilesEqualFullAsync(file1, file2, cancellationToken).ConfigureAwait(false);
		}


		/* methods */


		/// <summary>
		/// Создает директорию по указанному пути, если она еще отсутствует в системе.
		/// </summary>
		/// <param name="path">Путь к создаваемой директории.</param>
		public static void CreateDirectoryIfNotExists(
			string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}


		/// <summary>
		/// Удаляет директорию и все ее внутреннее содержимое (рекурсивно) по указанному пути, если она существует.
		/// </summary>
		/// <param name="path">Путь к удаляемой директории.</param>
		public static void DeleteDirectoryIfExists(
			string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);
		}


		/// <summary>
		/// Удаляет файл по указанному пути на диске, если он существует.
		/// </summary>
		/// <param name="path">Путь к удаляемому файлу.</param>
		public static void DeleteFileIfExists(
			string path)
		{
			if (File.Exists(path))
				File.Delete(path);
		}


		/// <summary>
		/// Переименовывает файл, безопасно перемещая его. Если целевое имя уже занято, автоматически подбирает уникальное имя, добавляя нижнее подчеркивание.
		/// </summary>
		/// <param name="file">Переименовываемый информационный объект файла.</param>
		/// <param name="newName">Новое желаемое имя файла.</param>
		public static void Rename(
			FileInfo file,
			string newName)
		{
			var uniquePath = GetNewName(file, newName);
			file.MoveTo(uniquePath);
		}


		/// <summary>
		/// Асинхронно записывает строковый контент в файл по указанному пути, используя выбранную кодировку <see cref="EncodingsEnum"/> и режим открытия файлового потока.
		/// </summary>
		/// <param name="path">Путь к целевому файлу для записи.</param>
		/// <param name="content">Строковое содержимое, подлежащее сохранению.</param>
		/// <param name="encoding">Кодировка текста. По умолчанию используется <see cref="EncodingsEnum.UTF8"/>.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <param name="mode">Режим работы файлового потока. По умолчанию используется <see cref="FileMode.Create"/> (перезапись/создание нового).</param>
		public static async Task FileWriteAsync(
			string path,
			string content,
			EncodingsEnum encoding = EncodingsEnum.UTF8,
			FileMode mode = FileMode.Create,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			using var fs1 = new FileStream(
				path, mode, FileAccess.Write, FileShare.None, 4096, useAsync: true);
			using var sw1 = new StreamWriter(fs1, GetEncoding(encoding));
			await sw1.WriteAsync(content.AsMemory(), cancellationToken);
		}


		/// <summary>
		/// Асинхронно записывает сырой массив байт в файл по указанному пути, используя выбранный режим открытия файлового потока.
		/// </summary>
		/// <param name="path">Путь к файлу для записи.</param>
		/// <param name="content">Массив байт, который необходимо записать в файл.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <param name="mode">Режим работы файлового потока. По умолчанию используется <see cref="FileMode.Create"/>.</param>
		public static async Task FileWriteAsync(
			string path,
			byte[] content,
			FileMode mode = FileMode.Create,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			using var fs1 = new FileStream(
				path, mode, FileAccess.Write, FileShare.None, 4096, useAsync: true);
			await fs1.WriteAsync(content.AsMemory(), cancellationToken);
		}

	}

}
