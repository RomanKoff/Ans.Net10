// rev 2026-10-21

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
		/// При переопределении в производном классе формирует объект ссылки <see cref="LinkBuilder"/> для текущего профиля.
		/// </summary>
		/// <returns>Экземпляр <see cref="LinkBuilder"/> или <see langword="null"/>, если ссылка не может быть построена.</returns>
		public abstract LinkBuilder? GetLink();


		/* ctor */


		/// <summary>
		/// Инициализирует базовые параметры прототипа профиля контента.
		/// </summary>
		protected _Profile_Proto(
			CurrentContext current)
		{
			_Current = current;
		}


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
		/// Получает список ссылок на вышестоящие родительские элементы текущего профиля.
		/// </summary>
		/// <value>Коллекция объектов <see cref="LinkBuilder"/>.</value>
		/// <returns>Изменяемый список ссылок на родительские элементы.</returns>
		public List<LinkBuilder> ParentsLinks { get; internal set; } = [];


		/// <summary>
		/// Получает признак наличия зарегистрированных родительских элементов.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если список родителей содержит элементы; иначе — <see langword="false"/>.</value>
		/// <returns><see langword="true"/>, если у профиля есть родительские элементы; иначе — <see langword="false"/>.</returns>
		public bool HasParents
			=> ParentsLinks is not [];


		/* methods */


		/// <summary>
		/// Добавляет готовый объект ссылки родительского элемента в конец текущей коллекции.
		/// </summary>
		/// <param name="link">Объект ссылки <see cref="LinkBuilder"/>.</param>
		public void AddParent(
			LinkBuilder link)
		{
			_resetParents();
			ParentsLinks.Add(link);
		}


		/// <summary>
		/// Формирует и добавляет ссылку родительского элемента по указанному URL и заголовку.
		/// </summary>
		public void AddParent(
			string url,
			string title)
		{
			_resetParents();
			AddParent(_getLink(url, title));
		}


		/// <summary>
		/// Формирует и добавляет ссылку родительского элемента на основе MVC-действия (Action).
		/// </summary>
		public void AddParentFromAction(
			string action,
			string controller,
			object routes,
			string title)
		{
			_resetParents();
			AddParent(_getLink(action, controller, routes, title));
		}


		/// <summary>
		/// Вставляет готовый объект ссылки родительского элемента в указанную позицию списка.
		/// </summary>
		public void InsertParent(
			int index,
			LinkBuilder link)
		{
			_resetParents();
			ParentsLinks.Insert(index, link);
		}


		/// <summary>
		/// Формирует и вставляет ссылку родительского элемента в указанную позицию списка по URL и заголовку.
		/// </summary>
		public void InsertParent(
			int index,
			string url,
			string title)
		{
			_resetParents();
			InsertParent(index, _getLink(url, title));
		}


		/// <summary>
		/// Формирует и вставляет ссылку родительского элемента на основе MVC-действия в указанную позицию.
		/// </summary>
		public void InsertParentFromAction(
			int index,
			string action,
			string controller,
			object routes,
			string title)
		{
			_resetParents();
			InsertParent(index, _getLink(action, controller, routes, title));
		}


		/// <summary>
		/// Удаляет родительский элемент из коллекции по его порядковому индексу.
		/// </summary>
		/// <param name="index">Порядковый индекс удаляемого элемента.</param>
		public void RemoveParent(
			int index)
		{
			_resetParents();
			if (ParentsLinks.Count > index)
				ParentsLinks.RemoveAt(index);
		}


		/// <summary>
		/// Удаляет последний родительский элемент из коллекции.
		/// </summary>
		public void RemoveParentLast()
		{
			_resetParents();
			if (ParentsLinks is not [])
				ParentsLinks.RemoveAt(ParentsLinks.Count - 1);
		}


		/* functions */


		/// <summary>
		/// Возвращает объединенную строку из оттипографленных заголовков всех родительских элементов.
		/// </summary>
		/// <returns>Строка текстовой иерархии предков с разделителем.</returns>
		public string GetParentsTitles()
		{
			return _parentsTitles ??= HasParents
				? SuppTypograph.GetTypografMin(
					$"{string.Join(". ", ParentsLinks.Select(x => x.InnerHtml))}. ")
				: string.Empty;
		}
		private string? _parentsTitles = null;


		/// <summary>
		/// Формирует и возвращает ленивую локальную часть коллекции элементов цепочки навигации («хлебных крошек»).
		/// </summary>
		/// <returns>Перечисление объектов <see cref="LinkBuilder"/>, включающее родителей и текущий элемент.</returns>
		public IEnumerable<LinkBuilder> GetBreadcrumbsPart()
		{
			if (_breadcrumbsPart is not null)
				return _breadcrumbsPart;
			var items1 = new List<LinkBuilder>();
			if (HasParents)
				items1.AddRange(ParentsLinks);
			var item1 = GetLink();
			if (item1 is not null && item1.InnerHtml is { Length: > 0 })
				items1.Add(item1);
			_breadcrumbsPart = items1.AsReadOnly();
			return _breadcrumbsPart;
		}
		private IEnumerable<LinkBuilder>? _breadcrumbsPart = null;


		/* privates */


		private void _resetParents()
		{
			_parentsTitles = null;
			_breadcrumbsPart = null;
		}


		private LinkBuilder _getLink(
			string url,
			string title)
		{
			var url1 = _Current.GetSmartUrl(url);
			return new LinkBuilder(url1, title);
		}


		private LinkBuilder _getLink(
			string action,
			string controller,
			object routes,
			string title)
		{
			var url1 = _Current.LinkGenerator.GetPathByAction(action, controller, routes);
			return new LinkBuilder(url1, title);
		}

	}

}
