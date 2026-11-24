// rev 2026-11-24

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Представляет спроецированный на текущий HTTP-запрос элемент навигации
    /// (для построения меню, деревьев разделов и цепочек «хлебных крошек»).
    /// </summary>
    public sealed class NavItem
    {

        private readonly string[]? _displayOnTags;
        private readonly string[]? _displayOffTags;
        private readonly string[]? _passthroughsOnTags;
        private readonly string[]? _passthroughsOffTags;


        /* ctors */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NavItem"/> на основе узла или страницы карты сайта
        /// с возможностью переопределения заголовков из профиля запроса.
        /// </summary>
        /// <param name="source">Исходный элемент карты сайта (<see cref="NodemapItem"/> или <see cref="PagemapItem"/>).</param>
        /// <param name="httpContext">Контекст текущего HTTP-запроса для вычисления URL.</param>
        /// <param name="isActive">Признак точного совпадения элемента с текущим запросом.</param>
        /// <param name="isActiveBranch">Признак нахождения элемента в активной ветке предков текущего запроса.</param>
        /// <param name="children">Коллекция отфильтрованных дочерних элементов навигации.</param>
        /// <param name="overrideTitle">Переопределенный полный заголовок (если изменен в профиле).</param>
        /// <param name="overrideShortTitle">Переопределенный сокращенный заголовок (если изменен в профиле).</param>
        /// <exception cref="ArgumentNullException">Вызывается, если <paramref name="source"/> или <paramref name="httpContext"/> равен <see langword="null"/>.</exception>
        public NavItem(
            _MapItem_Proto source,
            HttpContext httpContext,
            bool isActive = false,
            bool isActiveBranch = false,
            IReadOnlyList<NavItem>? children = null,
            string? overrideTitle = null,
            string? overrideShortTitle = null)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(httpContext);

            Source = source;
            Title = overrideTitle ?? source.Title;
            ShortTitle = overrideShortTitle ?? (overrideTitle is not null ? overrideTitle : source.ShortTitle);
            TitleHtml = overrideTitle is not null ? Title.ToHtml(true) : source.TitleHtml;
            ShortTitleHtml = (overrideTitle is not null || overrideShortTitle is not null)
                ? (ShortTitle.Equals(Title, StringComparison.Ordinal) ? TitleHtml : ShortTitle.ToHtml(true))
                : source.ShortTitleHtml;
            Url = source.GetUrl(httpContext);
            IsActive = isActive;
            IsActiveBranch = isActiveBranch;
            Children = children ?? [];

            if (source is NodemapItem node1)
            {
                IsGroup = node1.Type is NodemapTypesEnum.Group;
                IsExternal = node1.Type is NodemapTypesEnum.ExternalUrl;
            }
            else if (source is PagemapItem page1)
            {
                IsDisabled = page1.HasNoIndex;
                IsExternal = page1.Type is PagemapTypesEnum.ExternalUrl;
            }
        }


        /// <summary>
        /// Инициализирует новый экземпляр синтетического (ручного) элемента навигации,
        /// не привязанного к статической карте сайта.
        /// </summary>
        /// <param name="url">URL-адрес ссылки.</param>
        /// <param name="title">Отображаемый заголовок элемента.</param>
        /// <param name="shortTitle">Опциональный сокращенный заголовок. Если не задан, используется <paramref name="title"/>.</param>
        /// <param name="isDisabled">Признак отключения кликабельности ссылки.</param>
        /// <param name="isActive">Признак активного состояния текущего элемента.</param>
        /// <param name="display">Декларативное правило видимости в формате <c>"теги_показа|теги_скрытия"</c>.</param>
        /// <param name="passthroughs">Декларативное правило сквозного прохода в формате <c>"теги_включения|теги_исключения"</c>.</param>
        public NavItem(
            string? url,
            string title,
            string? shortTitle = null,
            bool isDisabled = false,
            bool isActive = false,
            string? display = null,
            string? passthroughs = null)
        {
            Title = title ?? string.Empty;
            ShortTitle = string.IsNullOrEmpty(shortTitle)
                ? Title
                : shortTitle;
            TitleHtml = Title.ToHtml(true);
            ShortTitleHtml = ShortTitle.Equals(Title, StringComparison.Ordinal)
                ? TitleHtml
                : ShortTitle.ToHtml(true);
            Url = url ?? string.Empty;
            IsDisabled = isDisabled;
            IsActive = isActive;
            Children = [];

            (_displayOnTags, _displayOffTags) = SuppValues.ParseRule(display);
            (_passthroughsOnTags, _passthroughsOffTags) = SuppValues.ParseRule(passthroughs);
        }


        /* readonly properties */


        /// <summary>
        /// Получает исходный узел или страницу карты сайта, если элемент построен на основе карты.
        /// </summary>
        public _MapItem_Proto? Source { get; }


        /// <summary>
        /// Получает полный заголовок элемента навигации.
        /// </summary>
        public string Title { get; }


        /// <summary>
        /// Получает сокращенный заголовок элемента навигации.
        /// </summary>
        public string ShortTitle { get; }


        /// <summary>
        /// Получает безопасное HTML-представление полного заголовка с типографикой.
        /// </summary>
        public HtmlString TitleHtml { get; }


        /// <summary>
        /// Получает безопасное HTML-представление сокращенного заголовка с типографикой.
        /// </summary>
        public HtmlString ShortTitleHtml { get; }


        /// <summary>
        /// Получает вычисленный виртуальный или внешний URL-адрес элемента.
        /// </summary>
        public string Url { get; }


        /// <summary>
        /// Получает признак того, является ли элемент декоративным заголовком группы без ссылки.
        /// </summary>
        public bool IsGroup { get; }


        /// <summary>
        /// Получает признак того, что ссылка на элемент отключена (например, из-за <c>HasNoIndex</c>).
        /// </summary>
        public bool IsDisabled { get; }


        /// <summary>
        /// Получает признак того, что элемент ведет на внешний ресурс.
        /// </summary>
        public bool IsExternal { get; }


        /// <summary>
        /// Получает признак точного совпадения элемента с текущей открытой страницей или узлом.
        /// </summary>
        public bool IsActive { get; }


        /// <summary>
        /// Получает признак того, что элемент является предком текущей открытой страницы или узла.
        /// </summary>
        public bool IsActiveBranch { get; }


        /// <summary>
        /// Получает коллекцию отфильтрованных дочерних элементов навигации.
        /// </summary>
        public IReadOnlyList<NavItem> Children { get; }


        /// <summary>
        /// Получает признак наличия дочерних элементов навигации.
        /// </summary>
        public bool HasChildren
            => Children is { Count: > 0 };


        /* functions */


        /// <summary>
        /// Проверяет, разрешено ли отображение элемента для указанного набора активных тегов.
        /// </summary>
        /// <param name="activeTags">Коллекция активных тегов навигатора.</param>
        /// <returns><see langword="true"/>, если элемент должен отображаться; иначе — <see langword="false"/>.</returns>
        public bool IsDisplayed(
            IEnumerable<string>? activeTags = null)
        {
            if (Source is not null)
                return Source.IsDisplayed(activeTags);

            if (_displayOffTags is null && _displayOnTags is null)
                return true;

            if (activeTags is null)
                return _displayOnTags is null;

            if (_displayOnTags is not null)
            {
                foreach (var tag1 in activeTags)
                    if (tag1 is not null && Array.IndexOf(_displayOnTags, tag1.ToLowerInvariant()) != -1)
                        return true;
                return false;
            }

            if (_displayOffTags is not null)
                foreach (var tag1 in activeTags)
                    if (tag1 is not null && Array.IndexOf(_displayOffTags, tag1.ToLowerInvariant()) != -1)
                        return false;

            return true;
        }


        /// <summary>
        /// Проверяет, является ли элемент прозрачным (сквозным) для указанного набора активных тегов.
        /// </summary>
        /// <param name="activeTags">Коллекция активных тегов навигатора.</param>
        /// <returns><see langword="true"/>, если элемент должен быть пропущен с подъемом потомков; иначе — <see langword="false"/>.</returns>
        public bool IsPassthrough(
            IEnumerable<string>? activeTags = null)
        {
            if (Source is not null)
                return Source.IsPassthrough(activeTags);

            if (_passthroughsOnTags is null && _passthroughsOffTags is null)
                return false;

            if (activeTags is null)
                return _passthroughsOnTags is null;

            if (_passthroughsOnTags is not null)
            {
                foreach (var tag1 in activeTags)
                    if (tag1 is not null && Array.IndexOf(_passthroughsOnTags, tag1.ToLowerInvariant()) != -1)
                        return true;
                return false;
            }

            if (_passthroughsOffTags is not null)
                foreach (var tag1 in activeTags)
                    if (tag1 is not null && Array.IndexOf(_passthroughsOffTags, tag1.ToLowerInvariant()) != -1)
                        return false;

            return true;
        }


        /// <summary>
        /// Преобразует элемент навигации в строитель HTML-ссылки <see cref="LinkBuilder"/>
        /// с автоматическим переносом состояний активности, блокировки и внешних ссылок.
        /// </summary>
        /// <param name="useShortTitle">Если <see langword="true"/>, использует <see cref="ShortTitle"/>; иначе — <see cref="Title"/>.</param>
        /// <returns>Настроенный экземпляр <see cref="LinkBuilder"/>.</returns>
        public LinkBuilder ToLinkBuilder(
            bool useShortTitle = true)
        {
            return new LinkBuilder(
                string.IsNullOrEmpty(Url) ? null : Url,
                useShortTitle ? ShortTitle : Title,
                isDisabled: IsDisabled || IsGroup)
            {
                IsExternal = IsExternal,
                IsActive = IsActive,
                IsSubActive = IsActiveBranch
            };
        }

    }

}