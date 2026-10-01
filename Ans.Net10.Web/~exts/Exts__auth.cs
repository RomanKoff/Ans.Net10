// rev 2026-010-01

using Microsoft.AspNetCore.Authorization;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для настройки подсистемы авторизации <see cref="AuthorizationOptions"/> платформы.
	/// </summary>
	public static partial class Exts__auth
	{

		/// <summary>
		/// Декларативно регистрирует стандартный иерархический набор политик безопасности приложения 
		/// на основе предопределенных системных утверждений (Claims).
		/// </summary>
		/// <remarks>
		/// Метод формирует следующие политики авторизации:
		/// <list type="bullet">
		/// <item><description><c>Admins</c> — требует уровень доступа Администратора.</description></item>
		/// <item><description><c>Moderators</c> — разрешает доступ Администраторам и Модераторам.</description></item>
		/// <item><description><c>Writers</c> — разрешает доступ Администраторам, Модераторам и Редакторам.</description></item>
		/// <item><description><c>Readers</c> — разрешает доступ любым сотрудникам с правами на чтение и выше.</description></item>
		/// <item><description><c>Users</c> — требует наличия любого валидного утверждения типа политики авторизации.</description></item>
		/// </list>
		/// </remarks>
		/// <param name="options">Объект конфигурации параметров авторизации ASP.NET Core.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="options"/> равен <see langword="null"/>.</exception>
		public static void AddAnsPolicies(
			this AuthorizationOptions options)
		{
			ArgumentNullException.ThrowIfNull(options);
			options.AddPolicy(
				SuppClaims.AUTH_POLICY_ADMINS,
				x => x.RequireClaim(
					SuppClaims.CLAIMS_AUTH_POLICY_NAME,
					SuppClaims.AUTH_POLICY_ADMINS_VALUE));
			options.AddPolicy(
				SuppClaims.AUTH_POLICY_MODERATORS,
				x => x.RequireClaim(
					SuppClaims.CLAIMS_AUTH_POLICY_NAME,
					SuppClaims.AUTH_POLICY_ADMINS_VALUE,
					SuppClaims.AUTH_POLICY_MODERATORS_VALUE));
			options.AddPolicy(
				SuppClaims.AUTH_POLICY_WRITERS,
				x => x.RequireClaim(
					SuppClaims.CLAIMS_AUTH_POLICY_NAME,
					SuppClaims.AUTH_POLICY_ADMINS_VALUE,
					SuppClaims.AUTH_POLICY_MODERATORS_VALUE,
					SuppClaims.AUTH_POLICY_WRITERS_VALUE));
			options.AddPolicy(
				SuppClaims.AUTH_POLICY_READERS,
				x => x.RequireClaim(
					SuppClaims.CLAIMS_AUTH_POLICY_NAME,
					SuppClaims.AUTH_POLICY_ADMINS_VALUE,
					SuppClaims.AUTH_POLICY_MODERATORS_VALUE,
					SuppClaims.AUTH_POLICY_WRITERS_VALUE,
					SuppClaims.AUTH_POLICY_READERS_VALUE));
			options.AddPolicy(
				SuppClaims.AUTH_POLICY_USERS,
				x => x.RequireClaim(
					SuppClaims.CLAIMS_AUTH_POLICY_NAME));
		}

	}

}
