// rev 2026-10-02

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ans.Net10.Web.Filters
{

	/// <summary>
	/// Глобальный или контроллерный фильтр для автоматической строгой валидации полей моделей
	/// на основе системных констант и регулярных выражений регулярных выражений экосистемы Ans.
	/// </summary>
	public sealed class SuppFormsValidationFilter
		: IAsyncActionFilter
	{

		/// <summary>
		/// Асинхронно перехватывает конвейер выполнения действия контроллера, выполняет
		/// строгую пре-валидацию строковых свойств входящих моделей по регулярным выражениям
		/// и прерывает запрос с возвратом стандартизированной ошибки в случае обнаружения нарушений.
		/// </summary>
		/// <param name="context">Контекст выполнения текущего действия MVC-контроллера.</param>
		/// <param name="next">Делегат, представляющий последующий этап выполнения конвейера запроса.</param>
		/// <returns>Задача, представляющая асинхронную операцию перехвата и валидации.</returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="context"/> или <paramref name="next"/> равен <see langword="null"/>.
		/// </exception>
		/// <remarks>
		/// Метод автоматически сканирует словарь <see cref="ActionExecutingContext.ActionArguments"/>, 
		/// находит пользовательские ссылочные типы (DTO и доменные модели) и выполняет сопоставление 
		/// имен их строковых свойств с каноническими методами проверки из <see cref="Exts_ModelStateValidation"/>.
		/// Если <see cref="ModelStateDictionary.IsValid"/> возвращает <see langword="false"/>, выполнение действия 
		/// блокируется, а клиенту возвращается структура <see cref="ApiResultModel"/> со статусом Bad Request.
		/// </remarks>
		public async Task OnActionExecutionAsync(
			ActionExecutingContext context,
			ActionExecutionDelegate next)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(next);
			foreach (var argument1 in context.ActionArguments.Values)
			{
				if (argument1 == null)
					continue;
				var argumentType1 = argument1.GetType();
				if (argumentType1.IsPrimitive || argumentType1 == typeof(string))
					continue;
				var properties1 = argumentType1.GetProperties();
				foreach (var prop1 in properties1)
				{
					if (prop1.PropertyType == typeof(string))
					{
						string? value1 = prop1.GetValue(argument1) as string;
						if (string.IsNullOrWhiteSpace(value1))
							continue;
						string fieldName1 = prop1.Name;
						switch (fieldName1.ToLowerInvariant())
						{
							case "email":
							case "mail":
								context.ModelState.ValidateEmailStrict(fieldName1, value1);
								break;
							case "varname":
							case "codename":
								context.ModelState.ValidateVarnameStrict(fieldName1, value1);
								break;
							case "name":
							case "identifier":
								context.ModelState.ValidateNameStrict(fieldName1, value1);
								break;
							case "ip":
							case "ipaddress":
								context.ModelState.ValidateIp4(fieldName1, value1);
								break;
						}
					}
				}
			}
			if (!context.ModelState.IsValid)
			{
				if (context.Controller is ControllerBase controller1)
				{
					context.Result = controller1.GetErrorApiResult(
						"Ошибки автоматической валидации данных формы.");
					return;
				}
			}
			await next();
		}

	}

}
