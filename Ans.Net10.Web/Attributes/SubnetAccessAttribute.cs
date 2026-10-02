// rev 2026-10-02

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Ans.Net10.Web.Attributes
{

	/// <summary>
	/// Декларативный атрибут для ограничения доступа к действиям контроллера
	/// на основе проверки вхождения IP-адреса клиента в заданные подсети.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class SubnetAccessAttribute
		: Attribute,
		IFilterFactory
	{

		private readonly string _subnetZone;


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="SubnetAccessAttribute"/> с указанием сетевой зоны.
		/// </summary>
		/// <param name="subnetZone">Имя зоны из настроек подсетей (например, "Admin", "Safe", "Allow").</param>
		public SubnetAccessAttribute(
			string subnetZone)
		{
			ArgumentException.ThrowIfNullOrEmpty(subnetZone);
			_subnetZone = subnetZone;
		}


		/* readonly properties */


		/// <inheritdoc />
		public bool IsReusable
			=> false;


		/* functions */


		/// <inheritdoc />
		public IFilterMetadata CreateInstance(
			IServiceProvider serviceProvider)
		{
			var current1 = serviceProvider.GetRequiredService<CurrentContext>();
			return new SubnetAccessFilter(current1, _subnetZone);
		}

	}


	/* internals */


	internal sealed class SubnetAccessFilter
		: IAsyncAuthorizationFilter
	{
		private readonly CurrentContext _current;
		private readonly string _zone;

		public SubnetAccessFilter(
			CurrentContext current,
			string zone)
		{
			_current = current;
			_zone = zone;
		}

		public Task OnAuthorizationAsync(
			AuthorizationFilterContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			var networkService1 = new NetworkService(_current);
			bool isAllowed1 = _zone.ToLowerInvariant() switch
			{
				"admin" => networkService1.IsAdmin,
				"safe" => networkService1.IsSafe,
				"allow" => networkService1.IsAllow,
				_ => false,
			};
			if (networkService1.IsDeny)
				isAllowed1 = false;
			if (!isAllowed1)
				throw new AnsHttpException(
					HttpStatusCode.Forbidden, "Access denied by subnet policy.");
			return Task.CompletedTask;
		}

	}

}
