// rev 2026-09-30

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ans.Net10.Web.Attributes
{

	/// <summary>
	/// Асинхронный фильтр авторизации, выполняющий гибкую валидацию структуры утверждений (Claims) 
	/// через внешнюю функцию фильтрации.
	/// </summary>
	/// <param name="type">Тип проверяемого клейма.</param>
	/// <param name="funcValue">Делегат проверки текстового значения клейма.</param>
	public class ClaimRequirementFilter(
		string type,
		Func<string, bool> funcValue)
		: IAsyncAuthorizationFilter
	{

		/// <inheritdoc />
		public Task OnAuthorizationAsync(
			AuthorizationFilterContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			var hasClaim1 = context.HttpContext.User.Claims.Any(x =>
				string.Equals(x.Type, type, StringComparison.Ordinal) && funcValue(x.Value));
			if (!hasClaim1)
				context.Result = new ForbidResult();
			return Task.CompletedTask;
		}

	}



	/// <summary>
	/// Декларативный атрибут авторизации, требующий наличия определенного Claim-партишена 
	/// и соответствия его значения пользовательскому предикату.
	/// </summary>
	public class ClaimRequirementAttribute
		: TypeFilterAttribute
	{

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ClaimRequirementAttribute"/>.
		/// </summary>
		/// <param name="type">Тип проверяемого клейма (Claim Type).</param>
		/// <param name="funcValue">Функция-предикат (лямбда) для валидации текстового значения клейма.</param>
		/// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="type"/> пуст или равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="funcValue"/> равен <see langword="null"/>.</exception>
		public ClaimRequirementAttribute(
			string type,
			Func<string, bool> funcValue)
			: base(typeof(ClaimRequirementFilter))
		{
			ArgumentException.ThrowIfNullOrEmpty(type);
			ArgumentNullException.ThrowIfNull(funcValue);
			Arguments = [type, funcValue];
		}

	}

}
