// rev 2026-10-07

using Ans.Net10.Common;
using System.Net;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба сетевого анализа текущего подключения, обеспечивающая валидацию 
	/// удаленного IP-адреса клиента на вхождение в сконфигурированные CIDR-подсети.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="NetworkService"/> на основе первичного конструктора C#.
	/// Позволяет определять принадлежность клиента к административным, безопасным, разрешенным или заблокированным сетевым зонам.
	/// </remarks>
	/// <param name="current">Текущий оркестровый контекст обработки запроса, содержащий настройки подсетей и HTTP-контекст.</param>
	public class NetworkService(
		CurrentContext current)
	{

		/* readonly properties */


		/// <summary>
		/// Получает удаленный IP-адрес клиента, инициировавшего текущий HTTP-запрос.
		/// </summary>
		/// <value>
		/// Объект <see cref="IPAddress"/> или <see langword="null"/>, если контекст подключения недоступен.
		/// </value>
		public IPAddress? RemoteIpAddress
			=> (current ?? throw new ArgumentNullException(nameof(current)))
				.HttpContext?.Connection.RemoteIpAddress;


		/// <summary>
		/// Получает список подсетей, выделенных для администраторов.
		/// </summary>
		public IPSubnetsList? AdminSubnets
			=> current.Options.Subnets?.AdminSubnets;


		/// <summary>
		/// Получает список безопасных (доверенных) подсетей.
		/// </summary>
		public IPSubnetsList? SafeSubnets
			=> current.Options.Subnets?.SafeSubnets;


		/// <summary>
		/// Получает список небезопасных (скомпрометированных) подсетей.
		/// </summary>
		public IPSubnetsList? UnsafeSubnets
			=> current.Options.Subnets?.UnsafeSubnets;


		/// <summary>
		/// Получает белый список разрешенных подсетей.
		/// </summary>
		public IPSubnetsList? AllowSubnets
			=> current.Options.Subnets?.AllowSubnets;


		/// <summary>
		/// Получает черный список принудительно запрещенных подсетей.
		/// </summary>
		public IPSubnetsList? DenySubnets
			=> current.Options.Subnets?.DenySubnets;


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущий IP-адрес клиента к зоне администраторов.
		/// </summary>
		public bool IsAdmin
			=> current.Options.Subnets?.AdminSubnets != null
				&& current.HttpContext?.Connection.RemoteIpAddress != null
				&& current.Options.Subnets.AdminSubnets.Count > 0
				&& current.Options.Subnets.AdminSubnets.Any(
					current.HttpContext.Connection.RemoteIpAddress.IsInSubnet);


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущий IP-адрес клиента к доверенной безопасной зоне.
		/// </summary>
		public bool IsSafe
			=> current.Options.Subnets?.SafeSubnets != null
				&& current.HttpContext?.Connection.RemoteIpAddress != null
				&& current.Options.Subnets.SafeSubnets.Count > 0
				&& current.Options.Subnets.SafeSubnets.Any(
					current.HttpContext.Connection.RemoteIpAddress.IsInSubnet);


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущий IP-адрес клиента к небезопасной зоне.
		/// </summary>
		public bool IsUnsafe
			=> current.Options.Subnets?.UnsafeSubnets != null
				&& current.HttpContext?.Connection.RemoteIpAddress != null
				&& current.Options.Subnets.UnsafeSubnets.Count > 0
				&& current.Options.Subnets.UnsafeSubnets.Any(
					current.HttpContext.Connection.RemoteIpAddress.IsInSubnet);


		/// <summary>
		/// Возвращает признак того, разрешен ли доступ для текущего IP-адреса клиента.
		/// </summary>
		public bool IsAllow
			=> current.Options.Subnets?.AllowSubnets != null
				&& current.HttpContext?.Connection.RemoteIpAddress != null
				&& current.Options.Subnets.AllowSubnets.Count > 0
				&& current.Options.Subnets.AllowSubnets.Any(
					current.HttpContext.Connection.RemoteIpAddress.IsInSubnet);


		/// <summary>
		/// Возвращает признак того, заблокирован ли доступ для текущего IP-адреса клиента.
		/// </summary>
		public bool IsDeny
			=> current.Options.Subnets?.DenySubnets != null
				&& current.HttpContext?.Connection.RemoteIpAddress != null
				&& current.Options.Subnets.DenySubnets.Count > 0
				&& current.Options.Subnets.DenySubnets.Any(
					current.HttpContext.Connection.RemoteIpAddress.IsInSubnet);


		/* functions */


		/// <summary>
		/// Проверяет, входит ли текущий удаленный IP-адрес клиента в указанный список подсетей.
		/// </summary>
		/// <param name="subnets">Проверяемый список IP-подсетей.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если удаленный IP-адрес входит хотя бы в одну подсеть из списка; 
		/// в противном случае, а также если адрес не определен — <see langword="false"/>.
		/// </returns>
		public bool IsRelate(
			IPSubnetsList? subnets)
		{
			if (RemoteIpAddress == null
				|| subnets == null
				|| subnets.Count == 0)
				return false;
			foreach (var subnet1 in subnets)
				if (RemoteIpAddress.IsInSubnet(subnet1))
					return true;
			return false;
		}


		/// <summary>
		/// Проверяет, что текущий удаленный IP-адрес клиента ПОЛНОСТЬЮ ОТСУТСТВУЕТ в указанном списке подсетей.
		/// </summary>
		/// <param name="subnets">Проверяемый список IP-подсетей.</param>
		/// <returns>
		/// Значение <see langword="true"/>, if удаленный IP-адрес не входит ни в одну подсеть из списка; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		public bool IsNotRelate(
			IPSubnetsList? subnets)
		{
			return !IsRelate(subnets);
		}

	}

}
