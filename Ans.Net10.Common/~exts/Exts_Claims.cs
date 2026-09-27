// rev 2026-09-26

using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для работы со структурой утверждений  и субъектами безопасности <see cref="ClaimsPrincipal"/>.
	/// </summary>
	public static partial class Exts_Claims
	{

		/* methods */


		/// <summary>
		/// Безопасно добавляет утверждение в удостоверение, если такое утверждение (идентичный тип и значение) еще не существует.
		/// </summary>
		/// <param name="identity">Удостоверение, в которое добавляется утверждение.</param>
		/// <param name="type">Тип добавляемого утверждения.</param>
		/// <param name="value">Значение добавляемого утверждения.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddClaim(
			this ClaimsIdentity identity,
			string type,
			string value)
		{
			if (string.IsNullOrEmpty(type)
				|| string.IsNullOrEmpty(value))
				return;
			if (!identity.HasClaim(x => x.Type == type && x.Value == value))
				identity.AddClaim(new Claim(type, value));
		}


		/// <summary>
		/// Добавляет группу утверждений одного типа с множеством значений в виде новой идентичности субъекта безопасности.
		/// </summary>
		/// <param name="principal">Субъект безопасности <see cref="ClaimsPrincipal"/>, которому добавляются новые утверждения.</param>
		/// <param name="type">Тип добавляемых утверждений.</param>
		/// <param name="values">Массив строковых значений для формируемых утверждений.</param>
		public static void AddClaims(
			this ClaimsPrincipal principal,
			string type,
			params string[] values)
		{
			if (principal == null
				|| string.IsNullOrEmpty(type)
				|| values == null
				|| values.Length == 0)
				return;
			var identity1 = new ClaimsIdentity();
			foreach (var value1 in values)
			{
				if (string.IsNullOrEmpty(value1))
					continue;
				identity1.AddClaim(new Claim(type, value1));
			}
			if (identity1.Claims.Any())
				principal.AddIdentity(identity1);
		}


		/// <summary>
		/// Добавляет субъекту безопасности группу утверждений о ролях с типом <see cref="ClaimTypes.Role"/>.
		/// </summary>
		/// <param name="principal">Субъект безопасности <see cref="ClaimsPrincipal"/>, которому добавляются роли.</param>
		/// <param name="roles">Массив наименований ролей.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddRolesClaims(
			this ClaimsPrincipal principal,
			string[] roles)
		{
			principal.AddClaims(ClaimTypes.Role, roles);
		}


		/* functions */


		/// <summary>
		/// Возвращает значение утверждения уникального идентификатора пользователя (<see cref="ClaimTypes.NameIdentifier"/>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения в виде строки или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetNameIdentifierFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}


		/// <summary>
		/// Возвращает значение кастомного утверждения логина пользователя (<c>"id_username"</c>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetIdUsernameFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst("id_username")?.Value;
		}


		/// <summary>
		/// Возвращает значение утверждения адреса электронной почты (<see cref="ClaimTypes.Email"/>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetEmailFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst(ClaimTypes.Email)?.Value;
		}


		/// <summary>
		/// Возвращает значение кастомного утверждения отображаемого имени пользователя (<c>"name"</c>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetNameFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst("name")?.Value;
		}


		/// <summary>
		/// Возвращает значение утверждения фамилии (<see cref="ClaimTypes.Surname"/>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetSurnameFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst(ClaimTypes.Surname)?.Value;
		}


		/// <summary>
		/// Возвращает значение утверждения личного имени (<see cref="ClaimTypes.GivenName"/>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetGivenNameFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst(ClaimTypes.GivenName)?.Value;
		}


		/// <summary>
		/// Возвращает значение утверждения биологического пола пользователя (<see cref="ClaimTypes.Gender"/>).
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Значение утверждения или <see langword="null"/>, если утверждение не найдено или субъект равен <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string? GetGenderFromClaim(
			this ClaimsPrincipal? principal)
		{
			return principal?.FindFirst(ClaimTypes.Gender)?.Value;
		}


		/// <summary>
		/// Возвращает коллекцию значений всех утверждений о ролях (<see cref="ClaimTypes.Role"/>) текущего субъекта.
		/// </summary>
		/// <param name="principal">Субъект для анализа. Допускает значение <see langword="null"/>.</param>
		/// <returns>Последовательность строковых названий ролей <see cref="IEnumerable{String}"/>. Если субъект не задан или равен <see langword="null"/>, возвращает пустой массив.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> GetRolesFromClaim(
			this ClaimsPrincipal? principal)
		{
			if (principal == null)
				return [];
			return principal.Claims
				.Where(x => x.Type == ClaimTypes.Role)
				.Select(x => x.Value);
		}


		//public static IEnumerable<string> GetClaims(
		//	this ClaimsPrincipal principal,
		//	string typePrefix)
		//{
		//	return principal?.Claims
		//		.Where(x => x.Type.StartsWith(typePrefix))
		//		.Select(x => $"{x.Type} {x.Value}");
		//}

	}

}
