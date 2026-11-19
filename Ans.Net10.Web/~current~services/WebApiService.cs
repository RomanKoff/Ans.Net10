// rev 2026-11-19

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


        /* functions */


        /// <summary>
        /// Упрощенный асинхронный метод для получения десериализованных JSON-данных напрямую из Web API.
        /// Автоматически использует токен отмены текущего HTTP-запроса и возвращает <see langword="null"/> при любой сетевой ошибке или сбое парсинга.
        /// </summary>
        /// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
        /// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
        /// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
        /// <param name="jsonOptions">Опциональные параметры настройки десериализации JSON.</param>
        /// <param name="cancellationToken">Опциональный токен отмены. Если не задан, используется <see cref="Microsoft.AspNetCore.Http.HttpContext.RequestAborted"/>.</param>
        /// <returns>Десериализованный объект типа <typeparamref name="T"/> при успешном ответе (HTTP 2xx); иначе — <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
        public async Task<T?> GetJsonAsync<T>(
            string url,
            HybridCacheEntryOptions? cacheOptions = null,
            JsonSerializerOptions? jsonOptions = null,
            CancellationToken cancellationToken = default)
        {
            var token1 = cancellationToken == default
                ? _current.HttpContext.RequestAborted
                : cancellationToken;
            var result1 = await FetchJsonWebApiResultAsync<T>(
                url, jsonOptions, cacheOptions, null, null, token1).ConfigureAwait(false);
            return result1.IsSuccess
                ? result1.Content
                : default;
        }


        /// <summary>
        /// (Source Gen) Упрощенный асинхронный метод для получения десериализованных JSON-данных без использования рантайм-рефлексии.
        /// Автоматически использует токен отмены текущего HTTP-запроса и возвращает <see langword="null"/> при любой ошибке.
        /// </summary>
        /// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
        /// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
        /// <param name="jsonTypeInfo">Метаданные типа со сценарием Source Generation, сгенерированные компилятором.</param>
        /// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
        /// <param name="cancellationToken">Опциональный токен отмены. Если не задан, используется <see cref="Microsoft.AspNetCore.Http.HttpContext.RequestAborted"/>.</param>
        /// <returns>Десериализованный объект типа <typeparamref name="T"/> при успешном ответе (HTTP 2xx); иначе — <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="jsonTypeInfo"/> равен <see langword="null"/>.</exception>
        public async Task<T?> GetJsonGenAsync<T>(
            string url,
            JsonTypeInfo<T> jsonTypeInfo,
            HybridCacheEntryOptions? cacheOptions = null,
            CancellationToken cancellationToken = default)
        {
            var token1 = cancellationToken == default
                ? _current.HttpContext.RequestAborted
                : cancellationToken;
            var result1 = await FetchJsonGenWebApiResultAsync(
                url, jsonTypeInfo, cacheOptions, null, null, token1).ConfigureAwait(false);
            return result1.IsSuccess
                ? result1.Content
                : default;
        }


        /// <summary>
        /// Упрощенный асинхронный метод для получения десериализованных XML-данных напрямую из Web API.
        /// Автоматически использует токен отмены текущего HTTP-запроса и возвращает <see langword="null"/> при любой ошибке.
        /// </summary>
        /// <typeparam name="T">Тип модели данных, в которую десериализуется ответ.</typeparam>
        /// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
        /// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
        /// <param name="defaultNamespace">Опциональное пространство имен XML, применяемое при разборе схемы.</param>
        /// <param name="cancellationToken">Опциональный токен отмены. Если не задан, используется <see cref="Microsoft.AspNetCore.Http.HttpContext.RequestAborted"/>.</param>
        /// <returns>Десериализованный объект типа <typeparamref name="T"/> при успешном ответе (HTTP 2xx); иначе — <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
        public async Task<T?> GetXmlAsync<T>(
            string url,
            HybridCacheEntryOptions? cacheOptions = null,
            string? defaultNamespace = null,
            CancellationToken cancellationToken = default)
        {
            var token1 = cancellationToken == default
                ? _current.HttpContext.RequestAborted
                : cancellationToken;
            var result1 = await FetchXmlWebApiResultAsync<T>(
                url, defaultNamespace, cacheOptions, null, null, token1).ConfigureAwait(false);
            return result1.IsSuccess
                ? result1.Content
                : default;
        }


        /// <summary>
        /// Упрощенный асинхронный метод для получения материализованной коллекции данных в формате GRID напрямую из Web API.
        /// Автоматически использует токен отмены текущего HTTP-запроса и возвращает <see langword="null"/> при любой ошибке.
        /// </summary>
        /// <typeparam name="T">Тип результирующего объекта маппинга строки.</typeparam>
        /// <param name="url">Абсолютный или относительный URL-адрес целевого API эндпоинта.</param>
        /// <param name="selector">Функция-предикат (лямбда) для пошагового маппинга полей строки в объект типа <typeparamref name="T"/>.</param>
        /// <param name="cacheOptions">Опциональные параметры времени жизни и политик гибридного кэша.</param>
        /// <param name="cancellationToken">Опциональный токен отмены. Если не задан, используется <see cref="Microsoft.AspNetCore.Http.HttpContext.RequestAborted"/>.</param>
        /// <returns>Коллекция объектов типа <typeparamref name="T"/> при успешном ответе (HTTP 2xx); иначе — <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="url"/> пуст или равен <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">Вызывается, если передана пустая ссылка на делегат <paramref name="selector"/>.</exception>
        public async Task<IEnumerable<T>?> GetGridAsync<T>(
            string url,
            Func<GridRowParser, T> selector,
            HybridCacheEntryOptions? cacheOptions = null,
            CancellationToken cancellationToken = default)
        {
            var token1 = cancellationToken == default
                ? _current.HttpContext.RequestAborted
                : cancellationToken;
            var result1 = await FetchGridWebApiResultAsync(
                url, selector, null, null, cacheOptions, null, token1).ConfigureAwait(false);
            return result1.IsSuccess
                ? result1.Content
                : default;
        }



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
        public Task<WebApiResult<T>> FetchJsonWebApiResultAsync<T>(
            string url,
            JsonSerializerOptions? jsonOptions = null,
            HybridCacheEntryOptions? cacheOptions = null,
            Encoding? encoding = null,
            Action<HttpRequestMessage>? configureRequest = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
            var client1 = _current.HttpClientFactory.CreateClient();
            return client1.FetchJsonResultAsync<T>(
                url, _current.HybridCache, jsonOptions, cacheOptions, encoding, configureRequest, cancellationToken);
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
        public Task<WebApiResult<T>> FetchJsonGenWebApiResultAsync<T>(
            string url,
            JsonTypeInfo<T> jsonTypeInfo,
            HybridCacheEntryOptions? cacheOptions = null,
            Encoding? encoding = null,
            Action<HttpRequestMessage>? configureRequest = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
            ArgumentNullException.ThrowIfNull(jsonTypeInfo);
            var client1 = _current.HttpClientFactory.CreateClient();
            return client1.FetchJsonGenResultAsync(
                url, _current.HybridCache, jsonTypeInfo, cacheOptions, encoding, configureRequest, cancellationToken);
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
        public Task<WebApiResult<T>> FetchXmlWebApiResultAsync<T>(
            string url,
            string? defaultNamespace = null,
            HybridCacheEntryOptions? cacheOptions = null,
            Encoding? encoding = null,
            Action<HttpRequestMessage>? configureRequest = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(url);
            var client1 = _current.HttpClientFactory.CreateClient();
            return client1.FetchXmlResultAsync<T>(
                url, _current.HybridCache, defaultNamespace, cacheOptions, encoding, configureRequest, cancellationToken);
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
        public Task<WebApiResult<IEnumerable<T>>> FetchGridWebApiResultAsync<T>(
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
            var client1 = _current.HttpClientFactory.CreateClient();
            return client1.FetchGridResultAsync(
                url, _current.HybridCache, selector, provider, encoding, cacheOptions, configureRequest, cancellationToken);
        }

    }

}
