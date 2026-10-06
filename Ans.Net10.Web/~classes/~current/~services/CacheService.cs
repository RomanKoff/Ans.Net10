// rev 2026-10-05

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
	/// <param name="current">Текущий контекст обработки запроса, содержащий базовую службу кэша.</param>
	public class CacheService(
		CurrentContext current)
	{

		private readonly HybridCache _cache = current.HybridCache;


		/* functions */


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
		/// Значение типа <typeparamref name="T"/>, содержащееся в кэше или созданное фабрикой.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="cacheKey"/> или <paramref name="getObject"/> равен <see langword="null"/>.
		/// </exception>
		public async ValueTask<T> GetAsync<T>(
			string cacheKey,
			Func<CancellationToken, ValueTask<T>> getObject,
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
			if (isCacheMiss1 && current.Logger.IsEnabled(LogLevel.Information))
				current.Logger.LogInformation("Cache MISS: {Key}", cacheKey);
			return result1;
		}


		/* methods */


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
		public async ValueTask RemoveAsync(
			string cacheKey)
		{
			ArgumentNullException.ThrowIfNull(cacheKey);
			await _cache.RemoveAsync(cacheKey);
			if (current.Logger.IsEnabled(LogLevel.Information))
				current.Logger.LogInformation("Cache PURGED: {Key}", cacheKey);
		}

	}

}
