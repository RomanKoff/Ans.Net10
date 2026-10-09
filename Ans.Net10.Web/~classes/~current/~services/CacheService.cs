// rev 2026-10-07

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба кэширования, предоставляющая высокоуровневую обертку над 
	/// системой гибридного кэширования <see cref="HybridCache"/> для работы в контексте приложения.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="CacheService"/> с использованием первичного конструктора C#.
	/// </remarks>
	/// <param name="current">Текущий контекст обработки запроса, содержащий базовую службу кэша и логгер.</param>
	public partial class CacheService(
		CurrentContext current)
	{

		private readonly HybridCache _cache
			= current.HybridCache
				?? throw new ArgumentNullException(nameof(current));
		private readonly ILogger _logger
			= current.Logger
				?? throw new ArgumentNullException(nameof(current));


		/* methods */


		/// <summary>
		/// Асинхронно извлекает объект из кэша по указанному ключу. Если объект отсутствует, 
		/// выполняет его получение через фабричный делегат, сохраняет в кэш и возвращает результат.
		/// </summary>
		/// <typeparam name="T">Тип кэшируемого объекта.</typeparam>
		/// <param name="cacheKey">Уникальный строковый ключ записи кэша.</param>
		/// <param name="getObject">Асинхронный делегат-фабрика, возвращающий объект при промахе кэша.</param>
		/// <param name="options">Опциональные индивидуальные параметры времени жизни и политик текущей записи кэша.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Значение типа <typeparamref name="T"/>, содержащееся в кэше или созданное фабрикой. Допускает возвращение <see langword="null"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="cacheKey"/> или <paramref name="getObject"/> равен <see langword="null"/>.
		/// </exception>
		public async ValueTask<T?> GetAsync<T>(
			string cacheKey,
			Func<CancellationToken, ValueTask<T?>> getObject,
			HybridCacheEntryOptions? options = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(cacheKey);
			ArgumentNullException.ThrowIfNull(getObject);
			var isCacheMiss1 = false;
			var result1 = await _cache.GetOrCreateAsync(
				cacheKey,
				async token =>
				{
					isCacheMiss1 = true;
					return await getObject(token);
				},
				options,
				cancellationToken: cancellationToken);
			if (isCacheMiss1)
				_log.CacheMiss(_logger, cacheKey);
			return result1;
		}


		/* functions */


		/// <summary>
		/// Асинхронно и немедленно удаляет запись из гибридного кэша по её уникальному строковому ключу.
		/// </summary>
		/// <param name="cacheKey">Уникальный строковый ключ удаляемой записи.</param>
		/// <returns>
		/// Структура <see cref="ValueTask"/>, представляющая асинхронную операцию удаления.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="cacheKey"/> равен <see langword="null"/>.
		/// </exception>
		public ValueTask RemoveAsync(
			string cacheKey)
		{
			ArgumentNullException.ThrowIfNull(cacheKey);
			var task1 = _cache.RemoveAsync(cacheKey);
			_log.CachePurged(_logger, cacheKey);
			return task1;
		}


		/* privates */


		private static partial class _log
		{
			[LoggerMessage(
				EventId = 20,
				Level = LogLevel.Information,
				Message = "[Cache] Промах кэша (Cache MISS) для ключа: {Key}")]
			public static partial void CacheMiss(ILogger logger, string key);

			[LoggerMessage(
				EventId = 21,
				Level = LogLevel.Information,
				Message = "[Cache] Запись принудительно удалена из кэша (Cache PURGED): {Key}")]
			public static partial void CachePurged(ILogger logger, string key);
		}

	}

}
