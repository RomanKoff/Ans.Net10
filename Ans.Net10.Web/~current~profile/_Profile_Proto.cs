// rev 2026-11-24

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Routing;

namespace Ans.Net10.Web
{

    /// <summary>
    /// Абстрактный прототип класса для профилей контента (Сайт, Узел, Страница).
    /// </summary>
    public abstract class _Profile_Proto
    {

        internal readonly CurrentContext _Current;


        /* ctor */


        /// <summary>
        /// Инициализирует базовые параметры прототипа профиля контента.
        /// </summary>
        protected _Profile_Proto(
            CurrentContext current)
        {
            _Current = current;
        }


        /* abstracts */


        /// <summary>
		/// При переопределении в производном классе получает или задает CSS-классы контейнера для текущего профиля.
		/// </summary>
		/// <returns>Строка CSS-классов контейнера.</returns>
		public abstract string ContainerClasses { get; set; }


        /// <summary>
		/// При переопределении в производном классе получает полный относительный URL-адрес текущего профиля.
		/// </summary>
		/// <returns>Строка относительного URL-адреса профиля контента либо <see langword="null"/>.</returns>
		public abstract string? Url { get; }


        /// <summary>
        /// При переопределении в производном классе получает или задает URL-адрес ресурса (например, изображения) профиля.
        /// </summary>
        /// <returns>Строка URL-адреса ресурса либо <see langword="null"/>.</returns>
        public abstract string? ResUrl { get; set; }


        /// <summary>
        /// При переопределении в производном классе формирует элемент навигации <see cref="NavItem"/> для текущего профиля.
        /// </summary>
        /// <returns>Экземпляр <see cref="NavItem"/> или <see langword="null"/>, если элемент не может быть построен.</returns>
        public abstract NavItem? GetNavItem();


        /* properties */


        /// <summary>
		/// Получает или задает основной заголовок элемента контента.
		/// </summary>
		/// <returns>Строка основного заголовка элемента контента.</returns>
		public string Title
        {
            get;
            set
            {
                field = value ?? string.Empty;
                TitleHtml = null!;
                IsShortTitleUnique = !field.Equals(ShortTitle, StringComparison.Ordinal);
                _resetParents();
            }
        } = string.Empty;


        /// <summary>
		/// Получает или задает сокращенный заголовок элемента контента для компактных представлений.
		/// Если сокращенная форма не задана явно, возвращает значение свойства <see cref="Title"/>.
		/// </summary>
		/// <returns>Строка сокращенного заголовка элемента контента.</returns>
		public string ShortTitle
        {
            get => field is not { Length: > 0 } ? Title : field;
            set
            {
                field = value ?? string.Empty;
                ShortTitleHtml = null!;
                IsShortTitleUnique = !Title.Equals(field, StringComparison.Ordinal);
                _resetParents();
            }
        } = string.Empty;


        /* readonly properties */


        /// <summary>
		/// Получает безопасное HTML-представление основного заголовка с примененными правилами типографики.
		/// </summary>
		/// <value>Объект <see cref="HtmlString"/>, готовый к выводу в Razor-представлениях без экранирования.</value>
		/// <returns>Безопасная HTML-строка с оттипографленным основным заголовком.</returns>
		public HtmlString TitleHtml
        {
            get => field ??= Title.ToHtml(true);
            private set;
        }


        /// <summary>
        /// Получает безопасное HTML-представление сокращенного заголовка с примененными правилами типографики. 
        /// Если сокращенный заголовок не задан, возвращает HTML-представление основного заголовка.
        /// </summary>
        /// <value>Объект <see cref="HtmlString"/> для вывода в компактных блоках интерфейса.</value>
        /// <returns>Безопасная HTML-строка с оттипографленным сокращенным заголовком.</returns>
        public HtmlString ShortTitleHtml
        {
            get => IsShortTitleUnique
                ? (field ??= ShortTitle.ToHtml(true))
                : TitleHtml;
            private set;
        }


        /// <summary>
		/// Возвращает признак того, является ли сокращенный заголовок уникальным (установлен отличным от <see cref="Title"/>).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сокращенный заголовок задан и не совпадает с основным; иначе — <see langword="false"/>.</value>
		/// <returns><see langword="true"/>, если сокращенный заголовок уникален; иначе — <see langword="false"/>.</returns>
		public bool IsShortTitleUnique { get; private set; }


        /// <summary>
        /// Получает список элементов навигации вышестоящих родительских узлов или страниц текущего профиля.
        /// </summary>
        /// <returns>Изменяемый список родительских элементов <see cref="NavItem"/>.</returns>
        public List<NavItem> Parents { get; internal set; } = [];


        /// <summary>
        /// Получает лениво инициализируемый элемент прямого родителя текущего профиля.
        /// </summary>
        /// <returns>Элемент <see cref="NavItem"/> прямого родителя либо <see langword="null"/>.</returns>
        public NavItem? ParentItem
            => field ??= GetDirectParent();


        /// <summary>
        /// Получает признак наличия зарегистрированных родительских элементов.
        /// </summary>
        /// <returns><see langword="true"/>, если у профиля есть родительские элементы; иначе — <see langword="false"/>.</returns>
        public bool HasParents
            => Parents is not [];


        /* functions */


        /// <summary>
        /// Возвращает элемент прямого родителя текущего профиля (ближайшего предка в иерархии).
        /// </summary>
        /// <returns>Объект <see cref="NavItem"/> прямого родителя либо <see langword="null"/>.</returns>
        public NavItem? GetDirectParent()
        {
            return HasParents
                ? Parents[^1] : null;
        }


        /// <summary>
        /// Возвращает объединенную строку из оттипографленных заголовков всех родительских элементов.
        /// </summary>
        /// <returns>Строка текстовой иерархии предков с разделителем.</returns>
        public string GetParentsTitles()
        {
            return _parentsTitles ??= HasParents
                ? SuppTypograph.GetTypografMin(
                    $"{string.Join(". ", Parents.Select(x => x.ShortTitleHtml))}. ")
                : string.Empty;
        }
        private string? _parentsTitles = null;


        /// <summary>
        /// Формирует локальную последовательность элементов цепочки навигации («хлебных крошек»)
        /// для текущего профиля с учетом правил видимости и сквозного прохода.
        /// </summary>
        /// <returns>Последовательность элементов <see cref="NavItem"/>, включающая родителей и текущий элемент.</returns>
        public IEnumerable<NavItem> GetBreadcrumbsPart()
        {
            if (HasParents)
                foreach (var parent1 in Parents)
                    yield return parent1;

            var current1 = GetNavItem();
            if (current1 is not null && current1.ShortTitle is { Length: > 0 })
                yield return current1;
        }


        /* methods */


        /// <summary>
        /// Добавляет готовый элемент навигации родителя в конец текущей коллекции.
        /// </summary>
        /// <param name="item">Элемент навигации <see cref="NavItem"/>.</param>
        public void AddParent(
            NavItem item)
        {
            _resetParents();
            Parents.Add(item);
        }


        /// <summary>
        /// Формирует и добавляет элемент навигации родителя по указанному URL и заголовку.
        /// </summary>
        /// <param name="url">URL-адрес ссылки.</param>
        /// <param name="title">Заголовок элемента.</param>
        /// <param name="display">Опциональное правило видимости (<c>"показ|скрытие"</c>).</param>
        /// <param name="passthroughs">Опциональное правило сквозного прохода (<c>"включение|исключение"</c>).</param>
        public void AddParent(
            string url,
            string title,
            string? display = null,
            string? passthroughs = null)
        {
            AddParent(_getNavItem(url, title, display, passthroughs));
        }


        /// <summary>
        /// Формирует и добавляет элемент навигации родителя на основе MVC-действия (Action).
        /// </summary>
        /// <param name="action">Имя действия контроллера.</param>
        /// <param name="controller">Имя контроллера.</param>
        /// <param name="routes">Объект параметров маршрута.</param>
        /// <param name="title">Заголовок элемента.</param>
        public void AddParentFromAction(
            string action,
            string controller,
            object routes,
            string title)
        {
            AddParent(_getNavItem(action, controller, routes, title));
        }


        /// <summary>
        /// Вставляет готовый элемент навигации родителя в указанную позицию списка.
        /// </summary>
        /// <param name="index">Порядковый индекс позиции вставки.</param>
        /// <param name="item">Элемент навигации <see cref="NavItem"/>.</param>
        public void InsertParent(
            int index,
            NavItem item)
        {
            _resetParents();
            Parents.Insert(index, item);
        }


        /// <summary>
        /// Формирует и вставляет элемент навигации родителя в указанную позицию списка по URL и заголовку.
        /// </summary>
        /// <param name="index">Порядковый индекс позиции вставки.</param>
        /// <param name="url">URL-адрес ссылки.</param>
        /// <param name="title">Заголовок элемента.</param>
        /// <param name="display">Опциональное правило видимости (<c>"показ|скрытие"</c>).</param>
        /// <param name="passthroughs">Опциональное правило сквозного прохода (<c>"включение|исключение"</c>).</param>
        public void InsertParent(
            int index,
            string url,
            string title,
            string? display = null,
            string? passthroughs = null)
        {
            InsertParent(index, _getNavItem(url, title, display, passthroughs));
        }


        /// <summary>
        /// Формирует и вставляет элемент навигации родителя на основе MVC-действия в указанную позицию.
        /// </summary>
        /// <param name="index">Порядковый индекс позиции вставки.</param>
        /// <param name="action">Имя действия контроллера.</param>
        /// <param name="controller">Имя контроллера.</param>
        /// <param name="routes">Объект параметров маршрута.</param>
        /// <param name="title">Заголовок элемента.</param>
        public void InsertParentFromAction(
            int index,
            string action,
            string controller,
            object routes,
            string title)
        {
            InsertParent(index, _getNavItem(action, controller, routes, title));
        }


        /// <summary>
        /// Удаляет родительский элемент из коллекции по его порядковому индексу.
        /// </summary>
        /// <param name="index">Порядковый индекс удаляемого элемента.</param>
        public void RemoveParent(
            int index)
        {
            _resetParents();
            if (Parents.Count > index)
                Parents.RemoveAt(index);
        }


        /// <summary>
        /// Удаляет последний родительский элемент из коллекции.
        /// </summary>
        public void RemoveParentLast()
        {
            _resetParents();
            if (Parents is not [])
                Parents.RemoveAt(Parents.Count - 1);
        }


        /* privates */


        private void _resetParents()
        {
            _parentsTitles = null;
            _Current._ResetBreadcrumbsCache();
        }


        private NavItem _getNavItem(
            string url,
            string title,
            string? display = null,
            string? passthroughs = null)
        {
            var url1 = _Current.GetSmartUrl(url);
            return new NavItem(url1, title, display: display, passthroughs: passthroughs);
        }


        private NavItem _getNavItem(
            string action,
            string controller,
            object routes,
            string title)
        {
            var url1 = _Current.LinkGenerator.GetPathByAction(action, controller, routes);
            return new NavItem(url1, title);
        }

    }

}
