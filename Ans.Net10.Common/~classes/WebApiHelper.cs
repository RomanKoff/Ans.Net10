// rev 2026-09-25

using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет конечный результат выполнения запроса к Web API, расширяющий базовый результат.
	/// </summary>
	/// <typeparam name="T">Тип ожидаемых полезных данных ответа.</typeparam>
	public class WebApiResult<T>
		: _WebResult_Base<T>
	{
	}



	/// <summary>
	/// Специализированный асинхронный REST-клиент для отправки GET, POST, PUT и DELETE запросов 
	/// к Web API с автоматической поддержкой десериализации, кэширования результатов, кастомных кодировок и отмены.
	/// </summary>
	/// <typeparam name="T">Тип доменной модели данных, ожидаемой в ответе от API.</typeparam>
	public class WebApiHelper<T>
	{

		private readonly HttpClient _httpClient;
		private readonly IMemoryCache _cache;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="WebApiHelper{T}"/> с поддержкой кэширования и REST-операций.
		/// </summary>
		/// <param name="httpClient">Экземпляр HTTP-клиента (рекомендуется использовать фабричный HttpClient).</param>
		/// <param name="baseUrl">Базовый URL-адрес целевого API шлюза.</param>
		/// <param name="cache">Интерфейс службы нативного кэширования в памяти.</param>
		/// <param name="jsonOptions">
		/// Опциональные параметры конфигурации сериализации JSON.
		/// Если передано значение <see langword="null"/> — используется <see cref="SuppJson.DEFAULT_JSON_SERIALIZER_OPTIONS"/>.
		/// </param>
		/// <param name="cacheOptions">
		/// Опциональные параметры времени жизни записей кэша по умолчанию. 
		/// Если передано значение <see langword="null"/> — используется <see cref="SuppCache.DEFAULT_CACHE_OPTIONS"/>.
		/// </param>
		/// <param name="jsonTypeInfo">
		/// Опциональные метаданные типа Source Generation для высокопроизводительной десериализации без рефлексии.
		/// </param>
		public WebApiHelper(
			HttpClient httpClient,
			string baseUrl,
			IMemoryCache cache,
			JsonSerializerOptions? jsonOptions = null,
			MemoryCacheEntryOptions? cacheOptions = null,
			JsonTypeInfo<T>? jsonTypeInfo = null)
		{
			_httpClient = httpClient;
			_cache = cache;
			BaseUrl = baseUrl;
			JsonOptions = jsonOptions ?? SuppJson.DEFAULT_JSON_SERIALIZER_OPTIONS;
			CacheOptions = cacheOptions ?? SuppCache.DEFAULT_CACHE_OPTIONS;
			JsonTypeInfo = jsonTypeInfo;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает базовый URL-адрес целевого API.
		/// </summary>
		/// <value>Строковое представление базового адреса удаленного шлюза.</value>
		public string BaseUrl { get; }


		/// <summary>
		/// Возвращает настройки сериализатора JSON, используемые при обработке запросов и ответов.
		/// </summary>
		/// <value>Объект <see cref="JsonSerializerOptions"/> конфигурации парсера.</value>
		public JsonSerializerOptions JsonOptions { get; }


		/// <summary>
		/// Возвращает метаданные типа компиляции Source Generation.
		/// </summary>
		/// <value>Экземпляр <see cref="JsonTypeInfo{T}"/> или значение <see langword="null"/>, если используется классическая рефлексия.</value>
		public JsonTypeInfo<T>? JsonTypeInfo { get; }


		/// <summary>
		/// Возвращает или задает параметры времени жизни и ограничений записей кэша по умолчанию для текущего хелпера.
		/// </summary>
		/// <value>Объект <see cref="MemoryCacheEntryOptions"/> конфигурации времени удержания данных в памяти.</value>
		public MemoryCacheEntryOptions? CacheOptions { get; set; }


		/// <summary>
		/// Построитель параметров строки URL-запроса.
		/// </summary>
		/// <value>Объект-строитель <see cref="ParamsBuilder"/> параметров адресной строки.</value>
		public ParamsBuilder Params { get; } = new();


		/* functions */


		/// <summary>
		/// Выполняет асинхронный GET-запрос к API с автоматической проверкой и наполнением нативного кэша.
		/// </summary>
		/// <param name="queryString">Часть URL-строки запроса с параметрами (например, <c>"?id=10"</c>).</param>
		/// <param name="encoding">Кастомная кодировка текста ответа. Если <see langword="null"/> — используется UTF-8.</param>
		/// <param name="configureRequest">Делегат для кастомизации заголовков запроса перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток-задача, возвращающая объект ответа <see cref="WebApiResult{T}"/> из памяти кэша или напрямую от удаленного API.
		/// </returns>
		public virtual async Task<WebApiResult<T>> SendGetAsync(
			string queryString,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			string cacheKey1 = _getCacheKey(queryString);
			if (_cache.TryGetValue(cacheKey1, out WebApiResult<T>? cachedResult) && cachedResult != null)
				return cachedResult;
			var freshResult1 = await _sendInternalAsync(
				HttpMethod.Get, queryString, null, encoding, configureRequest, cancellationToken);
			if (freshResult1.StatusCode == HttpStatusCode.OK
				&& freshResult1.Content != null
				&& !freshResult1.IsDeserializationError)
				_cache.Set(cacheKey1, freshResult1, CacheOptions);
			return freshResult1;
		}


		/// <summary>
		/// Выполняет асинхронный GET-запрос с автоматической проверкой кэша,
		/// используя параметры, накопленные в свойстве <see cref="Params"/>.
		/// </summary>
		/// <param name="encoding">Кастомная кодировка текста ответа. Если <see langword="null"/> — используется UTF-8.</param>
		/// <param name="configureRequest">Делегат для кастомизации заголовков запроса перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток-задача, возвращающая объект ответа <see cref="WebApiResult{T}"/> из памяти кэша или напрямую от удаленного API.
		/// </returns>		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Task<WebApiResult<T>> SendGetAsync(
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			return SendGetAsync(
				Params.ToString(), encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный POST-запрос (создание ресурса) к API, передавая payload объект в формате JSON.
		/// Результаты операции POST не подвергаются кэшированию.
		/// </summary>
		/// <param name="queryString">Относительный URI путь или параметры запроса.</param>
		/// <param name="payload">Объект данных, сериализуемый и отправляемый в теле запроса (Body).</param>
		/// <param name="encoding">Кодировка исходящего и входящего текста ответа. Если <see langword="null"/> — используется UTF-8.</param>
		/// <param name="configureRequest">Делегат для кастомизации заголовков запроса перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток-задача, возвращающая объект ответа <see cref="WebApiResult{T}"/> с результатами выполнения со стороны сервера.
		/// </returns>
		public virtual Task<WebApiResult<T>> SendPostAsync(
			string queryString,
			object payload,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			return _sendInternalAsync(
				HttpMethod.Post, queryString, payload, encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный PUT-запрос (полное обновление ресурса) к API, передавая payload объект в формате JSON.
		/// </summary>
		/// <param name="queryString">Относительный URI путь или параметры запроса.</param>
		/// <param name="payload">Объект данных, отправляемый в теле запроса (Body) для обновления.</param>
		/// <param name="encoding">Кодировка исходящего и входящего текста. Если <see langword="null"/> — используется UTF-8.</param>
		/// <param name="configureRequest">Делегат для кастомизации заголовков запроса перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток-задача, возвращающая объект ответа <see cref="WebApiResult{T}"/> с результатами выполнения.
		/// </returns>
		public virtual Task<WebApiResult<T>> SendPutAsync(
			string queryString,
			object payload,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			return _sendInternalAsync(
				HttpMethod.Put, queryString, payload, encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный DELETE-запрос (удаление ресурса) к API.
		/// </summary>
		/// <param name="queryString">Относительный URI путь или параметры идентификации удаляемого ресурса.</param>
		/// <param name="encoding">Кодировка входящего текста ответа. Если <see langword="null"/> — используется UTF-8.</param>
		/// <param name="configureRequest">Делегат для кастомизации заголовков запроса перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Поток-задача, возвращающая объект ответа <see cref="WebApiResult{T}"/>.</returns>
		public virtual Task<WebApiResult<T>> SendDeleteAsync(
			string queryString,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			return _sendInternalAsync(
				HttpMethod.Delete, queryString, null, encoding, configureRequest, cancellationToken);
		}


		/// <summary>
		/// Принудительно аннулирует (удаляет) из нативного кэша запись GET-запроса для конкретной строки параметров.
		/// </summary>
		/// <param name="queryString">Часть URL-строки запроса с параметрами.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InvalidateHelperCache(
			string queryString)
		{
			_cache.Remove(_getCacheKey(queryString));
		}


		/// <summary>
		/// Принудительно аннулирует из нативного кэша запись GET-запроса для текущего состояния параметров в <see cref="Params"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InvalidateHelperCache()
		{
			InvalidateHelperCache(Params.ToString());
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
				response1 = await _httpClient.SendAsync(
					request1, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
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


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private string _getCacheKey(
			string queryString)
		{
			return $"api_helper_cache:{BaseUrl}{queryString}";
		}


	}

}
