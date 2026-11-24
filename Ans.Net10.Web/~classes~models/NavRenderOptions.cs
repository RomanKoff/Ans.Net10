// rev 2026-11-24

namespace Ans.Net10.Web
{

    /// <summary>
    /// Параметры рендеринга частичных представлений навигации на базе <see cref="NavItem"/>.
    /// </summary>
    public sealed class NavRenderOptions
    {

        /* ctor */


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="NavRenderOptions"/>.
        /// </summary>
        /// <param name="tags">Строка тегов навигатора через запятую (например, <c>"header"</c> или <c>"science,main"</c>).</param>
        /// <param name="maxDepth">Максимальная глубина отрисовки дерева (<see langword="null"/> — без ограничений).</param>
        /// <param name="useShortTitle">Если <see langword="true"/>, используется сокращенный заголовок <see cref="NavItem.ShortTitle"/>.</param>
        /// <param name="cssClass">Дополнительные CSS-классы для корневого контейнера меню.</param>
        public NavRenderOptions(
            string? tags = null,
            int? maxDepth = null,
            bool useShortTitle = true,
            string? cssClass = null)
        {
            Tags = string.IsNullOrWhiteSpace(tags)
                ? null
                : [.. tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
            MaxDepth = maxDepth;
            UseShortTitle = useShortTitle;
            CssClass = cssClass;
        }


        /* readonly properties */


        /// <summary>
        /// Получает массив активных тегов навигатора.
        /// </summary>
        public string[]? Tags { get; }


        /// <summary>
        /// Получает максимальную глубину вложенности дерева.
        /// </summary>
        public int? MaxDepth { get; }


        /// <summary>
        /// Получает признак использования сокращенного заголовка элемента.
        /// </summary>
        public bool UseShortTitle { get; }


        /// <summary>
        /// Получает дополнительные CSS-классы корневого элемента.
        /// </summary>
        public string? CssClass { get; }


        /* functions */


        /// <summary>
        /// Безопасно извлекает параметры рендеринга из произвольной модели представления.
        /// Поддерживает экземпляры <see cref="NavRenderOptions"/> и строковые списки тегов.
        /// </summary>
        /// <param name="model">Модель, переданная в частичное представление.</param>
        /// <returns>Экземпляр <see cref="NavRenderOptions"/>.</returns>
        public static NavRenderOptions Resolve(
            object? model)
        {
            return model switch
            {
                NavRenderOptions opts1 => opts1,
                string tags1 => new NavRenderOptions(tags1),
                _ => new NavRenderOptions()
            };
        }

    }

}
