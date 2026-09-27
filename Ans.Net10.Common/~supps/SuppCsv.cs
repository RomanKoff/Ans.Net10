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

		/* functions */

		/// <summary>
		/// Сериализует коллекцию объектов в массив байт CSV (в кодировке UTF-8 без BOM).
		/// </summary>
		/// <remarks>
		/// Использование UTF-8 без BOM является лучшей практикой для интеграции с веб-интерфейсами и внешними API.
		/// Если переданная коллекция <paramref name="items"/> пуста, метод вернет массив байт, содержащий только строку заголовков свойств типа <typeparamref name="T"/>.
		/// </remarks>
		/// <typeparam name="T">Тип сериализуемых доменных объектов или моделей данных.</typeparam>
		/// <param name="items">Коллекция элементов для выгрузки в CSV-документ.</param>
		/// <returns>Массив байт, представляющий готовый документ в формате CSV.</returns>
		public static byte[] GetCsvBytesFromObject<T>(
			IEnumerable<T> items)
		{
			using var stream1 = new MemoryStream();
			// Использование UTF-8 без BOM — лучшая практика для веб-интерфейсов и API
			using var writer1 = new StreamWriter(stream1, new UTF8Encoding(false));
			using var csv1 = new CsvWriter(writer1, CultureInfo.CurrentCulture);
			csv1.WriteRecords(items);
			writer1.Flush();
			return stream1.ToArray();
		}

	}

}
