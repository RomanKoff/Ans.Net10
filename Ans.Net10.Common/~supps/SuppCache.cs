// rev 2026-09-26

using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для работы с кэшем в памяти.
	/// </summary>
	public static class SuppCache
	{

		/* consts */


		/// <summary>
		/// Настройки кэширования по умолчанию (абсолютное время жизни — 10 секунд).
		/// </summary>
		/// <value>Экземпляр <see cref="MemoryCacheEntryOptions"/> с предустановленным 10-секундным интервалом абсолютного истечения срока.</value>
		public static readonly MemoryCacheEntryOptions DEFAULT_CACHE_OPTIONS = new()
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
		};


		/// <summary>
		/// Настройки кэширования с минимальным временем жизни (фактически отключенное кэширование).
		/// </summary>
		/// <value>Экземпляр <see cref="MemoryCacheEntryOptions"/> с интервалом абсолютного истечения в 1 миллисекунду.</value>
		public static readonly MemoryCacheEntryOptions ZERO_CACHE_OPTIONS = new()
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1)
		};


		/* functions */


		/// <summary>
		/// Формирует параметры конфигурации записи кэша на основе переданных интервалов времени в секундах.
		/// </summary>
		/// <param name="slidingExpirationSeconds">Скользящее время жизни записи в секундах (интервал пролонгируется при каждом повторном обращении).</param>
		/// <param name="absoluteExpirationRelativeToNowSeconds">Абсолютное время жизни записи в секундах относительно текущего рантайм-момента.</param>
		/// <returns>Готовый сконфигурированный объект параметров записи кэша <see cref="MemoryCacheEntryOptions"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemoryCacheEntryOptions GetOptions(
			int slidingExpirationSeconds,
			int absoluteExpirationRelativeToNowSeconds)
		{
			if (slidingExpirationSeconds <= 0 && absoluteExpirationRelativeToNowSeconds <= 0)
				return DEFAULT_CACHE_OPTIONS;
			var options1 = new MemoryCacheEntryOptions();
			if (slidingExpirationSeconds > 0)
				options1.SlidingExpiration = TimeSpan.FromSeconds(
					slidingExpirationSeconds);
			if (absoluteExpirationRelativeToNowSeconds > 0)
				options1.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(
					absoluteExpirationRelativeToNowSeconds);
			return options1;
		}

	}

}
