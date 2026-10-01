// rev 2026-09-25

using Microsoft.Extensions.Caching.Hybrid;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Специализированный асинхронный REST-клиент для отправки GET, POST, PUT и DELETE запросов 
	/// к Web API с автоматической поддержкой десериализации, современного гибридного кэширования результатов .NET 10 и отмены.
	/// </summary>
	/// <typeparam name="T">Тип доменной модели данных, ожидаемой в ответе от API.</typeparam>
	public partial class WebApiHelper<T>
	{

		private readonly HttpClient _httpClient;
		private readonly HybridCache _cache;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="WebApiHelper{T}"/> с поддержкой гибридного кэширования и REST-операций.
		/// </summary>
		/// <param name="httpClient">Экземпляр HTTP-клиента (рекомендуется использовать фабричный HttpClient).</param>
		/// <param name="baseUrl">Базовый URL-адрес целевого API шлюза.</param>
		/// <param name="cache">Экземпляр новой службы гибридного кэширования <see cref="HybridCache"/>.</param>
		/// <param name="jsonOptions">Опциональные параметры конфигурации сериализации JSON.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни записей гибридного кэша по умолчанию.</param>
		/// <param name="jsonTypeInfo">Опциональные метаданные типа Source Generation для высокопроизводительной десериализации.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если <paramref name="httpClient"/>, <paramref name="baseUrl"/> или <paramref name="cache"/> равны <see langword="null"/>.
		/// </exception>
		public WebApiHelper(
			HttpClient httpClient,
			string baseUrl,
			HybridCache cache,
			JsonSerializerOptions? jsonOptions = null,
			HybridCacheEntryOptions? cacheOptions = null,
			JsonTypeInfo<T>? jsonTypeInfo = null)
		{
			ArgumentNullException.ThrowIfNull(httpClient);
			ArgumentNullException.ThrowIfNull(baseUrl);
			ArgumentNullException.ThrowIfNull(cache);
			_httpClient = httpClient;
			_cache = cache;
			BaseUrl = baseUrl;
			JsonOptions = jsonOptions ?? SuppJson.DEFAULT_JSON_SERIALIZER_OPTIONS;
			CacheOptions = cacheOptions;
			JsonTypeInfo = jsonTypeInfo;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает базовый URL-адрес целевого API.
		/// </summary>
		public string BaseUrl { get; }


		/// <summary>
		/// Возвращает настройки сериализатора JSON, используемые при обработке запросов и ответов.
		/// </summary>
		public JsonSerializerOptions JsonOptions { get; }


		/// <summary>
		/// Возвращает метаданные типа компиляции Source Generation.
		/// </summary>
		public JsonTypeInfo<T>? JsonTypeInfo { get; }


		/* properties */


		/// <summary>
		/// Возвращает или задает параметры времени жизни записей кэша по умолчанию для текущего хелпера.
		/// </summary>
		public HybridCacheEntryOptions? CacheOptions { get; set; }


		/// <summary>
		/// Построитель параметров строки URL-запроса.
		/// </summary>
		public ParamsBuilder Params { get; } = new();


		/* functions */


		/// <summary>
		/// Выполняет асинхронный GET-запрос к API с автоматической проверкой, блокировкой сквозных запросов (stampede protection) и наполнением гибридного кэша.
		/// </summary>
		/// <param name="queryString">Часть URL-строки запроса с параметрами (например, <c>"?id=10"</c>).</param>
		/// <param name="encoding">Кастомная кодировка текста ответа. Если <see langword="null"/> — используется UTF-8.</param>
		/// <param name="configureRequest">Делегат для кастомизации заголовков запроса перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Поток-задача, возвращающая объект ответа <see cref="WebApiResult{T}"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="queryString"/> равен <see langword="null"/>.</exception>
		public virtual async Task<WebApiResult<T>> SendGetAsync(
			string queryString,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(queryString);
			string cacheKey1 = _getCacheKey(queryString);
			return await _cache.GetOrCreateAsync(
				cacheKey1,
				async token => await _sendInternalAsync(
					HttpMethod.Get, queryString, null, encoding, configureRequest, token),
				CacheOptions,
				cancellationToken: cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный GET-запрос с автоматической проверкой гибридного кэша,
		/// используя параметры, накопленные в свойстве <see cref="Params"/>.
		/// </summary>
		public Task<WebApiResult<T>> SendGetAsync(
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			return SendGetAsync(
				Params.ToString(), encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный POST-запрос к API, передавая payload объект в формате JSON. Результаты операции не кэшируются.
		/// </summary>
		public virtual Task<WebApiResult<T>> SendPostAsync(
			string queryString,
			object payload,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(queryString);
			ArgumentNullException.ThrowIfNull(payload);
			return _sendInternalAsync(
				HttpMethod.Post, queryString, payload, encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный PUT-запрос к API, передавая payload объект в формате JSON.
		/// </summary>
		public virtual Task<WebApiResult<T>> SendPutAsync(
			string queryString,
			object payload,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(queryString);
			ArgumentNullException.ThrowIfNull(payload);
			return _sendInternalAsync(
				HttpMethod.Put, queryString, payload, encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный DELETE-запрос к API.
		/// </summary>
		public virtual Task<WebApiResult<T>> SendDeleteAsync(
			string queryString,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(queryString);
			return _sendInternalAsync(
				HttpMethod.Delete, queryString, null, encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Асинхронно аннулирует (удаляет) из гибридного кэша (L1/L2) запись GET-запроса для конкретной строки параметров.
		/// </summary>
		public ValueTask InvalidateHelperCacheAsync(
			string queryString,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(queryString);
			return _cache.RemoveAsync(
				_getCacheKey(queryString), cancellationToken);
		}


		/// <summary>
		/// Асинхронно аннулирует из гибридного кэша запись GET-запроса для текущего состояния параметров в <see cref="Params"/>.
		/// </summary>
		public ValueTask InvalidateHelperCacheAsync(
			CancellationToken cancellationToken = default)
		{
			return InvalidateHelperCacheAsync(
				Params.ToString(), cancellationToken);
		}


		/* privates */


		private async Task<WebApiResult<T>> _sendInternalAsync(
			HttpMethod method,
			string queryString,
			object? payload,
			Encoding? encoding,
			Action<HttpRequestMessage>? configureRequest,
			CancellationToken cancellationToken)
		{
			var result1 = new WebApiResult<T>();
			HttpResponseMessage? response1 = null;
			try
			{
				using var request1 = new HttpRequestMessage(method, $"{BaseUrl}{queryString}");
				configureRequest?.Invoke(request1);
				if (payload != null)
				{
					string jsonBody1 = (JsonTypeInfo != null && payload is T castedPayload)
						? SuppJson.GetJsonStringFromObjectGen(castedPayload, JsonTypeInfo)
						: SuppJson.GetJsonStringFromObject(payload, JsonOptions);
					request1.Content = new StringContent(
						jsonBody1, encoding ?? Encoding.UTF8, "application/json");
				}
				response1 = await _httpClient.SendAsync(request1, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
				result1.StatusCode = response1.StatusCode;
				result1.Headers = response1.Headers;
				if (response1.IsSuccessStatusCode)
				{
					try
					{
						using var stream1 = await response1.Content.ReadAsStreamAsync(cancellationToken);
						if (JsonTypeInfo != null && (encoding == null || encoding.Equals(Encoding.UTF8)))
							result1.Content = await SuppJson.GetObjectFromJsonStreamGenAsync(stream1, JsonTypeInfo);
						else
						{
							if (encoding == null || encoding.Equals(Encoding.UTF8))
								result1.Content = await SuppJson.GetObjectFromJsonStreamAsync<T>(stream1, JsonOptions);
							else
							{
								using var reader1 = new StreamReader(stream1, encoding);
								string rawJson1 = await reader1.ReadToEndAsync(cancellationToken);
								result1.Content = JsonTypeInfo != null
									? SuppJson.GetObjectFromJsonStringGen(rawJson1, JsonTypeInfo)
									: SuppJson.GetObjectFromJsonString<T>(rawJson1, JsonOptions);
							}
						}
					}
					catch (JsonException ex)
					{
						result1.IsDeserializationError = true;
						result1.DeserializationException = ex;
						result1.ErrorBody = await response1.Content.ReadAsStringAsync(cancellationToken);
					}
				}
				else
					result1.ErrorBody = await response1.Content.ReadAsStringAsync(cancellationToken);
			}
			catch (HttpRequestException ex)
			{
				if (response1 != null)
				{
					result1.StatusCode = response1.StatusCode;
					result1.Headers = response1.Headers;
				}
				result1.ErrorBody = $"[Ans.Net10.Common] Сетевая ошибка REST-клиента (HttpRequestException): {ex.Message}";
			}
			catch (TaskCanceledException ex)
			{
				result1.ErrorBody = cancellationToken.IsCancellationRequested
					? "[Ans.Net10.Common] Операция удаленного вызова была отменена пользователем."
					: $"[Ans.Net10.Common] Превышено время ожидания ответа от API шлюза (Timeout): {ex.Message}";
			}
			return result1;
		}


				private string _getCacheKey(
			string queryString)
		{
			return $"api_helper_cache:{BaseUrl}{queryString}";
		}

	}

}
