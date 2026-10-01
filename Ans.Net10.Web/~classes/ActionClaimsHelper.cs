// rev 2026-10-01

using System.Security.Claims;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для иерархической проверки утверждений (Claims) текущего пользователя 
	/// на соответствие уровням доступа: Каталог, Контроллер и Конкретное Действие (Action).
	/// </summary>
	/// <remarks>
	/// Упрощает декларативную проверку прав в формате <c>Каталог.Контроллер.Действие</c> на основе утверждений действий.
	/// </remarks>
	public class ActionClaimsHelper
	{

		private readonly ClaimsPrincipal _principal;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ActionClaimsHelper"/> для указанного пользователя.
		/// </summary>
		/// <param name="principal">Объект текущего авторизованного пользователя <see cref="ClaimsPrincipal"/>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="principal"/> равен <see langword="null"/>.</exception>
		public ActionClaimsHelper(
			ClaimsPrincipal principal)
		{
			ArgumentNullException.ThrowIfNull(principal);
			_principal = principal;
		}


		/* functions */


		/// <summary>
		/// Проверяет, разрешен ли доступ к указанному разделу (каталогу) в целом.
		/// </summary>
		/// <param name="catalog">Имя целевого каталога или программного модуля.</param>
		/// <returns>Значение <see langword="true"/>, если доступ к каталогу разрешен; в противном случае — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="catalog"/> равен <see langword="null"/>.</exception>
		public bool AllowCatalog(
			string catalog)
		{
			ArgumentNullException.ThrowIfNull(catalog);
			return _principal.HasActionClaim(catalog);
		}


		/// <summary>
		/// Проверяет, разрешен ли доступ к конкретному контроллеру внутри указанного каталога.
		/// </summary>
		/// <param name="catalog">Имя целевого каталога или программного модуля.</param>
		/// <param name="controller">Имя целевого контроллера.</param>
		/// <returns>Значение <see langword="true"/>, если доступ к контроллеру в рамках каталога разрешен; в противном случае — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="catalog"/> или <paramref name="controller"/> равен <see langword="null"/>.</exception>
		public bool AllowController(
			string catalog,
			string controller)
		{
			ArgumentNullException.ThrowIfNull(catalog);
			ArgumentNullException.ThrowIfNull(controller);
			var s1 = $"{catalog}.{controller}";
			return _principal.HasActionClaim(s1);
		}


		/// <summary>
		/// Проверяет, разрешен ли доступ к конкретному действию (Action) указанного контроллера и каталога.
		/// </summary>
		/// <param name="catalog">Имя целевого каталога или программного модуля.</param>
		/// <param name="controller">Имя целевого контроллера.</param>
		/// <param name="action">Имя целевого действия (метода).</param>
		/// <returns>Значение <see langword="true"/>, если доступ к конкретному действию разрешен; в противном случае — <see langword="false"/>.</returns>
		/// <exception cref="ArgumentNullException">Вызывается, если любой из параметров равен <see langword="null"/>.</exception>
		public bool AllowAction(
			string catalog,
			string controller,
			string action)
		{
			ArgumentNullException.ThrowIfNull(catalog);
			ArgumentNullException.ThrowIfNull(controller);
			ArgumentNullException.ThrowIfNull(action);
			var s1 = $"{catalog}.{controller}.{action}";
			return _principal.HasActionClaim(s1);
		}

	}

}
