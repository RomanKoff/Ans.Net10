// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для работы с подсистемой ввода-вывода (I/O), каталогами и файлами.
	/// </summary>
	public static partial class Exts__io
	{

		/* consts */


		/// <summary>
		/// Коллекция стандартных символов-разделителей, используемых для парсинга перечня расширений.
		/// </summary>
		/// <value>Массив символов, содержащий точку с запятой (<c>;</c>) и запятую (<c>,</c>).</value>
		public static readonly char[] SEP_EXTS = [';', ','];


		/// <summary>
		/// Возвращает перечисление файлов в каталоге, расширения которых соответствуют указанному списку в регистронезависимом режиме.
		/// </summary>
		/// <param name="directoryInfo">Информационный объект исследуемого каталога <see cref="DirectoryInfo"/>.</param>
		/// <param name="extensions">Список целевых расширений файлов, обязательно включая ведущую точку (например, <c>".txt"</c>, <c>".jpg"</c>).</param>
		/// <returns>Последовательность <see cref="IEnumerable{FileInfo}"/>, содержащая объекты найденных файлов.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="directoryInfo"/> равен <see langword="null"/>.</exception>
		public static IEnumerable<FileInfo> GetFilesByExtensions(
			this DirectoryInfo directoryInfo,
			params ReadOnlySpan<string> extensions)
		{
			ArgumentNullException.ThrowIfNull(directoryInfo);
			var a1 = new string[extensions.Length];
			for (int i1 = 0; i1 < extensions.Length; i1++)
				a1[i1] = extensions[i1];
			return directoryInfo.EnumerateFiles()
				.Where(x => a1.Contains(x.Extension, StringComparer.OrdinalIgnoreCase));
		}


		/// <summary>
		/// Возвращает перечисление файлов в каталоге, расширения которых соответствуют строке со списком расширений, разделенных точкой с запятой или запятой.
		/// </summary>
		/// <param name="directoryInfo">Информационный объект исследуемого каталога <see cref="DirectoryInfo"/>.</param>
		/// <param name="extensions">Строка с расширениями, разделенная символами из поля <see cref="SEP_EXTS"/> (например, <c>".txt;.jpg"</c> или <c>".png,.gif"</c>).</param>
		/// <returns>Последовательность <see cref="IEnumerable{FileInfo}"/>, содержащая объекты найденных файлов.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="directoryInfo"/> или <paramref name="extensions"/> равен <see langword="null"/>.</exception>
		public static IEnumerable<FileInfo> GetFilesByExtensions(
			this DirectoryInfo directoryInfo,
			string extensions)
		{
			ArgumentNullException.ThrowIfNull(extensions);
			return GetFilesByExtensions(
				directoryInfo,
				extensions.Split(SEP_EXTS, StringSplitOptions.RemoveEmptyEntries));
		}

	}

}
