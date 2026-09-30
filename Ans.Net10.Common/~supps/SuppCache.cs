// rev 2026-09-29

using Microsoft.Extensions.Caching.Hybrid;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для работы с гибридным кэшем.
	/// </summary>
	public static class SuppCache
	{

		/* consts */


		/// <summary>
		/// Пресет, полностью отключающий использование кэша (L1 и L2 уровни).
		/// </summary>
		public static readonly HybridCacheEntryOptions HYBRID_CACHE_NONE = new()
		{
			Flags = HybridCacheEntryFlags.DisableLocalCache
				| HybridCacheEntryFlags.DisableLocalCacheWrite
				| HybridCacheEntryFlags.DisableDistributedCache
				| HybridCacheEntryFlags.DisableDistributedCacheWrite
		};


		/// <summary>
		/// Пресет кэширования на 30 секунд с гарантированной очисткой.
		/// </summary>
		public static readonly HybridCacheEntryOptions HYBRID_CACHE_30SEC = new()
		{
			Expiration = TimeSpan.FromSeconds(30),
			LocalCacheExpiration = TimeSpan.FromSeconds(30)
		};


		/// <summary>
		/// Пресет кэширования на 1 минуту с гарантированной очисткой.
		/// </summary>
		public static readonly HybridCacheEntryOptions HYBRID_CACHE_1MIN = new()
		{
			Expiration = TimeSpan.FromMinutes(1),
			LocalCacheExpiration = TimeSpan.FromMinutes(1)
		};


		/// <summary>
		/// Пресет кэширования на 5 минут с гарантированной очисткой.
		/// </summary>
		public static readonly HybridCacheEntryOptions HYBRID_CACHE_5MIN = new()
		{
			Expiration = TimeSpan.FromMinutes(5),
			LocalCacheExpiration = TimeSpan.FromMinutes(5)
		};


		/// <summary>
		/// Пресет кэширования на 10 минут с гарантированной очисткой.
		/// </summary>
		public static readonly HybridCacheEntryOptions HYBRID_CACHE_10MIN = new()
		{
			Expiration = TimeSpan.FromMinutes(10),
			LocalCacheExpiration = TimeSpan.FromMinutes(10)
		};

	}

}
