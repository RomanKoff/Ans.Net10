// rev 2026-09-30

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ans.Net10.Web.Attributes
{

	/// <summary>
	/// Декларативный атрибут для ограничения доступа к действиям контроллера 
	/// на основе заданного строкового пути разрешения.
	/// </summary>
	public class ActionAccessAttribute
		: TypeFilterAttribute
	{

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ActionAccessAttribute"/> с указанием пути доступа.
		/// </summary>
		/// <param name="path">Строковый путь или идентификатор системного разрешения (например, "Admin.Users.Create").</param>
		/// <exception cref="ArgumentException">Вызывается, если параметр <paramref name="path"/> пуст или равен <see langword="null"/>.</exception>
		public ActionAccessAttribute(
			string path)
			: base(typeof(ActionAccessFilter))
		{
			ArgumentException.ThrowIfNullOrEmpty(path);
			Arguments = [path];
		}

	}



	/// <summary>
	/// Асинхронный фильтр авторизации, выполняющий валидацию прав текущего пользователя 
	/// на доступ к конкретному действию MVC без блокировки потоков.
	/// </summary>
	/// <param name="path">Строковый путь проверяемого системного разрешения.</param>
	public class ActionAccessFilter(
		string path)
		: IAsyncAuthorizationFilter
	{

		/// <inheritdoc />
		public async Task OnAuthorizationAsync(
			AuthorizationFilterContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			var http1 = context.HttpContext;
			var user1 = http1.User;
			var isAdmin1 = await http1.IsClaimsAdminAsync();
			if (isAdmin1 || user1.AllowAccessAction(path))
				return;
			context.Result = new ForbidResult();
		}

	}

}
