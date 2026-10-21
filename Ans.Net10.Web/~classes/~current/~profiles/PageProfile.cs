// rev 2026-10-21

using Ans.Net10.Common;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Описывает профиль текущей страницы.
	/// </summary>
	public sealed class PageProfile(
		CurrentContext current)
		: _Profile_Proto(current)
	{

		/// <summary>
		/// Получает или задает связанный элемент карты сайта для текущей контентной страницы.
		/// </summary>
		/// <remarks>
		/// При установке элемента автоматически инициализирует свойства <see cref="_Profile_Proto.Title"/> 
		/// и <see cref="_Profile_Proto.ShortTitle"/> значениями из контентной карты.
		/// </remarks>
		/// <value>
		/// Объект <see cref="PagemapItem"/>, описывающий положение страницы в иерархии сайта, 
		/// или <see langword="null"/>, если страница не привязана к структуре.
		/// </value>
		/// <returns>Текущий связанный экземпляр <see cref="PagemapItem"/> либо <see langword="null"/>.</returns>
		public PagemapItem? MapItem
		{
			get;
			set
			{
				field = value;
				if (value is not null)
				{
					Title = value.Title;
					ShortTitle = value.ShortTitle;
					_calcParents(value.GetParentPages());
				}
			}
		}


		/* overrides */


		/// <inheritdoc />
		/// <returns>Строка CSS-классов контейнера, специфичных для текущей страницы или унаследованных от родительского узла.</returns>
		public override string ContainerClasses
		{
			get => field ??= _Current.Node.ContainerClasses;
			set;
		}


		/// <inheritdoc />
		/// <returns>Полный виртуальный URL-адрес текущей страницы либо <see langword="null"/>.</returns>
		public override string? Url
			=> field ??= MapItem?.GetUrl(_Current.HttpContext);


		/// <inheritdoc />
		/// <returns>URL-адрес каталога ресурсов, выделенного для данной контентной страницы.</returns>
		public override string? ResUrl
		{
			get => field ??= $"{_Current.Node.ResUrl}{ResPath.Make("/{0}")}";
			set;
		}


		/// <summary>
		/// Возвращает объект ссылки <see cref="LinkBuilder"/> для текущего профиля страницы.
		/// </summary>
		/// <returns>Экземпляр <see cref="LinkBuilder"/> или <see langword="null"/>, если ссылка не может быть построена.</returns>
		public override LinkBuilder? GetLink()
		{
			if (MapItem is null)
				return null;
			return new LinkBuilder(Url, ShortTitle);
		}


		/* properties */


		/// <summary>
		/// Получает или задает относительный путь к каталогу статических ресурсов текущей страницы.
		/// </summary>
		/// <returns>Строка относительного пути к ресурсам страницы либо <see langword="null"/>.</returns>
		public string? ResPath
		{
			get => field ??= MapItem?.Name;
			set
			{
				field = value;
				ResUrl = null;
			}
		}


		/// <summary>
		/// Получает или задает переопределенный заголовок страницы для отображения в окне браузера.
		/// </summary>
		/// <remarks>
		/// Позволяет принудительно задать текстовое содержимое тега <c>&lt;title&gt;</c> в обход 
		/// автоматического иерархического построения заголовка из названий страницы, узла и сайта.
		/// </remarks>
		/// <value>
		/// Строка с кастомным заголовком вкладки браузера или <see langword="null"/>, 
		/// если должен использоваться автоматически рассчитываемый заголовок.
		/// </value>
		/// <returns>Кастомный заголовок вкладки браузера либо <see langword="null"/>.</returns>
		public string? CustomBrowserTitle { get; set; }


		/* privates */


		private void _calcParents(
			IEnumerable<PagemapItem>? items)
		{
			if (items is null || !items.Any())
				return;
			ParentsLinks = new List<LinkBuilder>();
			foreach (var item1 in items)
			{
				var url1 = item1.GetUrl(_Current.HttpContext);
				var link1 = new LinkBuilder(url1, item1.ShortTitle)
				{
					IsDisabled = item1.HasNoIndex
				};
				AddParent(link1);
			}
			ParentsLinks.Reverse();
		}

	}

}
