// rev 2026-10-02

using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для формирования стандартизированных результатов ответов контроллера (Action Results),
	/// оптимизированных для эффективной асинхронной отдачи физических файлов с поддержкой HTTP-кэширования.
	/// </summary>
	public static class SuppResult
	{

		/// <summary>
		/// Конструирует высокопроизводительный результат отдачи физического файла, внедряя в HTTP-заголовки 
		/// метаданные даты изменения и уникального валидатора тега (ETag) для кэширования на стороне клиента.
		/// </summary>
		/// <param name="physicalPath">Абсолютный физический путь к целевому файлу на диске сервера.</param>
		/// <param name="contentType">Официальный MIME-тип контента (например, "image/png", "application/pdf").</param>
		/// <returns>Строго типизированный объект результата <see cref="PhysicalFileResult"/>.</returns>
		/// <exception cref="ArgumentException">
		/// Вызывается, если параметр <paramref name="physicalPath"/> или <paramref name="contentType"/> пуст или равен <see langword="null"/>.
		/// </exception>
		/// <remarks>
		/// Метод считывает временную метку последней записи файла для формирования заголовка Last-Modified 
		/// и генерирует на её основе строгий <see cref="EntityTagHeaderValue"/>, предотвращая повторную 
		/// избыточную передачу неизмененного контента по сети.
		/// </remarks>
		public static PhysicalFileResult GetPhysicalFileResult(
			string physicalPath,
			string contentType)
		{
			ArgumentException.ThrowIfNullOrEmpty(physicalPath);
			ArgumentException.ThrowIfNullOrEmpty(contentType);
			var fileInfo1 = new FileInfo(physicalPath);
			var lastModifiedUtc1 = fileInfo1.Exists
				? fileInfo1.LastWriteTimeUtc
				: DateTime.UtcNow;
			var eTag1 = new EntityTagHeaderValue($"\"{lastModifiedUtc1.Ticks}\"");
			return new PhysicalFileResult(physicalPath, contentType)
			{
				LastModified = lastModifiedUtc1,
				EntityTag = eTag1
			};
		}


		/// <summary>
		/// Асинхронно проверяет фактическое существование файла на диске и возвращает результат отдачи файла, 
		/// либо стандартизированный ответ HTTP 404 Not Found, не блокируя потоки веб-сервера.
		/// </summary>
		/// <param name="path">Абсолютный физический путь к запрашиваемому файлу.</param>
		/// <param name="contentType">Официальный MIME-тип контента.</param>
		/// <returns>
		/// Задача, результатом которой является <see cref="IActionResult"/>: <see cref="PhysicalFileResult"/> при успешном обнаружении,
		/// либо <see cref="NotFoundResult"/>, если файл отсутствует или доступ к нему запрещен.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Вызывается, если параметр <paramref name="path"/> или <paramref name="contentType"/> пуст или равен <see langword="null"/>.
		/// </exception>
		public static Task<IActionResult> GetPhysicalFileOrNotFoundResultAsync(
			string path,
			string contentType)
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			ArgumentException.ThrowIfNullOrEmpty(contentType);
			return Task.Run<IActionResult>(() =>
			{
				if (File.Exists(path))
					return GetPhysicalFileResult(path, contentType);
				return new NotFoundResult();
			});
		}

	}

}
