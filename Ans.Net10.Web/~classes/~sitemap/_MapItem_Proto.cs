// rev 2026-10-22

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Абстрактный прототип класса для элементов карты сайта (Узел, Страница).
	/// </summary>
	public abstract class _MapItem_Proto
	{

		/* abstract */


		/// <summary>
		/// При переопределении в производном классе получает полный относительный ЧПУ-маршрут (Path) элемента структуры контента.
		/// </summary>
		/// <value>Строка канонического относительного пути, используемая для сопоставления маршрутов и индексации.</value>
		/// <returns>Строка канонического относительного пути (например, "/catalog").</returns>
		public abstract string Path { get; }


		/// <summary>
		/// Формирует полный виртуальный URL-адрес элемента с учетом базового пути приложения.
		/// </summary>
		/// <param name="httpContext">Контекст текущего HTTP-запроса.</param>
		/// <returns>Абсолютный относительный путь к элементу навигации.</returns>
		public abstract string GetUrl(HttpContext httpContext);


		/* ctor */


		/// <summary>
		/// Инициализирует базовые параметры прототипа элемента карты сайта.
		/// </summary>
		protected _MapItem_Proto()
		{
		}


		/* readonly properties */


		/// <summary>
		/// Получает основной заголовок элемента.
		/// </summary>
		/// <returns>Основной заголовок элемента структуры контента.</returns>
		public string Title { get; private set; } = string.Empty;


		/// <summary>
		/// Получает сокращенный заголовок элемента для компактных представлений.
		/// Если сокращенная форма не задана явно, возвращает значение свойства <see cref="Title"/>.
		/// </summary>
		/// <returns>Сокращенный заголовок для таблиц и компактных меню.</returns>
		public string ShortTitle { get; private set; } = string.Empty;


		/// <summary>
		/// Получает безопасное HTML-представление основного заголовка элемента с примененными правилами типографики.
		/// </summary>
		/// <value>Объект <see cref="HtmlString"/>, готовый к выводу в Razor-представлениях без экранирования.</value>
		/// <returns>Безопасная HTML-строка с оттипографленным основным заголовком.</returns>
		public HtmlString TitleHtml { get; private set; } = HtmlString.Empty;


		/// <summary>
		/// Получает безопасное HTML-представление сокращенного заголовка элемента с примененными правилами типографики. 
		/// Если сокращенный заголовок не задан, возвращает HTML-представление основного заголовка.
		/// </summary>
		/// <value>Объект <see cref="HtmlString"/> для вывода в компактных blocks интерфейса.</value>
		/// <returns>Безопасная HTML-строка с оттипографленным сокращенным заголовком.</returns>
		public HtmlString ShortTitleHtml { get; private set; } = HtmlString.Empty;


		/// <summary>
		/// Возвращает признак того, является ли сокращенный заголовок элемента уникальным (установлен отличным от <see cref="Title"/>).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сокращенный заголовок задан и не совпадает с основным; иначе — <see langword="false"/>.</value>
		/// <returns><see langword="true"/>, если сокращенный заголовок не совпадает с основным; иначе — <see langword="false"/>.</returns>
		public bool IsShortTitleUnique { get; private set; }


		/* methods */


		/// <summary>
		/// Декларативно устанавливает основной и сокращенный заголовки элемента карты сайта на основе переданной форматированной строки.
		/// </summary>
		/// <param name="face">
		/// Строка разметки заголовков. Может содержать разделитель <c>'|'</c> в форматах: 
		/// <c>"Заголовок"</c>, <c>"Сокр|ПолноеНазвание"</c> или <c>"Сокр|Шаблон с {0}"</c> 
		/// (например: <c>"УЦР|Управление цифрового развития"</c> или <c>"Ученый совет|{0} университета"</c>).
		/// </param>
		public void SetFace(
			string? face)
		{
			IsShortTitleUnique = false;
			if (string.IsNullOrWhiteSpace(face))
			{
				Title = string.Empty;
				ShortTitle = string.Empty;
				TitleHtml = HtmlString.Empty;
				ShortTitleHtml = HtmlString.Empty;
				return;
			}
			var a1 = face.SplitFix('|', 2);
			if (string.IsNullOrEmpty(a1[1]))
			{
				Title = a1[0];
				ShortTitle = Title;
				TitleHtml = Title.ToHtml(true);
				ShortTitleHtml = TitleHtml;
				return;
			}
			var s1 = a1[1];
			ShortTitle = a1[0];
			Title = s1.Contains("{0}")
				? s1.Replace("{0}", ShortTitle)
				: s1;
			TitleHtml = Title.ToHtml(true);
			IsShortTitleUnique = !Title.Equals(ShortTitle, StringComparison.Ordinal);
			ShortTitleHtml = IsShortTitleUnique
				? ShortTitle.ToHtml(true)
				: TitleHtml;
		}


		/* functions */


		/// <summary>
		/// Формирует объект ссылки <see cref="LinkBuilder"/> для текущего элемента карты на основе контекста запроса.
		/// </summary>
		/// <param name="httpContext">Текущий контекст HTTP-запроса, используемый для генерации URL.</param>
		/// <returns>Новый экземпляр <see cref="LinkBuilder"/>, содержащий сформированный URL и основной заголовок элемента.</returns>
		/// <exception cref="ArgumentNullException">Инициируется, если параметр <paramref name="httpContext"/> равен <see langword="null"/>.</exception>
		public LinkBuilder GetLink(
			HttpContext httpContext)
		{
			ArgumentNullException.ThrowIfNull(httpContext);
			return new LinkBuilder(GetUrl(httpContext), Title);
		}


		/// <summary>
		/// Определяет, должен ли элемент отображаться в интерфейсе на основе переданного тега контекста.
		/// </summary>
		/// <param name="tag">Тег для проверки видимости элемента. Если равен <see langword="null"/>, проверяется общая доступность элемента.</param>
		/// <returns><see langword="true"/>, если элемент разрешен к отображению; иначе — <see langword="false"/>.</returns>
		public bool IsDisplayed(
			string? tag = null)
		{
			if (string.IsNullOrWhiteSpace(tag))
				return _DisplayOnTags is null;
			if (_DisplayOnTags is not null && Array.IndexOf(_DisplayOnTags, tag.ToLowerInvariant()) != -1)
				return true;
			if (_DisplayOffTags is not null && Array.IndexOf(_DisplayOffTags, tag.ToLowerInvariant()) != -1)
				return false;
			return _DisplayOnTags is null;
		}


		/// <summary>
		/// Проверяет, удовлетворяет ли элемент условиям отображения на основе набора активных тегов контекста (например, ролей пользователя).
		/// </summary>
		/// <param name="activeTags">Коллекция активных тегов контекста.</param>
		/// <returns><see langword="true"/>, если элемент должен быть отображен; иначе — <see langword="false"/>.</returns>
		public bool IsDisplayed(
			IEnumerable<string>? activeTags)
		{
			if (_DisplayOffTags is null && _DisplayOnTags is null)
				return true;
			if (activeTags is null)
				return _DisplayOnTags is null;
			if (_DisplayOnTags is not null)
			{
				foreach (var tag1 in activeTags)
					if (tag1 is not null && Array.IndexOf(_DisplayOnTags, tag1.ToLowerInvariant()) != -1)
						return true;
				return false;
			}
			if (_DisplayOffTags is not null)
				foreach (var tag1 in activeTags)
					if (tag1 is not null && Array.IndexOf(_DisplayOffTags, tag1.ToLowerInvariant()) != -1)
						return false;
			return true;
		}


		/// <summary>
		/// Определяет, является ли текущий элемент структуры «сквозным» (прозрачным) для указанного тега.
		/// </summary>
		/// <param name="tag">Тег для проверки сквозного поведения.</param>
		/// <returns><see langword="true"/>, если элемент является прозрачным; иначе — <see langword="false"/>.</returns>
		public bool IsPassthrough(
			string? tag = null)
		{
			if (_PassthroughsOnTags is null && _PassthroughsOffTags is null)
				return false;
			if (string.IsNullOrWhiteSpace(tag))
				return _PassthroughsOnTags is null;
			var lowerTag1 = tag.ToLowerInvariant();
			if (_PassthroughsOnTags is not null && Array.IndexOf(_PassthroughsOnTags, lowerTag1) != -1)
				return true;
			if (_PassthroughsOffTags is not null && Array.IndexOf(_PassthroughsOffTags, lowerTag1) != -1)
				return false;
			return _PassthroughsOnTags is null;
		}



		/// <summary>
		/// Определяет, является ли текущий элемент структуры «сквозным» при наличии активных тегов контекста.
		/// </summary>
		/// <param name="activeTags">Коллекция активных тегов контекста.</param>
		/// <returns><see langword="true"/>, если сработал сквозной проход; иначе — <see langword="false"/>.</returns>
		public bool IsPassthrough(
			IEnumerable<string>? activeTags)
		{
			if (_PassthroughsOnTags is null && _PassthroughsOffTags is null)
				return false;
			if (activeTags is null)
				return _PassthroughsOnTags is null;
			if (_PassthroughsOnTags is not null)
			{
				foreach (var tag1 in activeTags)
					if (tag1 is not null && Array.IndexOf(_PassthroughsOnTags, tag1.ToLowerInvariant()) != -1)
						return true;
				return false;
			}
			if (_PassthroughsOffTags is not null)
				foreach (var tag1 in activeTags)
					if (tag1 is not null && Array.IndexOf(_PassthroughsOffTags, tag1.ToLowerInvariant()) != -1)
						return false;
			return true;
		}


		/* internals */


		/// <summary>
		/// Получает очищенный массив тегов обязательного отображения для высокопроизводительного поиска.
		/// </summary>
		/// <returns>Массив уникальных строковых тегов видимости в нижнем регистре либо <see langword="null"/>, если теги не заданы.</returns>
		internal string[]? _DisplayOnTags { get; set; }


		/// <summary>
		/// Получает очищенный массив тегов принудительного скрытия для высокопроизводительного поиска.
		/// </summary>
		/// <returns>Массив уникальных строковых тегов скрытия в нижнем регистре либо <see langword="null"/>, если теги не заданы.</returns>
		internal string[]? _DisplayOffTags { get; set; }


		/// <summary>
		/// Получает очищенный массив тегов активации «сквозного» поведения.
		/// </summary>
		/// <returns>Массив строковых тегов включения сквозного прохода либо <see langword="null"/>.</returns>
		internal string[]? _PassthroughsOnTags { get; set; }


		/// <summary>
		/// Получает очищенный массив тегов принудительного отключения «сквозного» поведения.
		/// </summary>
		/// <returns>Массив строковых тегов исключения сквозного прохода либо <see langword="null"/>.</returns>
		internal string[]? _PassthroughsOffTags { get; set; }

	}

}
