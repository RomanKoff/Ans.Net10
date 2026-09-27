// rev 2026-09-25

using System.Net;
using System.Net.Http.Headers;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Базовый класс для инкапсуляции результатов веб-запросов, ответов сервера и метаданных ошибок.
	/// </summary>
	/// <typeparam name="T">Тип ожидаемых десериализованных полезных данных (контента), содержащихся в ответе.</typeparam>
	public class _WebResult_Base<T>
	{

		/// <summary>
		/// Возвращает или задает десериализованные полезные данные (контент), полученные из ответа сервера.
		/// </summary>
		/// <value>
		/// Объект типа <typeparamref name="T"/>, содержащий результат успешного выполнения запроса, 
		/// или <see langword="null"/>, если запрос завершился ошибкой или данные не удалось десериализовать.
		/// </value>
		public T? Content { get; set; }


		/// <summary>
		/// Возвращает или задает HTTP-код статуса ответа от сервера.
		/// </summary>
		/// <value>
		/// Одно из значений перечисления <see cref="HttpStatusCode"/>, возвращенное удаленным сервером.
		/// </value>
		public HttpStatusCode StatusCode { get; set; }


		/// <summary>
		/// Возвращает или задает коллекцию заголовков HTTP-ответа, полученную от сервера.
		/// </summary>
		/// <value>
		/// Объект <see cref="HttpResponseHeaders"/>, содержащий заголовки ответа, или <see langword="null"/>, если заголовки отсутствуют.
		/// </value>
		public HttpResponseHeaders? Headers { get; set; }


		/// <summary>
		/// Возвращает или задает признак возникновения ошибки при десериализации полученных данных.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если при попытке преобразовать тело ответа в тип <typeparamref name="T"/> возникло исключение; 
		/// в противном случае — <see langword="false"/>.
		/// </value>
		public bool IsDeserializationError { get; set; }


		/// <summary>
		/// Возвращает или задает исключение, возникшее в процессе десериализации данных.
		/// </summary>
		/// <value>
		/// Объект <see cref="Exception"/>, описывающий ошибку парсинга, или <see langword="null"/>, 
		/// если десериализация прошла успешно или ошибка произошла на сетевом уровне.
		/// </value>
		public Exception? DeserializationException { get; set; }


		/// <summary>
		/// Возвращает или задает строковое содержимое (тело) ошибки, возвращенное сервером.
		/// </summary>
		/// <value>
		/// Необработанная строка ответа сервера (например, HTML-страница ошибки или JSON-описание проблемы), 
		/// возвращенная при неуспешном HTTP-статусе, или <see langword="null"/>.
		/// </value>
		public string? ErrorBody { get; set; }

	}

}
