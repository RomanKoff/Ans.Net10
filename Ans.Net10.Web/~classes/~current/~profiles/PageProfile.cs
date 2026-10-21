// rev 2026-10-20

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


		public override string ContainerClasses
		{
			get => field ?? _Current.Node.ContainerClasses;
			set;
		}


		public override string? Url
			=> field ??= MapItem?.GetUrl(_Current.HttpContext);


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
