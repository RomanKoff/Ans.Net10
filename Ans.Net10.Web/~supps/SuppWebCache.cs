// rev 2026-09-29

using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для работы с HTTP-кэшем.
	/// </summary>
	public static class SuppWebCache
	{

		/* consts */


		/// <summary>
		/// Профиль, полностью запрещающий кэширование HTTP-ответа на стороне клиента и прокси-серверов.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_NONE = new()
		{
			Duration = 0,
			Location = ResponseCacheLocation.None,
			NoStore = true
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 30 секунд.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_30SEC = new()
		{
			Duration = 30,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 1 минуту.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_1MIN = new()
		{
			Duration = 60,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 5 минут.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_5MIN = new()
		{
			Duration = 300,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 10 минут.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_10MIN = new()
		{
			Duration = 600,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Коллекция профилей кэширование HTTP-ответа на стороне клиента и прокси-серверов.
		/// </summary>
		public static readonly ReadOnlyDictionary<string, CacheProfile> HTTP_CACHE_PROFILES
			= new(new Dictionary<string, CacheProfile>
			{
				{ "HTTP_CACHE_NONE", HTTP_CACHE_NONE },
				{ "HTTP_CACHE_30SEC", HTTP_CACHE_30SEC },
				{ "HTTP_CACHE_1MIN", HTTP_CACHE_1MIN },
				{ "HTTP_CACHE_5MIN", HTTP_CACHE_5MIN },
				{ "HTTP_CACHE_10MIN", HTTP_CACHE_10MIN }
			});

	}

}
