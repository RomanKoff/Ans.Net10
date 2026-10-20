// rev 2026-10-17

namespace Ans.Net10.Web
{

	/// <summary>
	/// Описывает профиль текущего сайта.
	/// </summary>
	public sealed class SiteProfile(
		CurrentContext current)
		: _Profile_Proto(current)
	{

		/// <summary>
		/// Получает или задает переопределенный абсолютный или относительный URL-адрес главной страницы сайта.
		/// </summary>
		/// <remarks>
		/// Если кастомный адрес не был задан явно, свойство выполняет ленивую инициализацию 
		/// и возвращает базовое значение по умолчанию из глобальной конфигурации библиотеки <see cref="LibWebOptions.CustomHomeUrl"/>.
		/// </remarks>
		/// <value>
		/// Строка с настраиваемым адресом домашней страницы сайта или <see langword="null"/>, 
		/// если конфигурационный fallback-адрес еще не запрашивался.
		/// </value>
		public string? CustomHomeUrl
		{
			get => field ??= _Current.Options.CustomHomeUrl;
			set;
		}

	}

}
