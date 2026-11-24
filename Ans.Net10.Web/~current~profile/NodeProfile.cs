// rev 2026-11-24

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
        /// Возвращает элемент навигации <see cref="NavItem"/> для текущего профиля узла
        /// с учетом возможных переопределений заголовков в коде страницы.
        /// </summary>
        /// <returns>Экземпляр <see cref="NavItem"/> или <see langword="null"/>, если элемент не может быть построен.</returns>
        public override NavItem? GetNavItem()
        {
            if (MapItem is null)
                return null;
            string? overrideTitle1 = !Title.Equals(MapItem.Title, StringComparison.Ordinal)
                ? Title : null;
            string? overrideShortTitle1 = !ShortTitle.Equals(MapItem.ShortTitle, StringComparison.Ordinal)
                ? ShortTitle : null;
            return new NavItem(
                MapItem,
                _Current.HttpContext,
                overrideTitle: overrideTitle1,
                overrideShortTitle: overrideShortTitle1);
        }


        /* properties */


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
            Parents = [];
            foreach (var item1 in items)
                AddParent(new NavItem(item1, _Current.HttpContext));
            Parents.Reverse();
        }

    }

}
