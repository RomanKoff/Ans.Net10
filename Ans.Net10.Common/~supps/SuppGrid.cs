// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для высокопроизводительного потокового чтения данных в формате GRID.
	/// </summary>
	public static class SuppGrid
	{

		/* functions */

		/// <summary>
		/// Выполняет ленивое потоковое чтение GRID-данных из текстового потока <see cref="TextReader"/>, 
		/// автоматически фильтруя комментарии (//, --, ==) и проецируя строки в целевые объекты.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="reader">Входящий поток текстовых данных.</param>
		/// <param name="selector">Функция-предикат (лямбда) для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <returns>Ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="reader"/> или <paramref name="selector"/> равен <see langword="null"/>.</exception>
		public static IEnumerable<T> GetItems<T>(
			TextReader reader,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null)
		{
			ArgumentNullException.ThrowIfNull(reader);
			ArgumentNullException.ThrowIfNull(selector);
			string? line1;
			while ((line1 = reader.ReadLine()) != null)
			{
				var parser1 = _getRowParser(line1, provider);
				if (parser1 == null)
					continue;
				yield return selector(parser1);
			}
		}


		/// <summary>
		/// Выполняет ленивое чтение GRID-данных из текстовой строки.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="source">Исходная строка, содержащая документ в формате GRID.</param>
		/// <param name="selector">Функция-предикат для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <returns>Ленивый поток материализованных объектов типа T.</returns>
		public static IEnumerable<T> GetItemsFromString<T>(
			string source,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null)
		{
			if (string.IsNullOrEmpty(source))
				yield break;
			using var reader1 = new StringReader(source);
			foreach (T item1 in GetItems(reader1, selector, provider))
				yield return item1;
		}


		/// <summary>
		/// Выполняет ленивое чтение GRID-данных из входящего бинарного потока <see cref="Stream"/>.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="stream">Входящий бинарный поток данных.</param>
		/// <param name="selector">Функция-предикат для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <param name="encoding">Опциональная кодировка текста. Если равен <see langword="null"/> — используется UTF-8 без BOM.</param>
		/// <returns>Ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="stream"/> равен <see langword="null"/>.</exception>
		public static IEnumerable<T> GetItemsFromStream<T>(
			Stream stream,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null)
		{
			ArgumentNullException.ThrowIfNull(stream);
			using var reader1 = new StreamReader(stream, encoding ?? new UTF8Encoding(false));
			foreach (T item1 in GetItems(reader1, selector, provider))
				yield return item1;
		}


		/// <summary>
		/// Выполняет высокоэффективное ленивое чтение GRID-данных напрямую из файла на диске.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="path">Полный путь к файлу в формате GRID.</param>
		/// <param name="selector">Функция-предикат для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <param name="encoding">Опциональная кодировка текста файла. Если равен <see langword="null"/> — используется UTF-8 без BOM.</param>
		/// <returns>Ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если путь к файлу <paramref name="path"/> пуст или равен <see langword="null"/>.</exception>
		public static IEnumerable<T> GetItemsFromFile<T>(
			string path,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentException(
					"Путь к файлу не может быть пустым.", nameof(path));
			var lines1 = File.ReadLines(path, encoding ?? new UTF8Encoding(false));
			foreach (string line1 in lines1)
			{
				var parser1 = _getRowParser(line1, provider);
				if (parser1 == null)
					continue;
				yield return selector(parser1);
			}
		}


		/// <summary>
		/// Выполняет асинхронное и высокоэффективное построчное чтение GRID-данных напрямую из файла на диске.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="path">Полный путь к файлу в формате GRID.</param>
		/// <param name="selector">Функция-предикат для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <param name="encoding">Опциональная кодировка текста файла. Если равен <see langword="null"/> — используется UTF-8 без BOM.</param>
		/// <returns>Асинхронный ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если путь к файлу <paramref name="path"/> пуст или равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="selector"/> равен <see langword="null"/>.</exception>
		public static async IAsyncEnumerable<T> GetItemsFromFileAsync<T>(
			string path,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentException(
					"Путь к файлу не может быть пустым.", nameof(path));
			ArgumentNullException.ThrowIfNull(selector);
			var options1 = new FileStreamOptions
			{
				Mode = FileMode.Open,
				Access = FileAccess.Read,
				Share = FileShare.Read,
				Options = FileOptions.Asynchronous
			};
			using var fileStream1 = new FileStream(path, options1);
			using var reader1 = new StreamReader(fileStream1, encoding ?? new UTF8Encoding(false));
			string? line1;
			while ((line1 = await reader1.ReadLineAsync().ConfigureAwait(false)) != null)
			{
				var parser1 = _getRowParser(line1, provider);
				if (parser1 == null)
					continue;
				yield return selector(parser1);
			}
		}


		/* privates */


		/// <summary>
		/// Инкапсулирует логику валидации строки, фильтрации комментариев и создания парсера строки.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static GridRowParser? _getRowParser(
			string line,
			IFormatProvider? provider)
		{
			if (string.IsNullOrEmpty(line))
				return null;
			var span1 = line.AsSpan();
			if (span1.Length >= 2)
				if ((span1[0] == '/' && span1[1] == '/')
					|| (span1[0] == '-' && span1[1] == '-')
					|| (span1[0] == '=' && span1[1] == '='))
					return null;
			if (span1.IsWhiteSpace())
				return null;
			var fields1 = line.Split('|');
			return new GridRowParser(fields1, provider);
		}

	}

}
