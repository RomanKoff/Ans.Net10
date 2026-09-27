// rev 2026-09-25

using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для упрощения работы с кэшем в памяти <see cref="IMemoryCache"/>.
	/// Предоставляет потокобезопасные методы извлечения данных с автоматическим заполнением при их отсутствии.
	/// </summary>
	public class MemoryCacheHelper
	{

		private readonly IMemoryCache _cache;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="MemoryCacheHelper"/> с внедрением зависимости кэша
		/// и возможностью переопределения параметров кэширования по умолчанию.
		/// </summary>
		/// <param name="cache">Экземпляр службы кэширования в памяти, реализующий интерфейс <see cref="IMemoryCache"/>.</param>
		/// <param name="defaultCacheOptions">
		/// Опциональные параметры времени жизни и политик вытеснения записей кэша по умолчанию.
		/// Если передано значение <see langword="null"/> — принудительно применяется глобальная конфигурация <see cref="SuppCache.DEFAULT_CACHE_OPTIONS"/>.
		/// </param>
		public MemoryCacheHelper(
			IMemoryCache cache,
			MemoryCacheEntryOptions? defaultCacheOptions = null)
		{
			_cache = cache;
			DefaultCacheOptions = defaultCacheOptions ?? SuppCache.DEFAULT_CACHE_OPTIONS;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает параметры времени жизни, приоритетов и ограничений записей кэша, применяемые по умолчанию.
		/// </summary>
		/// <value>Объект конфигурации <see cref="MemoryCacheEntryOptions"/>.</value>
		public MemoryCacheEntryOptions DefaultCacheOptions { get; }


		/* functions */


		/// <summary>
		/// Возвращает значение из кэша по указанному уникальному ключу. Если объект отсутствует, 
		/// выполняет его атомарное получение через переданный делегат-фабрику, сохраняет в кэш и возвращает результат.
		/// </summary>
		/// <remarks>
		/// Метод использует системный метод расширения CacheExtensions.GetOrCreate{TItem}, гарантирующий потокобезопасность. 
		/// Делегат <paramref name="getObject"/> будет выполнен только в том случае, если в кэше нет валидной записи с ключом <paramref name="cacheKey"/>.
		/// </remarks>
		/// <typeparam name="T">Тип кэшируемого объекта (может быть как ссылочным, так и значимым типом).</typeparam>
		/// <param name="cacheKey">Уникальный строковый ключ записи кэша.</param>
		/// <param name="getObject">Делегат-фабрика (<see cref="Func{T}"/>), возвращающий объект для добавления в кэш при промахе (cache miss).</param>
		/// <param name="options">
		/// Опциональные индивидуальные параметры времени жизни текущей записи кэша.
		/// Если передано значение <see langword="null"/> — для этой записи применяются параметры по умолчанию из свойства <see cref="DefaultCacheOptions"/>.
		/// </param>
		/// <returns>Найденное в кэше значение или вновь созданный фабрикой объект типа <typeparamref name="T"/>.</returns>
		public T? Get<T>(
			string cacheKey,
			Func<T> getObject,
			MemoryCacheEntryOptions? options = null)
		{
			return _cache.GetOrCreate(
				cacheKey, x =>
				{
					x.SetOptions(options ?? DefaultCacheOptions);
					return getObject();
				});
		}


		/* methods */


		/// <summary>
		/// Принудительно и немедленно удаляет запись из кэша по её уникальному строковому ключу.
		/// </summary>
		/// <remarks>
		/// Если запись с указанным ключом отсутствует в кэше, метод не генерирует исключений.
		/// </remarks>
		/// <param name="cacheKey">Уникальный строковый ключ удаляемой записи.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(
			string cacheKey)
		{
			_cache.Remove(cacheKey);
		}

	}

}
