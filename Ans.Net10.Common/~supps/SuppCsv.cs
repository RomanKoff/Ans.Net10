// rev 2026-09-26

using CsvHelper;
using System.Globalization;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для высокопроизводительной работы с данными в формате CSV.
	/// </summary>
	public static class SuppCsv
	{
		/// <summary>
		/// Асинхронно и в неблокирующем режиме сериализует коллекцию объектов и записывает её напрямую в целевой поток 
		/// в кодировке UTF-8 без BOM, предотвращая избыточные аллокации памяти.
		/// </summary>
		/// <remarks>
		/// Этот метод идеален для веб-приложений, так как позволяет осуществлять потоковую передачу данных (streaming) 
		/// напрямую в тело HTTP-ответа без необходимости буферизации всего документа в оперативной памяти сервера.
		/// </remarks>
		/// <typeparam name="T">Тип сериализуемых доменных объектов или моделей данных.</typeparam>
		/// <param name="targetStream">Целевой выходной поток (например, поток файла или поток HTTP-ответа), в который ведется запись.</param>
		/// <param name="items">Коллекция элементов для выгрузки в CSV-документ.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, представляющая асинхронную операцию потоковой записи CSV.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="targetStream"/> или <paramref name="items"/> равен <see langword="null"/>.</exception>
		public static async Task WriteCsvToStreamAsync<T>(
			Stream targetStream,
			IEnumerable<T> items,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(targetStream);
			ArgumentNullException.ThrowIfNull(items);
			// Использование UTF-8 без BOM — лучшая практика для веб-интерфейсов и API
			// Оставляем поток открытым (leaveOpen: true), так как за жизненный цикл targetStream отвечает вызывающий код
			using var writer1 = new StreamWriter(targetStream, new UTF8Encoding(false), leaveOpen: true);
			using var csv1 = new CsvWriter(writer1, CultureInfo.CurrentCulture);
			await csv1.WriteRecordsAsync(items, cancellationToken).ConfigureAwait(false);
			await writer1.FlushAsync(cancellationToken).ConfigureAwait(false);
		}
	}

}
