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


		public const string HTTP_CACHE_NONE_NAME = "HTTP_CACHE_NONE";
		public const string HTTP_CACHE_30SEC_NAME = "HTTP_CACHE_30SEC";
		public const string HTTP_CACHE_1MIN_NAME = "HTTP_CACHE_1MIN";
		public const string HTTP_CACHE_5MIN_NAME = "HTTP_CACHE_5MIN";
		public const string HTTP_CACHE_10MIN_NAME = "HTTP_CACHE_10MIN";


		/// <summary>
		/// Профиль, полностью запрещающий кэширование HTTP-ответа на стороне клиента и прокси-серверов.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_NONE_PROFILE = new()
		{
			Duration = 0,
			Location = ResponseCacheLocation.None,
			NoStore = true
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 30 секунд.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_30SEC_PROFILE = new()
		{
			Duration = 30,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 1 минуту.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_1MIN_PROFILE = new()
		{
			Duration = 60,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 5 минут.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_5MIN_PROFILE = new()
		{
			Duration = 300,
			Location = ResponseCacheLocation.Any,
			NoStore = false,
			VaryByQueryKeys = ["*"]
		};


		/// <summary>
		/// Профиль кэширования HTTP-ответа на 10 минут.
		/// </summary>
		public static readonly CacheProfile HTTP_CACHE_10MIN_PROFILE = new()
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
				{ HTTP_CACHE_NONE_NAME, HTTP_CACHE_NONE_PROFILE },
				{ HTTP_CACHE_30SEC_NAME, HTTP_CACHE_30SEC_PROFILE },
				{ HTTP_CACHE_1MIN_NAME, HTTP_CACHE_1MIN_PROFILE },
				{ HTTP_CACHE_5MIN_NAME, HTTP_CACHE_5MIN_PROFILE },
				{ HTTP_CACHE_10MIN_NAME, HTTP_CACHE_10MIN_PROFILE }
			});

	}

}
