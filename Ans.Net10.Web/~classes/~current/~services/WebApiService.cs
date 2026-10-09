// rev 2026-10-07

using Ans.Net10.Common;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба для выполнения интеграционных запросов к внешним Web API 
	/// с поддержкой гибридного кэширования ответов .NET 10.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="WebApiService"/> с использованием первичного конструктора C#.
	/// </remarks>
	/// <param name="current">Текущий оркестровый контекст обработки запроса.</param>
	/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="current"/> равен <see langword="null"/>.</exception>
	public class WebApiService(
		CurrentContext current)
	{

		private readonly CurrentContext _current
			= current
				?? throw new ArgumentNullException(nameof(current));


		/// <summary>
		/// Асинхронно выполняет GET-запрос к указанному URL, десериализует JSON-ответ в объект типа <typeparamref name="T"/> 
		/// и сохраняет результат в распределенном гибридном кэше.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
		/// <param name="jsonOptions">Опциональные параметры настройки десериализации JSON.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
		/// <param name="encoding">Опциональная кодировка текста (по умолчанию UTF-8).</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации заголовков или контента исходящего HTTP-запроса.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Задача, возвращающая объект <see cref="WebApiResult{T}"/> с результатами запроса из кэша или напрямую от API.
		/// </returns>
		/// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
		public async Task<WebApiResult<T>> GetJsonAsync<T>(
			string url,
			JsonSerializerOptions? jsonOptions = null,
			HybridCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(url);
			string cacheKey1 = $"webapi_json_{SuppCrypto.ComputeSha256(url)}";
			var result1 = await _current.Cache.GetAsync(
				cacheKey1,
				async token =>
				{
					var client1 = _current.HttpClientFactory.CreateClient();
					return await client1.GetJsonResultAsync<T>(
						url, _current.HybridCache, jsonOptions, cacheOptions, encoding, configureRequest, token);
				},
				cacheOptions,
				cancellationToken);
			return result1!;
		}


		/// <summary>
		/// (Source Gen) Асинхронно выполняет GET-запрос к указанному URL, десериализует JSON-ответ 
		/// без использования рантайм-рефлексии на основе сгенерированных метаданных и кэширует результат.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
		/// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
		/// <param name="encoding">Опциональная кодировка текста (по умолчанию UTF-8).</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации заголовков или контента исходящего HTTP-запроса.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Задача, возвращающая объект <see cref="WebApiResult{T}"/> с результатами запроса из кэша или напрямую от API.
		/// </returns>
		/// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="jsonTypeInfo"/> равен <see langword="null"/>.</exception>
		public async Task<WebApiResult<T>> GetJsonGenAsync<T>(
			string url,
			JsonTypeInfo<T> jsonTypeInfo,
			HybridCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(url);
			ArgumentNullException.ThrowIfNull(jsonTypeInfo);
			string cacheKey1 = $"webapi_jsongen_{SuppCrypto.ComputeSha256(url)}";
			var result1 = await _current.Cache.GetAsync(
				cacheKey1,
				async token =>
				{
					var client1 = _current.HttpClientFactory.CreateClient();
					return await client1.GetJsonGenResultAsync(
						url, _current.HybridCache, jsonTypeInfo, cacheOptions, encoding, configureRequest, token);
				},
				cacheOptions,
				cancellationToken);
			return result1!;
		}


		/// <summary>
		/// Асинхронно выполняет GET-запрос к указанному URL, десериализует XML-ответ в объект типа <typeparamref name="T"/> 
		/// и сохраняет результат в распределенном гибридном кэше.
		/// </summary>
		/// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
		/// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
		/// <param name="defaultNamespace">Опциональное пространство имен XML, применяемое при разборе схемы.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
		/// <param name="encoding">Опциональная кодировка текста.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации заголовков или контента исходящего HTTP-запроса.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Задача, возвращающая объект <see cref="WebApiResult{T}"/> с результатами запроса из кэша или напрямую от API.
		/// </returns>
		/// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
		public async Task<WebApiResult<T>> GetXmlAsync<T>(
			string url,
			string? defaultNamespace = null,
			HybridCacheEntryOptions? cacheOptions = null,
			Encoding? encoding = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(url);
			string cacheKey1 = $"webapi_xml_{SuppCrypto.ComputeSha256(url)}";
			var result1 = await _current.Cache.GetAsync(
				cacheKey1,
				async token =>
				{
					var client1 = _current.HttpClientFactory.CreateClient();
					return await client1.GetXmlResultAsync<T>(
						url, _current.HybridCache, defaultNamespace, cacheOptions, encoding, configureRequest, token);
				},
				cacheOptions,
				cancellationToken);
			return result1!;
		}


		/// <summary>
		/// Асинхронно выполняет GET-запрос к указанному URL, безопасно парсит построчные текстовые данные 
		/// в формате GRID, проецирует их в материализованную коллекцию объектов типа <typeparamref name="T"/> и кэширует результат.
		/// </summary>
		/// <typeparam name="T">Тип результирующего объекта маппинга строки.</typeparam>
		/// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
		/// <param name="selector">Функция-предикат (лямбда) для пошагового маппинга полей строки в объект типа <typeparamref name="T"/>.</param>
		/// <param name="provider">Опциональный провайдер культуры для парсинга чисел и дат внутри строки.</param>
		/// <param name="encoding">Опциональная кодировка текста текстового файла.</param>
		/// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
		/// <param name="configureRequest">Опциональный делегат для кастомизации объекта исходящего HTTP-запроса.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Задача, возвращающая объект <see cref="WebApiResult{IEnumerable}"/> с результатами запроса из кэша или напрямую от API.
		/// </returns>
		/// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException">Вызывается, если передана пустая ссылка на делегат <paramref name="selector"/>.</exception>
		public async Task<WebApiResult<IEnumerable<T>>> GetGridAsync<T>(
			string url,
			Func<GridRowParser, T> selector,
			IFormatProvider? provider = null,
			Encoding? encoding = null,
			HybridCacheEntryOptions? cacheOptions = null,
			Action<HttpRequestMessage>? configureRequest = null,
			CancellationToken cancellationToken = default)
		{
			ArgumentException.ThrowIfNullOrEmpty(url);
			ArgumentNullException.ThrowIfNull(selector);
			string cacheKey1 = $"webapi_grid_{SuppCrypto.ComputeSha256(url)}";
			var result1 = await _current.Cache.GetAsync(
				cacheKey1,
				async token =>
				{
					var client1 = _current.HttpClientFactory.CreateClient();
					return await client1.GetGridResultAsync(
						url, _current.HybridCache, selector, provider, encoding, cacheOptions, configureRequest, token);
				},
				cacheOptions,
				cancellationToken);
			return result1!;
		}

	}

}
