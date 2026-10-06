using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web.Services
{

	/// <summary>
	/// Компонент конвейера обработки ошибок (Exception Handler) экосистемы Ans, реализующий 
	/// стандартный интерфейс .NET 10 <see cref="IExceptionHandler"/> для перехвата специализированных 
	/// исключений <see cref="AnsHttpException"/> и автоматической установки статус-кодов ответов.
	/// </summary>
	public class AnsExceptionHandler
		: IExceptionHandler
	{

		/* functions */


		/// <summary>
		/// Асинхронно перехватывает возникшее в конвейере исключение, проверяет его тип 
		/// и, в случае совпадения с <see cref="AnsHttpException"/>, фиксирует ассоциированный HTTP статус-код.
		/// </summary>
		/// <param name="httpContext">Контекст текущего обрабатываемого HTTP-запроса <see cref="HttpContext"/>.</param>
		/// <param name="exception">Экземпляр возникшего необработанного исключения <see cref="Exception"/>.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции <see cref="CancellationToken"/>.</param>
		/// <returns>
		/// Задача, результатом которой является <see langword="true"/>, если исключение успешно обработано 
		/// данным компонентом и конвейер должен продолжить работу; в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если переданный параметр <paramref name="httpContext"/> или <paramref name="exception"/> равен <see langword="null"/>.
		/// </exception>
		public ValueTask<bool> TryHandleAsync(
			HttpContext httpContext,
			Exception exception,
			CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(httpContext);
			ArgumentNullException.ThrowIfNull(exception);

			// Если исключение не является нашим кастомным AnsHttpException, пропускаем его дальше
			if (exception is not AnsHttpException ansException1)
				return ValueTask.FromResult(false);

			// Извлекаем заложенный разработчиком статус-код (например, Forbidden, NotFound)
			var targetStatusCode1 = (int)ansException1.StatusCode;

			// Фиксируем правильный HTTP-статус в ответе сервера
			httpContext.Response.StatusCode = targetStatusCode1;

			// Возвращаем true, сигнализируя платформе .NET 10, что исключение успешно перехвачено.
			// Теперь стандартный app.UseStatusCodePagesWithReExecute автоматически отобразит пользователю 
			// кастомную страницу из настроек options1.Errors.HttpErrorPath с нужным кодом ошибки.
			return ValueTask.FromResult(true);
		}

	}

}
