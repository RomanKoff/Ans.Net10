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
		/// Выполняет асинхронное ленивое потоковое чтение GRID-данных из текстового потока <see cref="TextReader"/>, 
		/// автоматически фильтруя комментарии (//, --, ==) и проецируя строки в целевые объекты.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="reader">Входящий поток текстовых данных.</param>
		/// <param name="selector">Функция-предикат (лямбда) для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Асинхронный ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="reader"/> или <paramref name="selector"/> равен <see langword="null"/>.</exception>
		public static async IAsyncEnumerable<T> GetItemsAsync<T>(
			TextReader reader,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(reader);
			ArgumentNullException.ThrowIfNull(selector);
			while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line1)
			{
				cancellationToken.ThrowIfCancellationRequested();
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
			foreach (T item1 in _getItems(reader1, selector, provider))
				yield return item1;
		}


		/// <summary>
		/// Выполняет асинхронное ленивое чтение GRID-данных из входящего бинарного потока <see cref="Stream"/>.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="stream">Входящий бинарный поток данных.</param>
		/// <param name="selector">Функция-предикат для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <param name="encoding">Опциональная кодировка текста. Если равен <see langword="null"/> — используется UTF-8 без BOM.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Асинхронный ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="stream"/> равен <see langword="null"/>.</exception>
		public static async IAsyncEnumerable<T> GetItemsFromStreamAsync<T>(
			Stream stream,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(stream);
			using var reader1 = new StreamReader(stream, encoding ?? new UTF8Encoding(false));
			await foreach (T item1 in GetItemsAsync(reader1, selector, provider, cancellationToken).ConfigureAwait(false))
				yield return item1;
		}


		/// <summary>
		/// Выполняет асинхронное и высокоэффективное построчное чтение GRID-данных напрямую из файла на диске.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="path">Полный путь к файлу в формате GRID.</param>
		/// <param name="selector">Функция-предикат для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки. Если равен <see langword="null"/>, используется инвариантная культура.</param>
		/// <param name="encoding">Опциональная кодировка текста файла. Если равен <see langword="null"/> — используется UTF-8 без BOM.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Асинхронный ленивый поток материализованных объектов типа T.</returns>
		/// <exception cref="ArgumentException">Выбрасывается, если путь к файлу <paramref name="path"/> пуст или равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="selector"/> равен <see langword="null"/>.</exception>
		public static async IAsyncEnumerable<T> GetItemsFromFileAsync<T>(
			string path,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentException(
					"[Ans.Net10.Common] Путь к файлу не может быть пустым.", nameof(path));
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
			await foreach (T item1 in GetItemsAsync(reader1, selector, provider, cancellationToken).ConfigureAwait(false))
				yield return item1;
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


		private static IEnumerable<T> _getItems<T>(
			TextReader reader,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider)
		{
			string? line1;
			while ((line1 = reader.ReadLine()) != null)
			{
				var parser1 = _getRowParser(line1, provider);
				if (parser1 == null)
					continue;
				yield return selector(parser1);
			}
		}


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
