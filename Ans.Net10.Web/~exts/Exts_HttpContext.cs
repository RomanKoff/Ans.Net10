// rev 2026-09-30

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Encodings.Web;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="HttpContext"/>, обеспечивающие удобную работу с параметрами запроса, 
	/// рендерингом и программной проверкой политик авторизации.
	/// </summary>
	public static partial class Exts_HttpContext
	{

		/// <summary>
		/// Возвращает базовый URL-адрес веб-приложения (протокол, хост и порт, если он нестандартный) 
		/// на основе данных текущего HTTP-запроса.
		/// </summary>
		/// <remarks>
		/// Если приложение работает за прокси-сервером (Nginx/IIS), в приложении должен быть 
		/// включен компонент <c>UseForwardedHeaders</c> для корректного определения внешнего хоста и порта.
		/// </remarks>
		/// <param name="context">Текущий контекст HTTP-запроса.</param>
		/// <returns>Строка вида <c>"https://mysite.com"</c> или <c>"http://localhost:5173"</c>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="context"/> равен <see langword="null"/>.</exception>
		public static string GetBaseUrl(
			this HttpContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			var request1 = context.Request;
			return $"{request1.Scheme}://{request1.Host.Value}";
		}


		/// <summary>
		/// Возвращает базовый виртуальный путь (PathBase) подстраиваемого приложения, если оно развернуто в подкаталоге IIS/Nginx.
		/// </summary>
		/// <param name="context">Текущий контекст HTTP-запроса.</param>
		/// <returns>Строковая строка пути или пустая строка, если приложение запущено в корне домена.</returns>
		public static string GetVirtualPath(
			this HttpContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			return context.Request.PathBase;
		}


		/// <summary>
		/// Возвращает полный абсолютный URL-адрес корня веб-приложения, включая виртуальный путь подкаталога.
		/// </summary>
		/// <param name="context">Текущий контекст HTTP-запроса.</param>
		/// <returns>Строка вида <c>"https://mysite.com"</c>.</returns>
		public static string GetApplicationUrl(
			this HttpContext context)
		{
			return context.GetBaseUrl() + context.GetVirtualPath();
		}


		/// <summary>
		/// Материализует изолированный HTML-контент Razor (лямбда-выражение) в стандартную строку C#.
		/// </summary>
		/// <param name="context">Текущий контекст HTTP-запроса.</param>
		/// <param name="html">Делегат, представляющий Razor-разметку или HTML-компонент.</param>
		/// <returns>Строка, содержащая отрендеренный HTML-код.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если передан пустой делегат разметки.</exception>
		public static string GetStringFromRazor(
			this HttpContext context,
			Func<dynamic, IHtmlContent> html)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(html);
			var sb1 = new StringBuilder();
			using TextWriter writer1 = new StringWriter(sb1);
			var encoder1 = (HtmlEncoder?)context.RequestServices.GetService(typeof(HtmlEncoder))
				?? HtmlEncoder.Default;
			html("").WriteTo(writer1, encoder1);
			return sb1.ToString();
		}


		/// <summary>
		/// Проверяет, обладает ли текущий пользователь правами глобального администратора системы.
		/// </summary>
		public static Task<bool> IsClaimsAdminAsync(
			this HttpContext context)
		{
			return context.TestClaimsPolicyAsync(
				SuppClaims.AUTH_POLICY_ADMINS);
		}


		/// <summary>
		/// Проверяет, обладает ли текущий пользователь правами модератора системы.
		/// </summary>
		public static Task<bool> IsClaimsModeratorAsync(
			this HttpContext context)
		{
			return context.TestClaimsPolicyAsync(
				SuppClaims.AUTH_POLICY_MODERATORS);
		}


		/// <summary>
		/// Проверяет, обладает ли текущий пользователь правами редактора/автора (Writer).
		/// </summary>
		public static Task<bool> IsClaimsWriterAsync(
			this HttpContext context)
		{
			return context.TestClaimsPolicyAsync(
				SuppClaims.AUTH_POLICY_WRITERS);
		}


		/// <summary>
		/// Проверяет, обладает ли текущий пользователь правами читателя (Reader).
		/// </summary>
		public static Task<bool> IsClaimsReaderAsync(
			this HttpContext context)
		{
			return context.TestClaimsPolicyAsync(
				SuppClaims.AUTH_POLICY_READERS);
		}


		/// <summary>
		/// Проверяет, является ли текущий пользователь авторизованным стандартным пользователем системы.
		/// </summary>
		public static Task<bool> IsClaimsUserAsync(
			this HttpContext context)
		{
			return context.TestClaimsPolicyAsync(
				SuppClaims.AUTH_POLICY_USERS);
		}


		/// <summary>
		/// Выполняет асинхронную программную проверку соответствия текущего пользователя указанной политике безопасности.
		/// </summary>
		public static async Task<bool> TestClaimsPolicyAsync(
			this HttpContext context,
			string policy)
		{
			ArgumentException.ThrowIfNullOrEmpty(policy);
			return await context.TestClaimsAsync(policy, null);
		}


		/// <summary>
		/// Выполняет асинхронную программную проверку принадлежности текущего пользователя к указанному списку ролей.
		/// </summary>
		public static async Task<bool> TestClaimsRolesAsync(
			this HttpContext context,
			string roles)
		{
			ArgumentException.ThrowIfNullOrEmpty(roles);
			return await context.TestClaimsAsync(null, roles);
		}


		/// <summary>
		/// Низкоуровневое ядро подсистемы авторизации, выполняющее агрегацию и программное вычисление политик 
		/// и ролей платформы через системные службы оценки политик ASP.NET Core.
		/// </summary>
		public static async Task<bool> TestClaimsAsync(
			this HttpContext context,
			string? policy,
			string? roles)
		{
			ArgumentNullException.ThrowIfNull(context);
			if (context.User.Identity?.IsAuthenticated != true)
				return false;
			var policyProvider1 = context.RequestServices
				.GetRequiredService<IAuthorizationPolicyProvider>();
			var policyEvaluator1 = context.RequestServices
				.GetRequiredService<IPolicyEvaluator>();
			var policy1 = await AuthorizationPolicy.CombineAsync(
				policyProvider1,
				[
					new AuthorizeDataModel
					{
						Policy = policy,
						Roles = roles,
						AuthenticationSchemes = null
					}
				]);
			if (policy1 == null)
				return false;
			var authenticate1 = await policyEvaluator1
				.AuthenticateAsync(policy1, context);
			var authorize1 = await policyEvaluator1
				.AuthorizeAsync(policy1, authenticate1, context, null);
			return authorize1.Succeeded;
		}

	}

}
