// rev 2026-09-30

using Microsoft.AspNetCore.Authorization;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Реализация интерфейса данных авторизации платформы для динамической сборки политик безопасности.
	/// </summary>
	public class AuthorizeDataModel
		: IAuthorizeData
	{
		/// <inheritdoc />
		public string? Policy { get; set; }

		/// <inheritdoc />
		public string? Roles { get; set; }

		/// <inheritdoc />
		public string? AuthenticationSchemes { get; set; }
	}

}
