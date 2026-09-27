// rev 2026-09-26

using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для <see cref="HttpClient"/>, обеспечивающие асинхронное получение и автоматическую десериализацию данных в форматах JSON, XML и GRID.
	/// </summary>
	public static partial class Exts_HttpClient
	{

		/// <summary>
		/// Выполняет асинхронный GET-запрос с поддержкой нативного кэширования <see cref="IMemoryCache"/> 
		/// и десериализует JSON-ответ в объект типа T. Безопасно обрабатывает сетевые сбои и ошибки парсинга.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Интерфейс службы нативного кэширования в памяти.</param>
		/// <param name="options">Опциональные параметры конфигурации десериализации JSON. Если <see langword="null"/> — используются параметры по умолчанию.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи кэша. Если <see langword="null"/> — применяются дефолтные опции кэша.</param>
		/// <param name="encoding">Кастомная кодировка текста (например, Windows-1251). Если <see langword="null"/> — используется стандартный UTF-8.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> (например, добавления заголовков авторизации Bearer).</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток, содержащий объект ответа <see cref="WebApiResult{T}"/> с десериализованными данными из кэша или напрямую от API, либо метаданные возникшей ошибки.
		/// </returns>
		public static async Task<WebApiResult<T>> GetJsonResultAsync<T>(
			this HttpClient client,
			string requestUri,
			IMemoryCache cache,
			JsonSerializerOptions? options = null,
			MemoryCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			string cacheKey1 = _getCacheKey("json", client.BaseAddress?.ToString(), requestUri);
			if (cache.TryGetValue(cacheKey1, out WebApiResult<T>? cachedResult) && cachedResult != null)
				return cachedResult;
			var result1 = new WebApiResult<T>();
			var rawResult1 = await _executeRequestAsync(client, requestUri, configureRequest, cancellationToken);
			result1.StatusCode = rawResult1.StatusCode;
			result1.Headers = rawResult1.Headers;
			result1.ErrorBody = rawResult1.ErrorBody;
			if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
			{
				try
				{
					using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(cancellationToken);
					if (encoding == null || encoding.Equals(Encoding.UTF8))
						result1.Content = await SuppJson.GetObjectFromJsonStreamAsync<T>(stream1, options);
					else
					{
						using var reader1 = new StreamReader(stream1, encoding);
						result1.Content = SuppJson.GetObjectFromJsonString<T>(
							await reader1.ReadToEndAsync(cancellationToken), options);
					}
				}
				catch (JsonException ex)
				{
					result1.IsDeserializationError = true;
					result1.DeserializationException = ex;
					result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
				}
			}
			else if (rawResult1.Content != null)
				result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
			if (result1.StatusCode == HttpStatusCode.OK
				&& result1.Content != null
				&& !result1.IsDeserializationError)
				cache.Set(cacheKey1, result1, cacheOptions ?? SuppCache.DEFAULT_CACHE_OPTIONS);
			return result1;
		}


		/// <summary>
		/// (Source Gen) Выполняет асинхронный GET-запрос с поддержкой нативного кэширования <see cref="IMemoryCache"/> 
		/// и десериализует JSON-ответ без использования рантайм-рефлексии. Безопасно обрабатывает сетевые сбои.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Интерфейс службы нативного кэширования в памяти.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи кэша. Если <see langword="null"/> — применяются дефолтные опции кэша.</param>
		/// <param name="encoding">Кастомная кодировка текста (например, Windows-1251). Если <see langword="null"/> — используется стандартный UTF-8.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> (например, добавления заголовков авторизации Bearer).</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток, содержащий объект ответа <see cref="WebApiResult{T}"/> со сгенерированными на этапе сборки десериализованными данными, либо метаданные ошибки.
		/// </returns>
		public static async Task<WebApiResult<T>> GetJsonGenResultAsync<T>(
			this HttpClient client,
			string requestUri,
			IMemoryCache cache,
			JsonTypeInfo<T> jsonTypeInfo,
			MemoryCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			string cacheKey1 = _getCacheKey("jsongen", client.BaseAddress?.ToString(), requestUri);
			if (cache.TryGetValue(cacheKey1, out WebApiResult<T>? cachedResult) && cachedResult != null)
				return cachedResult;
			var result1 = new WebApiResult<T>();
			var rawResult1 = await _executeRequestAsync(client, requestUri, configureRequest, cancellationToken);
			result1.StatusCode = rawResult1.StatusCode;
			result1.Headers = rawResult1.Headers;
			result1.ErrorBody = rawResult1.ErrorBody;
			if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
			{
				try
				{
					using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(cancellationToken);
					if (encoding == null || encoding.Equals(Encoding.UTF8))
						result1.Content = await SuppJson.GetObjectFromJsonStreamGenAsync(stream1, jsonTypeInfo);
					else
					{
						using var reader1 = new StreamReader(stream1, encoding);
						result1.Content = SuppJson.GetObjectFromJsonStringGen(
							await reader1.ReadToEndAsync(cancellationToken), jsonTypeInfo);
					}
				}
				catch (JsonException ex)
				{
					result1.IsDeserializationError = true;
					result1.DeserializationException = ex;
					result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
				}
			}
			else if (rawResult1.Content != null)
				result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
			if (result1.StatusCode == HttpStatusCode.OK
				&& result1.Content != null
				&& !result1.IsDeserializationError)
				cache.Set(cacheKey1, result1, cacheOptions ?? SuppCache.DEFAULT_CACHE_OPTIONS);
			return result1;
		}


		/// <summary>
		/// Выполняет асинхронный GET-запрос с поддержкой нативного кэширования <see cref="IMemoryCache"/> 
		/// и десериализует XML-ответ в объект типа T. Безопасно перехватывает ошибки валидации схемы.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется XML-документ.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Интерфейс службы нативного кэширования в памяти.</param>
		/// <param name="defaultNamespace">Пространство имен XML по умолчанию, используемое при десериализации.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи кэша. Если <see langword="null"/> — применяются дефолтные опции кэша.</param>
		/// <param name="encoding">Кастомная кодировка текста (например, Windows-1251). Если <see langword="null"/> — используется стандартный UTF-8.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> (например, добавления заголовков авторизации Bearer).</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток, содержащий объект ответа <see cref="WebApiResult{T}"/> с десериализованными из XML-схемы данными, либо метаданные возникшей ошибки.
		/// </returns>
		public static async Task<WebApiResult<T>> GetXmlResultAsync<T>(
			this HttpClient client,
			string requestUri,
			IMemoryCache cache,
			string? defaultNamespace = null,
			MemoryCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			string cacheKey1 = _getCacheKey("xml", client.BaseAddress?.ToString(), requestUri);
			if (cache.TryGetValue(cacheKey1, out WebApiResult<T>? cachedResult) && cachedResult != null)
				return cachedResult;
			var result1 = new WebApiResult<T>();
			var rawResult1 = await _executeRequestAsync(client, requestUri, configureRequest, cancellationToken);
			result1.StatusCode = rawResult1.StatusCode;
			result1.Headers = rawResult1.Headers;
			result1.ErrorBody = rawResult1.ErrorBody;
			if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
			{
				try
				{
					using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(cancellationToken);
					if (encoding == null)
						result1.Content = SuppXml.GetObjectFromXmlStream<T>(stream1, defaultNamespace);
					else
					{
						using var reader1 = new StreamReader(stream1, encoding);
						result1.Content = SuppXml.GetObjectFromXmlString<T>(
							await reader1.ReadToEndAsync(cancellationToken), defaultNamespace);
					}
				}
				catch (InvalidOperationException ex)
				{
					result1.IsDeserializationError = true;
					result1.DeserializationException = ex;
					result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
				}
			}
			else if (rawResult1.Content != null)
				result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
			if (result1.StatusCode == HttpStatusCode.OK
				&& result1.Content != null
				&& !result1.IsDeserializationError)
				cache.Set(cacheKey1, result1, cacheOptions ?? SuppCache.DEFAULT_CACHE_OPTIONS);
			return result1;
		}


		/// <summary>
		/// Выполняет асинхронный GET-запрос с поддержкой нативного кэширования <see cref="IMemoryCache"/>, 
		/// безопасно парсит построчные текстовые данные в формате GRID и проецирует их в материализованную коллекцию объектов.
		/// </summary>
		/// <typeparam name="T">Тип результирующих объектов, из которых состоит выходная коллекция.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Интерфейс службы нативного кэширования в памяти.</param>
		/// <param name="selector">Делегат-селектор для построчного маппинга сырых полей <see cref="GridRowParser"/> в целевой объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга чисел и дат внутри строк.</param>
		/// <param name="encoding">Кастомная кодировка текста (например, Windows-1251). Если <see langword="null"/> — используется стандартный UTF-8.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи кэша. Если <see langword="null"/> — применяются дефолтные опции кэша.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> (например, добавления заголовков авторизации Bearer).</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Поток, содержащий объект ответа <see cref="WebApiResult{T}"/> с материализованной коллекцией структурированных строк, либо метаданные ошибки.
		/// </returns>
		public static async Task<WebApiResult<IEnumerable<T>>> GetGridResultAsync<T>(
			this HttpClient client,
			string requestUri,
			IMemoryCache cache,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null,
			MemoryCacheEntryOptions? cacheOptions = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			string cacheKey1 = _getCacheKey("grid", client.BaseAddress?.ToString(), requestUri);
			if (cache.TryGetValue(cacheKey1, out WebApiResult<IEnumerable<T>>? cachedResult) && cachedResult != null)
				return cachedResult;
			var result1 = new WebApiResult<IEnumerable<T>>();
			var rawResult1 = await _executeRequestAsync(client, requestUri, configureRequest, cancellationToken);
			result1.StatusCode = rawResult1.StatusCode;
			result1.Headers = rawResult1.Headers;
			result1.ErrorBody = rawResult1.ErrorBody;
			if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
			{
				try
				{
					using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(cancellationToken);
					result1.Content = [.. SuppGrid.GetItemsFromStream(stream1, selector, provider, encoding)];
				}
				catch (Exception ex) when (ex is FormatException or IndexOutOfRangeException)
				{
					result1.IsDeserializationError = true;
					result1.DeserializationException = ex;
					result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
				}
			}
			else if (rawResult1.Content != null)
				result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(cancellationToken);
			if (result1.StatusCode == HttpStatusCode.OK
				&& result1.Content != null
				&& !result1.IsDeserializationError)
				cache.Set(cacheKey1, result1, cacheOptions ?? SuppCache.DEFAULT_CACHE_OPTIONS);
			return result1;
		}


		/// <summary>
		/// Принудительно удаляет (инвалидирует) ранее сохраненную запись HTTP-ответа из нативного кэша по заданному формату и URI ресурса.
		/// </summary>
		/// <param name="client">Экземпляр HTTP-клиента, для адреса которого очищается кэш.</param>
		/// <param name="format">Строковый идентификатор формата данных, использованный при кэшировании (например, <c>"json"</c>, <c>"xml"</c>, <c>"grid"</c>).</param>
		/// <param name="requestUri">Относительный или абсолютный URI запроса, для которого требуется аннулировать кэш.</param>
		/// <param name="cache">Интерфейс службы нативного кэширования, из которой удаляется запись.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvalidateHttpCache(
			this HttpClient client,
			string format,
			string requestUri,
			IMemoryCache cache)
		{
			cache.Remove(_getCacheKey(format, client.BaseAddress?.ToString(), requestUri));
		}


		/* privates */


		private static async Task<WebApiResult<HttpResponseMessage>> _executeRequestAsync(
			HttpClient client,
			string requestUri,
			Action<HttpRequestMessage>? configureRequest,
			CancellationToken cancellationToken)
		{
			var result = new WebApiResult<HttpResponseMessage>();
			HttpResponseMessage? response = null;
			try
			{
				using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
				configureRequest?.Invoke(request);
				response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
				result.StatusCode = response.StatusCode;
				result.Headers = response.Headers;
				result.Content = response;
			}
			catch (HttpRequestException ex)
			{
				if (response != null)
				{
					result.StatusCode = response.StatusCode;
					result.Headers = response.Headers;
				}
				result.ErrorBody = $"[Ans.Net10.Common] Сетевая ошибка HTTP (HttpRequestException): {ex.Message}";
			}
			catch (TaskCanceledException ex)
			{
				result.ErrorBody = cancellationToken.IsCancellationRequested
					? "[Ans.Net10.Common] Операция HTTP-запроса была отменена пользователем."
					: $"[Ans.Net10.Common] Таймаут HTTP-запроса (Timeout): {ex.Message}";
			}
			return result;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string _getCacheKey(
			string format,
			string? baseAddress,
			string requestUri)
		{
			return $"http_cache:{format}:{baseAddress}{requestUri}";
		}

	}

}
