// rev 2026-10-20

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
		/// или <see langword="null"/>, если узел не привязан к структуре.
		/// </value>
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


		/// <summary>
		/// Возвращает объект ссылки <see cref="LinkBuilder"/> для текущего профиля узла.
		/// </summary>
		/// <returns>Экземпляр <see cref="LinkBuilder"/> или <see langword="null"/>, если ссылка не может быть построена.</returns>
		public override LinkBuilder? GetLink()
		{
			if (MapItem is null)
				return null;
			var url1 = MapItem.GetUrl(_Current.HttpContext);
			return new LinkBuilder(url1, ShortTitle);
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
