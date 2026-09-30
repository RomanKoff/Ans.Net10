// rev 2026-09-25

using Microsoft.Extensions.Caching.Hybrid;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для упрощения работы с гибридным кэшем второго поколения <see cref="HybridCache"/>.
	/// Предоставляет потокобезопасные асинхронные методы извлечения данных с автоматической защитой от пробоя кэша (Cache Stampede).
	/// </summary>
	public class HybridCacheHelper
	{

		private readonly HybridCache _cache;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="HybridCacheHelper"/> с внедрением зависимости гибридного кэша
		/// и возможностью переопределения параметров кэширования по умолчанию.
		/// </summary>
		/// <param name="cache">Экземпляр службы гибридного кэширования платформы .NET 10.</param>
		/// <param name="defaultCacheOptions">
		/// Опциональные параметры времени жизни и политик вытеснения записей кэша по умолчанию.
		/// Если передано значение <see langword="null"/> — применяется глобальная конфигурация системы.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="cache"/> равен <see langword="null"/>.
		/// </exception>
		public HybridCacheHelper(
			HybridCache cache,
			HybridCacheEntryOptions? defaultCacheOptions = null)
		{
			ArgumentNullException.ThrowIfNull(cache);
			_cache = cache;
			DefaultCacheOptions = defaultCacheOptions;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает параметры времени жизни и ограничений записей кэша, применяемые по умолчанию.
		/// </summary>
		/// <value>Объект конфигурации <see cref="HybridCacheEntryOptions"/> или <see langword="null"/>, если используются системные настройки.</value>
		public HybridCacheEntryOptions? DefaultCacheOptions { get; }


		/* functions */


		/// <summary>
		/// Асинхронно извлекает значение из гибридного кэша по указанному уникальному ключу. Если объект отсутствует, 
		/// выполняет его атомарное получение через переданный делегат-фабрику, сохраняет в L1/L2 кэш и возвращает результат.
		/// </summary>
		/// <remarks>
		/// Метод использует встроенные механизмы <see cref="HybridCache"/>, гарантирующие, что при одновременном промахе кэша 
		/// из параллельных потоков фабричный метод <paramref name="getObject"/> выполнится только один раз.
		/// </remarks>
		/// <typeparam name="T">Тип кэшируемого объекта.</typeparam>
		/// <param name="cacheKey">Уникальный строковый ключ записи кэша.</param>
		/// <param name="getObject">Асинхронный делегат-фабрика, возвращающий объект для добавления в кэш при промахе.</param>
		/// <param name="options">
		/// Опциональные индивидуальные параметры времени жизни текущей записи кэша.
		/// Если передано значение <see langword="null"/> — для этой записи применяются параметры по умолчанию из свойства <see cref="DefaultCacheOptions"/>.
		/// </param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Найденное в кэше значение или вновь созданный фабрикой объект типа <typeparamref name="T"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="cacheKey"/> или <paramref name="getObject"/> равен <see langword="null"/>.
		/// </exception>
		public ValueTask<T> GetAsync<T>(
			string cacheKey,
			Func<CancellationToken, ValueTask<T>> getObject,
			HybridCacheEntryOptions? options = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(cacheKey);
			ArgumentNullException.ThrowIfNull(getObject);
			return _cache.GetOrCreateAsync(
				cacheKey,
				getObject,
				options ?? DefaultCacheOptions,
				cancellationToken: cancellationToken);
		}


		/* methods */


		/// <summary>
		/// Принудительно и немедленно удаляет запись из L1 и L2 уровней гибридного кэша по её уникальному строкову ключу.
		/// </summary>
		/// <param name="cacheKey">Уникальный строковый ключ удаляемой записи.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Значение <see cref="ValueTask"/>, представляющее асинхронную операцию удаления.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="cacheKey"/> равен <see langword="null"/>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueTask RemoveAsync(
			string cacheKey,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(cacheKey);
			return _cache.RemoveAsync(cacheKey, cancellationToken);
		}

	}

}
