// rev 2026-09-30

using Ans.Net10.Common;
using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="IFormFile"/>, обеспечивающие асинхронную загрузку файлов на диск, 
	/// в том числе с поддержкой поблочной (чанковой) передачи данных.
	/// </summary>
	public static partial class Exts_IFormFile
	{

		/* methods */


		/// <summary>
		/// Асинхронно загружает содержимое файла или его текущего блока (чанка) в целевую директорию на диске посредством ядра <see cref="SuppIO"/>.
		/// </summary>
		/// <param name="file">Объект загружаемого файла <see cref="IFormFile"/>.</param>
		/// <param name="filename">Имя, под которым файл будет сохранен на диске.</param>
		/// <param name="chunk">Порядковый номер текущего блока (индексация с 0). Если равен 0, файл перезаписывается; если больше 0 — данные дописываются в конец.</param>
		/// <param name="path">Абсолютный или относительный путь к целевой директории сохранения.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Поток-задача, представляющая асинхронную операцию записи файла на диск.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="file"/> равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Вызывается, если параметры <paramref name="filename"/> или <paramref name="path"/> пусты.</exception>
		public static async Task UploadSmallFileAsync(
			this IFormFile file,
			string filename,
			int chunk,
			string path,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(file);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			ArgumentException.ThrowIfNullOrEmpty(path);
			var filepath1 = Path.Combine(path, filename);
			var mode1 = chunk == 0
				? FileMode.Create
				: FileMode.Append;
			var bytes1 = await file.GetContentInBytesAsync().ConfigureAwait(false);
			await SuppIO.FileWriteAsync(filepath1, bytes1, mode1, cancellationToken)
				.ConfigureAwait(false);
		}


		/// <summary>
		/// Асинхронно и безопасно стримит содержимое крупного файла напрямую на диск без выделения памяти в куче под весь объем данных.
		/// </summary>
		public static async Task UploadLargeFileAsync(
			this IFormFile file,
			string filename,
			int chunk,
			string path,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(file);
			ArgumentException.ThrowIfNullOrEmpty(filename);
			ArgumentException.ThrowIfNullOrEmpty(path);
			var filepath1 = Path.Combine(path, filename);
			var mode1 = chunk == 0
				? FileMode.Create
				: FileMode.Append;
			using var fs1 = new FileStream(
				filepath1, mode1, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
			using var ns1 = file.OpenReadStream();
			await ns1.CopyToAsync(fs1, cancellationToken)
				.ConfigureAwait(false);
		}


		/* functions */


		/// <summary>
		/// Асинхронно считывает входящий поток файла посредством буферизации в памяти и возвращает его содержимое в виде массива байт.
		/// </summary>
		/// <param name="file">Объект загружаемого файла <see cref="IFormFile"/>.</param>
		/// <returns>Поток-задача, возвращающая массив байт, который представляет полное содержимое файла.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="file"/> равен <see langword="null"/>.</exception>
		public static async Task<byte[]> GetContentInBytesAsync(
			this IFormFile file)
		{
			ArgumentNullException.ThrowIfNull(file);
			using var ms1 = new MemoryStream((int)file.Length);
			using var fs1 = file.OpenReadStream();
			await fs1.CopyToAsync(ms1).ConfigureAwait(false);
			return ms1.ToArray();
		}

	}

}
