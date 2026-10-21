// rev 2026-10-21

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
		/// <returns>Переопределенный URL-адрес главной страницы или значение по умолчанию.</returns>
		public string? CustomHomeUrl
		{
			get => field ??= _Current.Options.CustomHomeUrl;
			set;
		}


		/* overrides */


		/// <inheritdoc />
		/// <returns>Строка CSS-классов контейнера по умолчанию для разметки страниц сайта.</returns>
		public override string ContainerClasses
		{
			get => field ??= _Current.Options.DefaultContainerClasses;
			set;
		}


		/// <inheritdoc />
		/// <returns>Относительный URL-адрес корня текущего веб-приложения (виртуальный путь).</returns>
		public override string? Url
			=> field ??= _Current.Host.VirtualPath;


		/// <inheritdoc />
		/// <returns>URL-адрес каталога статических ресурсов контента сайта.</returns>
		public override string? ResUrl
		{
			get => field ??= $"{Url}/content";
			set;
		}


		/// <summary>
		/// Возвращает объект ссылки <see cref="LinkBuilder"/> для текущего профиля сайта.
		/// </summary>
		/// <returns>Экземпляр <see cref="LinkBuilder"/> или <see langword="null"/>, если ссылка не может быть построена.</returns>
		public override LinkBuilder? GetLink()
		{
			var url1 = CustomHomeUrl ?? _Current.Host.VirtualPath ?? "/";
			return new LinkBuilder(url1, ShortTitle);
		}

	}

}
