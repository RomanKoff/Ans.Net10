// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень кодировок текстовых файлов поддерживаемых инфраструктурой библиотеки.
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
	public static class SuppIO
	{

		//public static string GetCatalogName(
		//	int id)
		//{
		//	return $"{id:0:000/00/00}"; // string.Format("{0:000/00/00}", id);
		//}

		//public static void Register_CodePagesEncodingProvider()
		//{
		//	Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		//}


		/* functions */


		/// <summary>
		/// Возвращает системный объект кодировки <see cref="Encoding"/> на основе выбранного значения из <see cref="EncodingsEnum"/>.
		/// </summary>
		/// <param name="encoding">Вариант кодировки из перечисления.</param>
		/// <returns>Экземпляр класса <see cref="Encoding"/>, соответствующий выбранному типу.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Encoding GetEncoding(
			EncodingsEnum encoding)
		{
			return encoding switch
			{
				EncodingsEnum.WINDOWS1251 => _Consts.ENCODING_WINDOWS1251,
				EncodingsEnum.KOI8R => _Consts.ENCODING_KOI8R,
				EncodingsEnum.CP866 => _Consts.ENCODING_CP866,
				EncodingsEnum.ISO88591 => _Consts.ENCODING_ISO88591,
				_ => _Consts.ENCODING_UTF8
			};
		}


		/// <summary>
		/// Генерация уникального имени: если файл с указанным именем существует, рекурсивно добавляет суффикс "_" перед расширением.
		/// </summary>
		/// <param name="file">Информационный объект исходного файла, используемый для определения целевой директории.</param>
		/// <param name="newName">Желаемое новое имя файла с расширением.</param>
		/// <returns>Строка, содержащая уникальный полный путь к файлу.</returns>
		public static string GetNewName(
			FileInfo file,
			string newName)
		{
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileExtension(
			string path,
			bool hasDot)
		{
			var s1 = Path.GetExtension(path).ToLowerInvariant();
			if (string.IsNullOrEmpty(s1))
				return string.Empty;
			return hasDot ? s1 : s1[1..];
		}


		/// <summary>
		/// Разделяет имя файла на две части: имя без расширения и само расширение (включая точку).
		/// </summary>
		/// <param name="filename">Имя файла для разделения.</param>
		/// <returns>Массив из двух элементов: <c>[имя_без_расширения, расширение]</c>. Если входная строка равна <see langword="null"/>, возвращает пустой массив.</returns>
		public static string[] GetFilenameHalfs(
			string filename)
		{
			if (filename == null)
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
		/// <returns>Объект метаданных контента; если расширение неизвестно, возвращается дефолтный бинарный тип <see cref="_Consts.CONTENTINFO_BIN"/>.</returns>
		public static ContentInfo GetContentInfoFromExtension(
			string extension)
		{
			if (string.IsNullOrEmpty(extension))
				return _Consts.CONTENTINFO_BIN;
			string key1 = extension[0] == '.'
				? extension
				: string.Concat(".", extension);
			return _Consts.CONTENTINFOS.TryGetValue(key1, out var info1)
				? info1
				: _Consts.CONTENTINFO_BIN;
		}


		/// <summary>
		/// Поиск метаданных контента <see cref="ContentInfo"/> по полному пути или имени файла.
		/// </summary>
		/// <param name="path">Путь к файлу или его имя.</param>
		/// <returns>Объект метаданных контента.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ContentInfo GetContentInfoFromPath(
			string path)
		{
			return GetContentInfoFromExtension(
				GetFileExtension(path, true));
		}


		/// <summary>
		/// Считывает весь текстовый контент из файла по указанному пути, используя выбранную кодировку.
		/// </summary>
		/// <param name="path">Путь к файлу для чтения.</param>
		/// <param name="encoding">Кодировка текста (по умолчанию <see cref="EncodingsEnum.UTF8"/>).</param>
		/// <returns>Строка, содержащая весь текст из файла.</returns>
		public static string FileRead(
			string path,
			EncodingsEnum encoding = EncodingsEnum.UTF8)
		{
			using var fs1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read);
			using var sr1 = new StreamReader(
				fs1, GetEncoding(encoding));
			return sr1.ReadToEnd();
		}


		/// <summary>
		/// Считывает начало файла из открытого потока до указанного размера и возвращает его в виде Base64-строки для быстрого анализа сигнатур (Magic Numbers).
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="size">Максимальное количество байт для чтения. По умолчанию равно 255.</param>
		/// <returns>Строка в формате Base64, представляющая начало файла.</returns>
		public static string GetFileBegin(
			FileStream stream,
			int size = 255)
		{
			if (size <= 0)
				return string.Empty;
			byte[] buffer1 = new byte[size];
			int i1 = stream.Read(buffer1, 0, size);
			if (i1 <= 0)
				return string.Empty;
			return Convert.ToBase64String(buffer1, 0, i1);
		}


		/// <summary>
		/// Считывает начало файла по указанному пути на диске до указанного размера и возвращает его в виде Base64-строки.
		/// </summary>
		/// <param name="path">Путь к файлу.</param>
		/// <param name="size">Максимальное количество байт для чтения. По умолчанию равно 255.</param>
		/// <returns>Строка в формате Base64, представляющая начало файла.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileBegin(
			string path,
			int size = 255)
		{
			using var stream1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return GetFileBegin(stream1, size);
		}


		/// <summary>
		/// Вычисляет HMAC-SHA1 хэш для указанного потока файла с использованием байтовой соли.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static string GetFileSHA1(
			FileStream stream,
			byte[] salt)
		{
			using var alg1 = new HMACSHA1(salt);
			byte[] hash1 = alg1.ComputeHash(stream);
			return Convert.ToHexString(hash1).ToLowerInvariant();
		}


		/// <summary>
		/// Вычисляет HMAC-SHA1 хэш для указанного потока файла с использованием строковой соли (переводится в Unicode-байты).
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileSHA1(
			FileStream stream,
			string salt)
		{
			return GetFileSHA1(stream, Encoding.Unicode.GetBytes(salt));
		}


		/// <summary>
		/// Вычисляет HMAC-SHA1 хэш для файла по указанному пути с использованием байтовой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileSHA1(
			string path,
			byte[] salt)
		{
			using var stream1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return GetFileSHA1(stream1, salt);
		}


		/// <summary>
		/// Вычисляет HMAC-SHA1 хэш для файла по указанному пути с использованием строковой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileSHA1(
			string path,
			string salt)
		{
			return GetFileSHA1(path, Encoding.Unicode.GetBytes(salt));
		}


		/// <summary>
		/// Вычисляет HMAC-SHA256 хэш для указанного потока файла с использованием байтовой соли.
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		public static string GetFileSHA256(
			FileStream stream,
			byte[] salt)
		{
			using var alg1 = new HMACSHA256(salt);
			byte[] hash1 = alg1.ComputeHash(stream);
			return Convert.ToHexString(hash1).ToLowerInvariant();
		}


		/// <summary>
		/// Вычисляет HMAC-SHA256 хэш для указанного потока файла с использованием строковой соли (переводится в Unicode-байты).
		/// </summary>
		/// <param name="stream">Открытый поток файла.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileSHA256(
			FileStream stream,
			string salt)
		{
			return GetFileSHA256(stream, Encoding.Unicode.GetBytes(salt));
		}


		/// <summary>
		/// Вычисляет HMAC-SHA256 хэш для файла по указанному пути с использованием байтовой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Байтовый массив соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileSHA256(
			string path,
			byte[] salt)
		{
			using var stream1 = new FileStream(
				path, FileMode.Open, FileAccess.Read, FileShare.Read);
			return GetFileSHA256(stream1, salt);
		}


		/// <summary>
		/// Вычисляет HMAC-SHA256 хэш для файла по указанному пути с использованием строковой соли.
		/// </summary>
		/// <param name="path">Путь к хэшируемому файлу.</param>
		/// <param name="salt">Строковое значение соли.</param>
		/// <returns>Шестнадцатеричная строка хэша в нижнем регистре.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetFileSHA256(
			string path,
			string salt)
		{
			return GetFileSHA256(path, Encoding.Unicode.GetBytes(salt));
		}


		/// <summary>
		/// Возвращает время последнего изменения файла с автоматическим приведением к типу <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <param name="filename">Полный или относительный путь к файлу.</param>
		/// <returns>Объект структуры <see cref="DateTimeOffset"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset GetFileLastModified(
			string filename)
		{
			var time1 = File.GetLastWriteTimeUtc(filename);
			return new DateTimeOffset(time1);
		}


		/// <summary>
		/// Возвращает максимальное (самое последнее) время модификации файлов, находящихся непосредственно в указанном каталоге.
		/// </summary>
		/// <param name="directory">Информационный объект исследуемого каталога <see cref="DirectoryInfo"/>.</param>
		/// <returns>Объект <see cref="DateTime"/>, соответствующий дате самого свежего файла.</returns>
		public static DateTime GetLastWriteTimeFiles(
			DirectoryInfo directory)
		{
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			return (name.Length < 5 && _Consts.FORBIDDEN_FILE_NAMES.Contains(name))
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasInvalidFileNameChars(
			string filename)
		{
			return filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
		}


		/// <summary>
		/// Выполняет полное глубокое побайтовое сравнение содержимого двух файлов на диске. Различия в путях или именах файлов игнорируются.
		/// </summary>
		/// <param name="file1">Первый сравниваемый файл.</param>
		/// <param name="file2">Второй сравниваемый файл.</param>
		/// <returns><see langword="true"/>, если содержимое файлов абсолютно побайтово совпадает; иначе — <see langword="false"/>.</returns>
		public static bool IsFilesEqualFull(
			FileInfo file1,
			FileInfo file2)
		{
			if (file1.Length != file2.Length)
				return false;
			if (string.Equals(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase))
				return true;
			using var stream1 = file1.OpenRead();
			using var stream2 = file2.OpenRead();
			const int _BUF_SIZE = 4096;
			byte[] buffer1 = new byte[_BUF_SIZE];
			byte[] buffer2 = new byte[_BUF_SIZE];
			while (true)
			{
				int count1 = stream1.Read(buffer1, 0, _BUF_SIZE);
				int count2 = stream2.Read(buffer2, 0, _BUF_SIZE);
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
		/// Выполняет быстрое «ленивое» сравнение двух файлов по их метаданным (размеру и дате изменения). Если они совпадают, возвращает true; иначе перепроверяет побайтово через <see cref="IsFilesEqualFull"/>.
		/// </summary>
		/// <param name="file1">Первый сравниваемый файл.</param>
		/// <param name="file2">Второй сравниваемый файл.</param>
		/// <returns><see langword="true"/>, если файлы идентичны; иначе — <see langword="false"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFilesEqualLazy(
			FileInfo file1,
			FileInfo file2)
		{
			if (file1.Length == file2.Length
				&& file1.LastWriteTimeUtc == file2.LastWriteTimeUtc)
				return true;
			return IsFilesEqualFull(file1, file2);
		}


		/* methods */


		/// <summary>
		/// Создает директорию по указанному пути, если она еще отсутствует в системе.
		/// </summary>
		/// <param name="path">Путь к создаваемой директории.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Rename(
			FileInfo file,
			string newName)
		{
			var uniquePath = GetNewName(file, newName);
			file.MoveTo(uniquePath);
		}


		/// <summary>
		/// Записывает строковый контент в файл по указанному пути, используя выбранную кодировку <see cref="EncodingsEnum"/> и режим открытия файлового потока.
		/// </summary>
		/// <param name="path">Путь к целевому файлу для записи.</param>
		/// <param name="content">Строковое содержимое, подлежащее сохранению.</param>
		/// <param name="encoding">Кодировка текста. По умолчанию используется <see cref="EncodingsEnum.UTF8"/>.</param>
		/// <param name="mode">Режим работы файлового потока. По умолчанию используется <see cref="FileMode.Create"/> (перезапись/создание нового).</param>
		public static void FileWrite(
			string path,
			string content,
			EncodingsEnum encoding = EncodingsEnum.UTF8,
			FileMode mode = FileMode.Create)
		{
			using var fs1 = new FileStream(path, mode);
			using var sw1 = new StreamWriter(fs1, GetEncoding(encoding));
			sw1.Write(content);
		}


		/// <summary>
		/// Записывает сырой массив байт в файл по указанному пути, используя выбранный режим открытия файлового потока.
		/// </summary>
		/// <param name="path">Путь к файлу для записи.</param>
		/// <param name="content">Массив байт, который необходимо записать в файл.</param>
		/// <param name="mode">Режим работы файлового потока. По умолчанию используется <see cref="FileMode.Create"/>.</param>
		public static void FileWrite(
			string path,
			byte[] content,
			FileMode mode = FileMode.Create)
		{
			using var fs1 = new FileStream(path, mode);
			fs1.Write(content);
		}

	}

}
