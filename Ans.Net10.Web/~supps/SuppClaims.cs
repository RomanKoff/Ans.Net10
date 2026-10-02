// rev 2026-09-30

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс, содержащий стандартизированные имена типов утверждений (Claims) 
	/// и строковые константы для настройки глобальных политик авторизации в приложении.
	/// </summary>
	public static class SuppClaims
	{

		/* consts */


		/// <summary>
		/// Системное имя типа утверждения (Claim Type) для определения базовой политики авторизации пользователя.
		/// </summary>
		public const string CLAIMS_AUTH_POLICY_NAME
			= "ANS_AUTHPOL";


		/// <summary>
		/// Системное имя типа утверждения (Claim Type) для хранения списка разрешенных пользователю действий (Actions).
		/// </summary>
		public const string CLAIMS_ACTIONS_NAME
			= "ANS_ACTIONS";


		/// <summary>
		/// Системное имя типа утверждения (Claim Type) для хранения списка доступных пользователю ресурсов (Resources).
		/// </summary>
		public const string CLAIMS_RESOURCES_NAME
			= "ANS_RESOURCES";


		/// <summary>
		/// Имя политики авторизации для группы "Администраторы".
		/// </summary>
		public const string AUTH_POLICY_ADMINS
			= $"{CLAIMS_AUTH_POLICY_NAME}_ADMINS";

		/// <summary>
		/// Маркер уровня доступа для политики администраторов. Соответствует значению <c>"4"</c>.
		/// </summary>
		public const string AUTH_POLICY_ADMINS_VALUE = "4";


		/// <summary>
		/// Имя политики авторизации для группы "Модераторы".
		/// </summary>
		public const string AUTH_POLICY_MODERATORS
			= $"{CLAIMS_AUTH_POLICY_NAME}_MODERATORS";

		/// <summary>
		/// Маркер уровня доступа для политики модераторов. Соответствует значению <c>"3"</c>.
		/// </summary>
		public const string AUTH_POLICY_MODERATORS_VALUE = "3";


		/// <summary>
		/// Имя политики авторизации для группы "Редакторы/Авторы" (Writers).
		/// </summary>
		public const string AUTH_POLICY_WRITERS
			= $"{CLAIMS_AUTH_POLICY_NAME}_WRITERS";

		/// <summary>
		/// Маркер уровня доступа для политики редакторов. Соответствует значению <c>"2"</c>.
		/// </summary>
		public const string AUTH_POLICY_WRITERS_VALUE = "2";


		/// <summary>
		/// Имя политики авторизации для группы "Читатели" (Readers).
		/// </summary>
		public const string AUTH_POLICY_READERS
			= $"{CLAIMS_AUTH_POLICY_NAME}_READERS";

		/// <summary>
		/// Маркер уровня доступа для политики читателей. Соответствует значению <c>"1"</c>.
		/// </summary>
		public const string AUTH_POLICY_READERS_VALUE = "1";


		/// <summary>
		/// Имя политики авторизации для стандартных зарегистрированных пользователей.
		/// </summary>
		public const string AUTH_POLICY_USERS
			= $"{CLAIMS_AUTH_POLICY_NAME}_USERS";

		/// <summary>
		/// Маркер уровня доступа для стандартных пользователей. Соответствует значению <c>"0"</c>.
		/// </summary>
		public const string AUTH_POLICY_USERS_VALUE = "0";

	}

}
