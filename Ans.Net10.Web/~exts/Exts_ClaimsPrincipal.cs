// rev 2026-09-30

using System.Security.Claims;
using System.Text;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="ClaimsPrincipal"/>, обеспечивающие удобную и высокопроизводительную 
	/// проверку иерархических прав доступа (Actions) в системе авторизации.
	/// </summary>
	public static partial class Exts_ClaimsPrincipal
	{

		/// <summary>
		/// Проверяет наличие у пользователя конкретного утверждения (Claim) действия с указанным значением.
		/// </summary>
		public static bool HasActionClaim(
			this ClaimsPrincipal principal,
			string value)
		{
			ArgumentNullException.ThrowIfNull(principal);
			ArgumentException.ThrowIfNullOrEmpty(value);
			return principal.HasClaim(
				SuppClaims.CLAIMS_ACTIONS_NAME, value);
		}


		/// <summary>
		/// Проверяет наличие у пользователя утверждения (Claim) действия, значение которого удовлетворяет заданному предикату.
		/// </summary>
		public static bool HasActionClaim(
			this ClaimsPrincipal principal,
			Func<string, bool> funcValue)
		{
			ArgumentNullException.ThrowIfNull(principal);
			ArgumentNullException.ThrowIfNull(funcValue);
			return principal.HasClaim(x =>
				string.Equals(x.Type, SuppClaims.CLAIMS_ACTIONS_NAME, StringComparison.Ordinal)
				&& funcValue(x.Value));
		}


		/// <summary>
		/// Выполняет иерархическую проверку доступа текущего пользователя к указанному пути действия (Action Path).
		/// </summary>
		/// <remarks>
		/// Разрешает доступ, если у пользователя есть точное совпадение пути, совпадение по маске родительского каталога 
		/// или права на любой из вышестоящих узлов иерархии (например, доступ к <c>"Admin"</c> открывает доступ к <c>"Admin.Users.Edit"</c>).
		/// </remarks>
		public static bool AllowAccessAction(
			this ClaimsPrincipal principal,
			string path)
		{
			ArgumentNullException.ThrowIfNull(principal);
			ArgumentException.ThrowIfNullOrEmpty(path);
			var subPathMask1 = path + ".";
			if (principal.HasActionClaim(path)
				|| principal.HasActionClaim(x => x.StartsWith(subPathMask1, StringComparison.Ordinal)))
				return true;
			var pathSpan1 = path.AsSpan();
			var sb1 = new StringBuilder(path.Length);
			foreach (var range1 in pathSpan1.Split('.'))
			{
				var segment1 = pathSpan1[range1];
				sb1.Append(segment1);
				if (principal.HasActionClaim(sb1.ToString()))
					return true;
				sb1.Append('.');
			}
			return false;
		}

	}

}
