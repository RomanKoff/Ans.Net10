// rev 2026-09-29

using Ans.Net10.Common;
using System.Net;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба сетевого анализа текущего подключения, обеспечивающая валидацию 
	/// удаленного IP-адреса клиента на вхождение в сконфигурированные CIDR-подсети.
	/// </summary>
	/// <remarks>
	/// Позволяет определять принадлежность клиента к административным, 
	/// безопасным, разрешенным или заблокированным сетевым зонам.
	/// </remarks>
	public class NetworkService
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NetworkService"/> на основе текущего контекста запроса.
		/// </summary>
		/// <param name="current">Текущий контекст обработки запроса, содержащий настройки подсетей и HTTP-контекст.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="current"/> равен <see langword="null"/>.
		/// </exception>
		public NetworkService(
			CurrentContext current)
		{
			ArgumentNullException.ThrowIfNull(current);
			RemoteIpAddress = current.HttpContext?.Connection.RemoteIpAddress;
			var subnetsOptions1 = current.Options.Subnets;
			if (subnetsOptions1 == null)
				return;

			AdminSubnets = subnetsOptions1.AdminSubnets;
			SafeSubnets = subnetsOptions1.SafeSubnets;
			UnsafeSubnets = subnetsOptions1.UnsafeSubnets;
			AllowSubnets = subnetsOptions1.AllowSubnets;
			DenySubnets = subnetsOptions1.DenySubnets;

			IsAdmin = AdminSubnets != null && IsRelate(AdminSubnets);
			IsSafe = SafeSubnets != null && IsRelate(SafeSubnets);
			IsUnsafe = UnsafeSubnets != null && IsRelate(UnsafeSubnets);
			IsAllow = AllowSubnets != null && IsRelate(AllowSubnets);
			IsDeny = DenySubnets != null && IsRelate(DenySubnets);
		}


		/* readonly properties */


		/// <summary>
		/// Получает удаленный IP-адрес клиента, инициировавшего текущий HTTP-запрос.
		/// </summary>
		/// <value>
		/// Объект <see cref="IPAddress"/> или <see langword="null"/>, если контекст подключения недоступен.
		/// </value>
		public IPAddress? RemoteIpAddress { get; }


		/// <summary>
		/// Получает список подсетей, выделенных для администраторов.
		/// </summary>
		public IPSubnetsList? AdminSubnets { get; }


		/// <summary>
		/// Получает список безопасных (доверенных) подсетей.
		/// </summary>
		public IPSubnetsList? SafeSubnets { get; }


		/// <summary>
		/// Получает список небезопасных (скомпрометированных) подсетей.
		/// </summary>
		public IPSubnetsList? UnsafeSubnets { get; }


		/// <summary>
		/// Получает белый список разрешенных подсетей.
		/// </summary>
		public IPSubnetsList? AllowSubnets { get; }


		/// <summary>
		/// Получает черный список принудительно запрещенных подсетей.
		/// </summary>
		public IPSubnetsList? DenySubnets { get; }


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущий IP-адрес клиента к зоне администраторов.
		/// </summary>
		public bool IsAdmin { get; }


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущий IP-адрес клиента к доверенной безопасной зоне.
		/// </summary>
		public bool IsSafe { get; }


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущий IP-адрес клиента к небезопасной зоне.
		/// </summary>
		public bool IsUnsafe { get; }


		/// <summary>
		/// Возвращает признак того, разрешен ли доступ для текущего IP-адреса клиента.
		/// </summary>
		public bool IsAllow { get; }


		/// <summary>
		/// Возвращает признак того, заблокирован ли доступ для текущего IP-адреса клиента.
		/// </summary>
		public bool IsDeny { get; }


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
			if (RemoteIpAddress == null || subnets == null || subnets.Count == 0)
				return false;
			foreach (var subnet in subnets)
				if (RemoteIpAddress.IsInSubnet(subnet))
					return true;
			return false;
		}


		/// <summary>
		/// Проверяет, что текущий удаленный IP-адрес клиента ПОЛНОСТЬЮ ОТСУТСТВУЕТ в указанном списке подсетей.
		/// </summary>
		/// <param name="subnets">Проверяемый список IP-подсетей.</param>
		/// <returns>
		/// Значение <see langword="true"/>, если удаленный IP-адрес не входит ни в одну подсеть из списка; 
		/// в противном случае — <see langword="false"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNotRelate(
			IPSubnetsList? subnets)
		{
			return !IsRelate(subnets);
		}

	}

}
