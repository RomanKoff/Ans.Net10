// rev 2026-09-26

using Microsoft.Extensions.Caching.Hybrid;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для <see cref="HttpClient"/>, обеспечивающие асинхронное получение 
	/// и автоматическую десериализацию данных в форматах JSON, XML и GRID с поддержкой гибридного кэширования .NET 10.
	/// </summary>
	public static partial class Exts_HttpClient
	{

		/// <summary>
		/// Выполняет асинхронный GET-запрос с поддержкой современного гибридного кэширования <see cref="HybridCache"/> 
		/// и десериализует JSON-ответ в объект типа T. Безопасно обрабатывает сетевые сбои и ошибки парсинга.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Служба гибридного кэширования платформы .NET 10.</param>
		/// <param name="jsonOptions">Опциональные параметры конфигурации десериализации JSON.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи гибридного кэша.</param>
		/// <param name="encoding">Кастомная кодировка текста. Если <see langword="null"/> — используется стандартный UTF-8.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Объект ответа <see cref="WebApiResult{T}"/> с десериализованными данными из кэша или напрямую от API.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="client"/>, <paramref name="requestUri"/> или <paramref name="cache"/> равны <see langword="null"/>.</exception>
		public static async Task<WebApiResult<T>> GetJsonResultAsync<T>(
			this HttpClient client,
			string requestUri,
			HybridCache cache,
			JsonSerializerOptions? jsonOptions = null,
			HybridCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(client);
			ArgumentNullException.ThrowIfNull(requestUri);
			ArgumentNullException.ThrowIfNull(cache);
			string cacheKey1 = _getCacheKey("json", client.BaseAddress?.ToString(), requestUri);
			return await cache.GetOrCreateAsync(
				cacheKey1,
				async token1 =>
				{
					var result1 = new WebApiResult<T>();
					var rawResult1 = await _executeRequestAsync(
						client, requestUri, configureRequest, token1);
					result1.StatusCode = rawResult1.StatusCode;
					result1.Headers = rawResult1.Headers;
					result1.ErrorBody = rawResult1.ErrorBody;
					if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
					{
						try
						{
							using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(token1);
							if (encoding == null || encoding.Equals(Encoding.UTF8))
								result1.Content = await SuppJson.GetObjectFromJsonStreamAsync<T>(stream1, jsonOptions);
							else
							{
								using var reader1 = new StreamReader(stream1, encoding);
								result1.Content = SuppJson.GetObjectFromJsonString<T>(
									await reader1.ReadToEndAsync(token1), jsonOptions);
							}
						}
						catch (JsonException ex)
						{
							result1.IsDeserializationError = true;
							result1.DeserializationException = ex;
							result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
						}
					}
					else if (rawResult1.Content != null)
						result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
					return result1;
				},
				cacheOptions,
				cancellationToken: cancellationToken);
		}


		/// <summary>
		/// (Source Gen) Выполняет асинхронный GET-запрос с поддержкой современного гибридного кэширования <see cref="HybridCache"/> 
		/// и десериализует JSON-ответ в объект типа <typeparamref name="T"/> без использования рантайм-рефлексии на основе сгенерированных метаданных. 
		/// Безопасно обрабатывает сетевые сбои и ошибки парсинга.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Служба гибридного кэширования платформы .NET 10.</param>
		/// <param name="jsonTypeInfo">Инфраструктурные метаданные типа со сценарием Source Generation, сгенерированные компилятором на этапе сборки.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи гибридного кэша.</param>
		/// <param name="encoding">Кастомная кодировка текста. Если <see langword="null"/> — используется стандартный UTF-8.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом которой является объект ответа <see cref="WebApiResult{T}"/> с десериализованными данными из кэша или напрямую от API.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="client"/>, <paramref name="requestUri"/>, <paramref name="cache"/> или <paramref name="jsonTypeInfo"/> равен <see langword="null"/>.</exception>
		public static async Task<WebApiResult<T>> GetJsonGenResultAsync<T>(
			this HttpClient client,
			string requestUri,
			HybridCache cache,
			JsonTypeInfo<T> jsonTypeInfo,
			HybridCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(client);
			ArgumentNullException.ThrowIfNull(requestUri);
			ArgumentNullException.ThrowIfNull(cache);
			ArgumentNullException.ThrowIfNull(jsonTypeInfo);
			string cacheKey1 = _getCacheKey("jsongen", client.BaseAddress?.ToString(), requestUri);
			return await cache.GetOrCreateAsync(
				cacheKey1,
				async token1 =>
				{
					var result1 = new WebApiResult<T>();
					var rawResult1 = await _executeRequestAsync(
						client, requestUri, configureRequest, token1);
					result1.StatusCode = rawResult1.StatusCode;
					result1.Headers = rawResult1.Headers;
					result1.ErrorBody = rawResult1.ErrorBody;
					if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
					{
						try
						{
							using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(token1);
							if (encoding == null || encoding.Equals(Encoding.UTF8))
								result1.Content = await SuppJson.GetObjectFromJsonStreamGenAsync(stream1, jsonTypeInfo);
							else
							{
								using var reader1 = new StreamReader(stream1, encoding);
								result1.Content = SuppJson.GetObjectFromJsonStringGen(
									await reader1.ReadToEndAsync(token1), jsonTypeInfo);
							}
						}
						catch (JsonException ex)
						{
							result1.IsDeserializationError = true;
							result1.DeserializationException = ex;
							result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
						}
					}
					else if (rawResult1.Content != null)
						result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
					return result1;
				},
				cacheOptions,
				cancellationToken: cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный GET-запрос с поддержкой современного гибридного кэширования <see cref="HybridCache"/> 
		/// и десериализует XML-ответ в объект типа <typeparamref name="T"/>. Безопасно перехватывает ошибки валидации схемы.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется XML-ответ.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Служба гибридного кэширования платформы .NET 10.</param>
		/// <param name="defaultNamespace">Опциональное пространство имен XML, применяемое при разборе схемы документа.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи гибридного кэша.</param>
		/// <param name="encoding">Кастомная кодировка текста. Если <see langword="null"/> — используется кодировка, определенная XML-десериализатором автоматически.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Задача, результатом которой является объект ответа <see cref="WebApiResult{T}"/> с десериализованными XML-данными из кэша или напрямую от API.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="client"/>, <paramref name="requestUri"/> или <paramref name="cache"/> равен <see langword="null"/>.</exception>
		public static async Task<WebApiResult<T>> GetXmlResultAsync<T>(
			this HttpClient client,
			string requestUri,
			HybridCache cache,
			string? defaultNamespace = null,
			HybridCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(client);
			ArgumentNullException.ThrowIfNull(requestUri);
			ArgumentNullException.ThrowIfNull(cache);
			string cacheKey1 = _getCacheKey("xml", client.BaseAddress?.ToString(), requestUri);
			return await cache.GetOrCreateAsync(
				cacheKey1,
				async token1 =>
				{
					var result1 = new WebApiResult<T>();
					var rawResult1 = await _executeRequestAsync(
						client, requestUri, configureRequest, token1);
					result1.StatusCode = rawResult1.StatusCode;
					result1.Headers = rawResult1.Headers;
					result1.ErrorBody = rawResult1.ErrorBody;
					if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
					{
						try
						{
							using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(token1);
							if (encoding == null)
								result1.Content = SuppXml.GetObjectFromXmlStream<T>(stream1, defaultNamespace);
							else
							{
								using var reader1 = new StreamReader(stream1, encoding);
								result1.Content = SuppXml.GetObjectFromXmlString<T>(
									await reader1.ReadToEndAsync(token1), defaultNamespace);
							}
						}
						catch (InvalidOperationException ex)
						{
							result1.IsDeserializationError = true;
							result1.DeserializationException = ex;
							result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
						}
					}
					else if (rawResult1.Content != null)
						result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
					return result1;
				},
				cacheOptions,
				cancellationToken: cancellationToken);
		}


		/// <summary>
		/// Выполняет асинхронный GET-запрос с поддержкой гибридного кэширования <see cref="HybridCache"/>, 
		/// безопасно и неблокирующе парсит построчные текстовые данные в формате GRID из HTTP-потока 
		/// и проецирует их в материализованную коллекцию объектов.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга.</typeparam>
		/// <param name="client">Экземпляр HTTP-клиента, выполняющий запрос.</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Служба гибридного кэширования платформы .NET 10.</param>
		/// <param name="selector">Функция-предикат (лямбда) для маппинга полей строки в объект типа T.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга полей внутри строки.</param>
		/// <param name="encoding">Опциональная кодировка текста. Если равен <see langword="null"/> — используется UTF-8 без BOM.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и ограничений записи гибридного кэша.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта <see cref="HttpRequestMessage"/> перед отправкой.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Объект ответа <see cref="WebApiResult{T}"/> с материализованной коллекцией данных из кэша или напрямую от API.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="client"/>, <paramref name="requestUri"/>, <paramref name="cache"/> или <paramref name="selector"/> равны <see langword="null"/>.</exception>
		public static async Task<WebApiResult<IEnumerable<T>>> GetGridResultAsync<T>(
			this HttpClient client,
			string requestUri,
			HybridCache cache,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null,
			HybridCacheEntryOptions? cacheOptions = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(client);
			ArgumentNullException.ThrowIfNull(requestUri);
			ArgumentNullException.ThrowIfNull(cache);
			ArgumentNullException.ThrowIfNull(selector);
			string cacheKey1 = _getCacheKey("grid", client.BaseAddress?.ToString(), requestUri);
			return await cache.GetOrCreateAsync(
				cacheKey1,
				async token1 =>
				{
					var result1 = new WebApiResult<IEnumerable<T>>();
					var rawResult1 = await _executeRequestAsync(
						client, requestUri, configureRequest, token1);
					result1.StatusCode = rawResult1.StatusCode;
					result1.Headers = rawResult1.Headers;
					result1.ErrorBody = rawResult1.ErrorBody;
					if (rawResult1.Content != null && rawResult1.Content.IsSuccessStatusCode)
					{
						try
						{
							using var stream1 = await rawResult1.Content.Content.ReadAsStreamAsync(token1);
							var list1 = new List<T>();
							await foreach (var item1 in SuppGrid.GetItemsFromStreamAsync(
								stream1, selector, provider, encoding, token1).ConfigureAwait(false))
								list1.Add(item1);
							result1.Content = list1;
						}
						catch (Exception ex) when (ex is FormatException or IndexOutOfRangeException)
						{
							result1.IsDeserializationError = true;
							result1.DeserializationException = ex;
							result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
						}
					}
					else if (rawResult1.Content != null)
						result1.ErrorBody = await rawResult1.Content.Content.ReadAsStringAsync(token1);
					return result1;
				},
				cacheOptions,
				cancellationToken: cancellationToken);
		}


		/// <summary>
		/// Принудительно асинхронно удаляет (инвалидирует) ранее сохраненную запись HTTP-ответа из L1/L2 уровней гибридного кэша 
		/// по вычисленному составному ключу на основе формата и URI ресурса.
		/// </summary>
		/// <param name="client">Экземпляр HTTP-клиента, чей базовый адрес используется для формирования ключа кэша.</param>
		/// <param name="format">Строковый идентификатор формата кэшируемых данных (например, "json", "xml", "grid").</param>
		/// <param name="requestUri">Относительный или абсолютный URI целевого ресурса.</param>
		/// <param name="cache">Служба гибридного кэширования платформы .NET 10.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>Структура <see cref="ValueTask"/>, представляющая асинхронную операцию удаления записи из кэша.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="client"/>, <paramref name="format"/>, <paramref name="requestUri"/> или <paramref name="cache"/> равен <see langword="null"/>.</exception>
		public static ValueTask InvalidateHttpCacheAsync(
			this HttpClient client,
			string format,
			string requestUri,
			HybridCache cache,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(client);
			ArgumentNullException.ThrowIfNull(format);
			ArgumentNullException.ThrowIfNull(requestUri);
			ArgumentNullException.ThrowIfNull(cache);
			return cache.RemoveAsync(
				_getCacheKey(format, client.BaseAddress?.ToString(), requestUri), cancellationToken);
		}


		/* privates */


		private static async Task<WebApiResult<HttpResponseMessage>> _executeRequestAsync(
			HttpClient client,
			string requestUri,
			Action<HttpRequestMessage>? configureRequest,
			CancellationToken cancellationToken)
		{
			var result1 = new WebApiResult<HttpResponseMessage>();
			HttpResponseMessage? response1 = null;
			try
			{
				using var request1 = new HttpRequestMessage(HttpMethod.Get, requestUri);
				configureRequest?.Invoke(request1);
				response1 = await client.SendAsync(request1, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
				result1.StatusCode = response1.StatusCode;
				result1.Headers = response1.Headers;
				result1.Content = response1;
			}
			catch (HttpRequestException ex)
			{
				if (response1 != null)
				{
					result1.StatusCode = response1.StatusCode;
					result1.Headers = response1.Headers;
				}
				result1.ErrorBody = $"[Ans.Net10.Common] Сетевая ошибка HTTP (HttpRequestException): {ex.Message}";
			}
			catch (TaskCanceledException ex)
			{
				result1.ErrorBody = cancellationToken.IsCancellationRequested
					? "[Ans.Net10.Common] Операция HTTP-запроса была отменена пользователем."
					: $"[Ans.Net10.Common] Таймаут HTTP-запроса (Timeout): {ex.Message}";
			}
			return result1;
		}


		private static string _getCacheKey(
			string format,
			string? baseAddress,
			string requestUri)
		{
			return $"http_cache:{format}:{baseAddress}{requestUri}";
		}

	}

}
