// rev 2026-09-25

using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет специализированный список IP-подсетей, расширяющий <see cref="List{IPSubnet}"/> 
	/// и поддерживающий инициализацию из различных строковых форматов CIDR.
	/// </summary>
	public class IPSubnetsList
		: List<IPSubnet>
	{

		/* consts */


		/// <summary>
		/// Коллекция стандартных символов-разделителей, используемых для парсинга перечня подсетей из строки.
		/// </summary>
		/// <value>Массив символов, содержащий точку с запятой (<c>;</c>) и запятую (<c>,</c>).</value>
		public static readonly char[] SEP_NETS = [';', ','];


		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="IPSubnetsList"/> на основе массива строк в формате CIDR.
		/// </summary>
		/// <remarks>
		/// Элементы массива автоматически очищаются от пробелов. Пустые строки или строки, состоящие 
		/// только из пробелов, полностью игнорируются. Если передан <see langword="null"/>, список останется пустым.
		/// </remarks>
		/// <param name="cidrs">Список аргументов (массив) строк в формате CIDR, например: <c>"192.168.1.0/24"</c>, <c>"10.0.0.0/8"</c>.</param>
		public IPSubnetsList(
			params string[] cidrs)
		{
			if (cidrs == null)
				return;
			foreach (var s1 in cidrs)
			{
				if (string.IsNullOrWhiteSpace(s1))
					continue;
				Add(new IPSubnet(s1.Trim()));
			}
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="IPSubnetsList"/>, разбирая единую текстовую строку с разделителями.
		/// </summary>
		/// <remarks>
		/// В качестве разделителей подсетей поддерживаются символы из поля <see cref="SEP_NETS"/>. 
		/// Пустые подстроки автоматически исключаются из разбора.
		/// </remarks>
		/// <param name="cidrs">Единая строка с перечислением CIDR-подсетей. Если строка пуста или равна <see langword="null"/>, инициализируется пустой список.</param>
		public IPSubnetsList(
			string cidrs)
			: this(string.IsNullOrWhiteSpace(cidrs)
				  ? [] : cidrs.Split(SEP_NETS, StringSplitOptions.RemoveEmptyEntries))
		{
		}


		/* functions */


		/// <summary>
		/// Возвращает строковое представление текущего списка подсетей, где элементы объединены точкой с запятой.
		/// </summary>
		/// <returns>Строка, содержащая все подсети в формате CIDR, разделенные символом <c>;</c>, например: <c>"192.168.1.0/24;10.0.0.0/8"</c>.</returns>
		public override string ToString()
		{
			return string.Join(";", this.Select(x => x.ToString()));
		}

	}



	/// <summary>
	/// Представляет подсеть IP-адресов (поддерживает протоколы IPv4 и IPv6) и инкапсулирует 
	/// низкоуровневую математическую логику битовых масок для проверки вхождения адресов.
	/// </summary>
	public class IPSubnet
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="IPSubnet"/> на основе базового IP-адреса и длины префикса маски.
		/// </summary>
		/// <param name="address">Базовый IP-адрес подсети в формате объекта <see cref="IPAddress"/>.</param>
		/// <param name="maskLength">Длина префикса сетевой маски в битах (от <c>0</c> до <c>32</c> для IPv4, и от <c>0</c> до <c>128</c> для IPv6).</param>
		/// <exception cref="NotSupportedException">
		/// Вызывается в следующих случаях: длина маски меньше <c>0</c>; семейство адресов не относится к IPv4/IPv6; 
		/// либо длина маски превышает допустимый предел для конкретного протокола (32 бита для IPv4 / 128 бит для IPv6).
		/// </exception>
		public IPSubnet(
			IPAddress address,
			int maskLength)
		{
			_init(address, maskLength);
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="IPSubnet"/>, десериализуя его из единой CIDR-строки.
		/// </summary>
		/// <param name="cidr">Строка формата <c>"IP/Маска"</c>, например: <c>"192.168.1.0/24"</c> или <c>"fe80::/10"</c>.</param>
		/// <exception cref="NotSupportedException">Вызывается, если строка не содержит обязательный символ разделителя маски <c>'/'</c>.</exception>
		/// <exception cref="ArgumentNullException">Вызывается, если переданная строка равна <see langword="null"/>.</exception>
		/// <exception cref="FormatException">Вызывается, если строка адреса или число маски имеют некорректный формат.</exception>
		public IPSubnet(
			string cidr)
		{
			var i1 = cidr.IndexOf('/');
			if (i1 == -1)
				throw new NotSupportedException(
					Resources.Exceptions.IPSubnet_Length);
			_init(
				IPAddress.Parse(cidr[..i1]),
				int.Parse(cidr[(i1 + 1)..]));
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает базовый IP-адрес подсети.
		/// </summary>
		/// <value>Объект класса <see cref="IPAddress"/>.</value>
		public IPAddress Address { get; private set; } = null!;


		/// <summary>
		/// Возвращает признак того, принадлежит ли текущая подсеть протоколу IPv6.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если семейство адресов подсети — IPv6; если это IPv4 — <see langword="false"/>.</value>
		public bool IsV6 { get; private set; }


		/// <summary>
		/// Возвращает длину префикса маски подсети в битах.
		/// </summary>
		/// <value>Целочисленное значение количества бит префикса.</value>
		public int MaskLength { get; private set; }


		/// <summary>
		/// Возвращает битовую маску-шаблон для сетей IPv4 в виде 32-битного числа без знака.
		/// </summary>
		/// <value>Число <see cref="uint"/>, где биты сети установлены в 1, а хостовые биты — в 0.</value>
		public uint MaskV4Template { get; private set; }


		/// <summary>
		/// Возвращает результат применения битовой маски-шаблона к базовому адресу IPv4.
		/// </summary>
		/// <value>Число <see cref="uint"/>, представляющее чистый адрес сети IPv4 без хостовой части.</value>
		public uint MaskV4Result { get; private set; }


		/// <summary>
		/// Возвращает массив байт отфильтрованной сетевой маски для подсетей протокола IPv6.
		/// </summary>
		/// <value>Массив из 16 байт, содержащий маскированные данные адреса сети IPv6.</value>
		public byte[] MaskV6Bytes { get; private set; } = null!;


		/* functions */


		/// <summary>
		/// Возвращает каноническое строковое представление подсети в формате CIDR.
		/// </summary>
		/// <returns>Строка формата <c>"БазовыйАдрес/ДлинаМаски"</c>.</returns>
		public override string ToString()
		{
			return $"{Address}/{MaskLength}";
		}


		/* privates */


		private void _init(
			IPAddress address,
			int maskLength)
		{
			if (maskLength < 0)
				throw new NotSupportedException(
					Resources.Exceptions.IPSubnet_Less0);
			Address = address;
			MaskLength = maskLength;
			Span<byte> a1 = stackalloc byte[16];
			if (!Address.TryWriteBytes(a1, out int _))
				throw new NotSupportedException(Resources.Exceptions.IPSubnet_OnlyIPv4orIPv6);
			switch (Address.AddressFamily)
			{
				case AddressFamily.InterNetwork:
					IsV6 = false;
					if (MaskLength > 32)
						throw new NotSupportedException(
							Resources.Exceptions.IPSubnet_LengthAddressAndMaskNotMatch);
					uint ipV4Int1 = BinaryPrimitives.ReadUInt32BigEndian(a1[..4]);
					MaskV4Template = MaskLength == 0
						? 0 : uint.MaxValue << (32 - MaskLength);
					MaskV4Result = ipV4Int1 & MaskV4Template;
					MaskV6Bytes = [];
					break;
				case AddressFamily.InterNetworkV6:
					IsV6 = true;
					if (MaskLength > 128)
						throw new NotSupportedException(
							Resources.Exceptions.IPSubnet_LengthAddressAndMaskNotMatch);
					MaskV6Bytes = new byte[16];
					int fullBytes1 = MaskLength / 8;
					int remainingBits1 = MaskLength % 8;
					for (int i1 = 0; i1 < fullBytes1; i1++)
						MaskV6Bytes[i1] = a1[i1];
					if (remainingBits1 > 0)
					{
						byte mask1 = (byte)(0xFF << (8 - remainingBits1));
						MaskV6Bytes[fullBytes1] = (byte)(a1[fullBytes1] & mask1);
					}
					break;
				default:
					throw new NotSupportedException(
						Resources.Exceptions.IPSubnet_OnlyIPv4orIPv6);
			}
		}

	}

}
