// rev 2026-10-21

using Ans.Net10.Common;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Описывает профиль текущего узла.
	/// </summary>
	public sealed class NodeProfile(
		CurrentContext current)
		: _Profile_Proto(current)
	{

		/// <summary>
		/// Получает или задает связанный элемент карты сайта для текущего информационного узла (Node).
		/// </summary>
		/// <remarks>
		/// При установке элемента автоматически инициализирует свойства <see cref="_Profile_Proto.Title"/> 
		/// и <see cref="_Profile_Proto.ShortTitle"/> значениями из метаданных карты сайта.
		/// </remarks>
		/// <value>
		/// Объект <see cref="NodemapItem"/>, содержащий навигационную структуру узла, 
		/// или <see langword="null"/>, если узел не привязан к структуру.
		/// </value>
		/// <returns>Текущий привязанный объект <see cref="NodemapItem"/> либо <see langword="null"/>.</returns>
		public NodemapItem? MapItem
		{
			get;
			set
			{
				field = value;
				if (value is not null)
				{
					Title = value.Title;
					ShortTitle = value.ShortTitle;
					_calcParents(value.GetParentNodes()?
						.Where(x => x.Type is not NodemapTypesEnum.Group));
				}
			}
		}


		/* overrides */


		/// <inheritdoc />
		/// <returns>Строка CSS-классов контейнера, специфичных для текущего узла или унаследованных от сайта.</returns>
		public override string ContainerClasses
		{
			get => field ??= _Current.Site.ContainerClasses;
			set;
		}


		/// <inheritdoc />
		/// <returns>Полный виртуальный URL-адрес текущего узла либо <see langword="null"/>.</returns>
		public override string? Url
			=> field ??= MapItem?.GetUrl(_Current.HttpContext);


		/// <inheritdoc />
		/// <returns>URL-адрес каталога ресурсов, выделенного для данного информационного узла.</returns>
		public override string? ResUrl
		{
			get => field ??= $"{_Current.Site.ResUrl}{ResPath.Make("/{0}")}";
			set;
		}


		/// <summary>
		/// Возвращает объект ссылки <see cref="LinkBuilder"/> для текущего профиля узла.
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
		/// Получает или задает относительный путь к каталогу ресурсов текущего узла.
		/// </summary>
		/// <returns>Строка пути к ресурсам узла либо <see langword="null"/>.</returns>
		public string? ResPath
		{
			get => field ??= MapItem?.Name;
			set
			{
				field = value;
				ResUrl = null;
			}
		}


		/* privates */


		private void _calcParents(
			IEnumerable<NodemapItem>? items)
		{
			if (items is null || !items.Any())
				return;
			ParentsLinks = new List<LinkBuilder>();
			foreach (var item1 in items)
			{
				var url1 = item1.GetUrl(_Current.HttpContext);
				var link1 = new LinkBuilder(url1, item1.ShortTitle);
				AddParent(link1);
			}
			ParentsLinks.Reverse();
		}

	}

}
