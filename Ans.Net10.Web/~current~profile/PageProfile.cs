// rev 2026-11-24

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
        /// Возвращает элемент навигации <see cref="NavItem"/> для текущего профиля страницы
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
                isActive: true,
                overrideTitle: overrideTitle1,
                overrideShortTitle: overrideShortTitle1);
        }


        /* properties */


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
            Parents = [];
            foreach (var item1 in items)
                AddParent(new NavItem(item1, _Current.HttpContext, isActiveBranch: true));
            Parents.Reverse();
        }

    }

}
