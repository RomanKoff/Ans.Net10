// rev 2026-09-26

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для управления региональными настройками (культурой) приложения.
	/// </summary>
	public static class SuppCulture
	{

		/* methods */


		/// <summary>
		/// Добавляет регистрацию расширенных кодовых страниц (включая Windows-1251 и CP866) в инфраструктуру .NET 10.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddCodePagesSupport()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}


		/// <summary>
		/// Устанавливает заданную культуру для текущего потока и его асинхронного контекста.
		/// </summary>
		/// <param name="culture">Объект информации о культуре <see cref="CultureInfo"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetCulture(
			CultureInfo culture)
		{
			CultureInfo.CurrentUICulture = culture;
			CultureInfo.CurrentCulture = culture;
		}


		/// <summary>
		/// Устанавливает культуру по её строковому имени (например, <c>"ru-RU"</c> или <c>"en-US"</c>).
		/// </summary>
		/// <param name="culture">Строковое имя (идентификатор) целевой культуры.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetCulture(
			string culture)
		{
			SetCulture(new CultureInfo(culture));
		}

	}

}
