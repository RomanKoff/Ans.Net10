// rev 2026-09-26

using System.Buffers.Binary;
using System.Net;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для работы с сетевыми IP-адресами <see cref="IPAddress"/>.
	/// </summary>
	public static partial class Exts_IPAddress
	{

		/* functions */


		/// <summary>
		/// Проверяет, входит ли указанный IP-адрес в заданную CIDR-подсеть. Поддерживает адреса семейств IPv4 и IPv6.
		/// </summary>
		/// <param name="address">Проверяемый IP-адрес <see cref="IPAddress"/>.</param>
		/// <param name="subnet">Целевой объект CIDR-подсети <see cref="IPSubnet"/>, на вхождение в которую тестируется адрес.</param>
		/// <returns>
		/// <see langword="true"/>, если проверяемый IP-адрес принадлежит указанной подсети; в противном случае — <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Вызывается при несовпадении семейств сетевых адресов (<see cref="System.Net.Sockets.AddressFamily"/>) у проверяемого адреса и целевой подсети.
		/// </exception>
		public static bool IsInSubnet(
			this IPAddress address,
			IPSubnet subnet)
		{
			if (subnet.MaskLength == 0)
				return true;
			if (address.AddressFamily != subnet.Address.AddressFamily)
				throw new ArgumentException(
					Resources.Exceptions.IPSubnet_LengthAddressAndMaskNotMatch);
			Span<byte> a1 = stackalloc byte[16];
			address.TryWriteBytes(a1, out int _);
			if (!subnet.IsV6)
			{
				uint ipInt1 = BinaryPrimitives.ReadUInt32BigEndian(a1[..4]);
				return (ipInt1 & subnet.MaskV4Template) == subnet.MaskV4Result;
			}
			int fullBytes1 = subnet.MaskLength / 8;
			int remainingBits1 = subnet.MaskLength % 8;
			for (int i1 = 0; i1 < fullBytes1; i1++)
				if (a1[i1] != subnet.MaskV6Bytes[i1])
					return false;
			if (remainingBits1 > 0)
			{
				byte mask1 = (byte)(0xFF << (8 - remainingBits1));
				if ((a1[fullBytes1] & mask1) != subnet.MaskV6Bytes[fullBytes1])
					return false;
			}
			return true;
		}

	}

}
