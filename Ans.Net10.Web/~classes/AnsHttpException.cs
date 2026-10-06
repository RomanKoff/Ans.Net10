using System.Net;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Исключение, генерируемое при возникновении специфических ошибок обработки HTTP-запросов,
	/// содержащее ассоциированный статус-код ответа.
	/// </summary>
	public class AnsHttpException
		: Exception
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AnsHttpException"/> с указанием HTTP статус-кода.
		/// </summary>
		/// <param name="statusCode">Статус-код HTTP-ответа, определяющий тип возникшей ошибки.</param>
		public AnsHttpException(
			HttpStatusCode statusCode)
			: base($"HTTP Error {(int)statusCode} ({statusCode})")
		{
			StatusCode = statusCode;
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AnsHttpException"/> с указанием HTTP статус-кода и детального сообщения об ошибке.
		/// </summary>
		/// <param name="statusCode">Статус-код HTTP-ответа, определяющий тип возникшей ошибки.</param>
		/// <param name="message">Детальное текстовое описание причины возникновения ошибки.</param>
		public AnsHttpException(
			HttpStatusCode statusCode,
			string? message)
			: base(message ?? $"HTTP Error {(int)statusCode} ({statusCode})")
		{
			StatusCode = statusCode;
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="AnsHttpException"/> с указанием HTTP статус-кода, 
		/// детального сообщения и ссылки на внутреннее исключение, ставшее причиной данной ошибки.
		/// </summary>
		/// <param name="statusCode">Статус-код HTTP-ответа, определяющий тип возникшей ошибки.</param>
		/// <param name="message">Детальное текстовое описание причины возникновения ошибки.</param>
		/// <param name="innerException">Внутреннее исключение, вызвавшее сбой в текущем конвейере.</param>
		public AnsHttpException(
			HttpStatusCode statusCode,
			string? message,
			Exception? innerException)
			: base(message ?? $"HTTP Error {(int)statusCode} ({statusCode})", innerException)
		{
			StatusCode = statusCode;
		}


		/* readonly properties */


		/// <summary>
		/// Получает статус-код HTTP-ответа, ассоциированный с данным исключением.
		/// </summary>
		/// <value>
		/// Значение перечисления <see cref="HttpStatusCode"/>.
		/// </value>
		public HttpStatusCode StatusCode { get; }

	}

}
