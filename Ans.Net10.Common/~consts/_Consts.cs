// rev 2026-09-25

using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Общие константы библиотеки.
	/// </summary>
	public static partial class _Consts
	{

		/// <summary>
		/// Возвращает экземпляр кодировки UTF-8.
		/// </summary>
		public static readonly Encoding ENCODING_UTF8 = Encoding.UTF8;

		/// <summary>
		/// Возвращает экземпляр кодировки Windows-1251 (Кириллица).
		/// </summary>
		public static readonly Encoding ENCODING_WINDOWS1251 = Encoding.GetEncoding(1251);

		/// <summary>
		/// Возвращает экземпляр кодировки KOI8-R (Кириллица).
		/// </summary>
		public static readonly Encoding ENCODING_KOI8R = Encoding.GetEncoding(20866);

		/// <summary>
		/// Возвращает экземпляр кодировки CP866 (DOS-кириллица).
		/// </summary>
		public static readonly Encoding ENCODING_CP866 = Encoding.GetEncoding(866);

		/// <summary>
		/// Возвращает экземпляр кодировки ISO-8859-1 (Западноевропейская).
		/// </summary>
		public static readonly Encoding ENCODING_ISO88591 = Encoding.GetEncoding(28591);

	}

}
