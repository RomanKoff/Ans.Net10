using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web.Middlewares
{

	/*
	Регистрация в контейнере зависимостей (DI):
		LibWebStartup.Use_AnsNet10Web()
			app.UseMiddleware<AnsHttpExceptionHandler>();
	*/



	/// <summary>
	/// Компонент конвейера (Middleware), обеспечивающий перехват специализированных исключений 
	/// <see cref="AnsHttpException"/> и автоматическую установку соответствующих HTTP-статусов ответа.
	/// </summary>
	/// <remarks>
	/// Регистрируется в методе запуска приложения:
	/// <c>app.UseMiddleware&lt;AnsHttpExceptionHandler&gt;();</c>
	/// </remarks>
	public class AnsHttpExceptionHandler(
		RequestDelegate pipeline)
	{

		/// <summary>
		/// Точка входа Middleware, перехватывающая управление обработкой текущего HTTP-запроса.
		/// </summary>
		/// <param name="context">Текущий контекст HTTP-запроса.</param>
		/// <returns>Задача асинхронного выполнения операции.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="context"/> равен <see langword="null"/>.</exception>
		public async Task InvokeAsync(
			HttpContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			try
			{
				await pipeline(context);
			}
			catch (AnsHttpException exception)
			{
				if (!context.Response.HasStarted)
				{
					context.Response.StatusCode = (int)exception.StatusCode;

					// Удаляем заголовки, описывающие старое (успешное) содержимое, 
					// но сохраняем системные заголовки инфраструктуры и CORS
					context.Response.Headers.Remove("Content-Type");
					context.Response.Headers.Remove("Content-Length");
					context.Response.Headers.Remove("ETag");
				}
				else
				{
					// Если поток ответа уже пошел, мы не можем изменить заголовки, 
					// поэтому просто перевыбрасываем исключение наружу для системного логирования
					throw;
				}
			}
		}

	}

}
