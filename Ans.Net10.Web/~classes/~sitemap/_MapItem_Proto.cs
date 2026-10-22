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


		/// <summary>
		/// Получает очищенный массив тегов принудительного скрытия для высокопроизводительного поиска.
		/// </summary>
		/// <returns>Массив уникальных строковых тегов скрытия в нижнем регистре либо <see langword="null"/>, если теги не заданы.</returns>
		public string[]? HidesArray { get; protected set; }


		/// <summary>
		/// Получает очищенный массив тегов обязательного отображения для высокопроизводительного поиска.
		/// </summary>
		/// <returns>Массив уникальных строковых тегов видимости в нижнем регистре либо <see langword="null"/>, если теги не заданы.</returns>
		public string[]? ShowsArray { get; protected set; }


		/// <summary>
		/// Получает очищенный массив тегов «сквозного» поведения для высокопроизводительного поиска.
		/// </summary>
		/// <returns>Массив уникальных строковых тегов сквозного прохода в нижнем регистре либо <see langword="null"/>, если теги не заданы.</returns>
		public string[]? PassthroughsArray { get; protected set; }


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
		/// Определяет, должен ли элемент отображаться в интерфейсе на основе переданного тега контекста.
		/// </summary>
		/// <param name="tag">Тег для проверки видимости элемента. Если равен <see langword="null"/>, проверяется общая доступность элемента.</param>
		/// <returns><see langword="true"/>, если элемент разрешен к отображению; иначе — <see langword="false"/>.</returns>
		public bool IsDisplayed(
			string? tag = null)
		{
			if (string.IsNullOrWhiteSpace(tag))
				return ShowsArray is null;
			if (ShowsArray is not null && Array.IndexOf(ShowsArray, tag.ToLowerInvariant()) != -1)
				return true;
			if (HidesArray is not null && Array.IndexOf(HidesArray, tag.ToLowerInvariant()) != -1)
				return false;
			return ShowsArray is null;
		}


		/// <summary>
		/// Проверяет, удовлетворяет ли элемент условиям отображения на основе набора активных тегов контекста (например, ролей пользователя).
		/// </summary>
		/// <param name="activeTags">Коллекция активных тегов контекста.</param>
		/// <returns><see langword="true"/>, если элемент должен быть отображен; иначе — <see langword="false"/>.</returns>
		public bool IsDisplayedTags(
			IEnumerable<string>? activeTags)
		{
			if (HidesArray is null && ShowsArray is null)
				return true;
			if (activeTags is null)
				return ShowsArray is null;
			if (ShowsArray is not null)
			{
				foreach (var tag1 in activeTags)
					if (tag1 is not null && Array.IndexOf(ShowsArray, tag1.ToLowerInvariant()) != -1)
						return true;
				return false;
			}
			if (HidesArray is not null)
				foreach (var tag1 in activeTags)
					if (tag1 is not null && Array.IndexOf(HidesArray, tag1.ToLowerInvariant()) != -1)
						return false;
			return true;
		}


		/// <summary>
		/// Определяет, должен ли элемент отображаться в интерфейсе на основе переданной строки активных тегов.
		/// </summary>
		/// <param name="rawActiveTags">Строка с активными тегами контекста, разделенными запятыми или точками с запятой (например, "user,premium").</param>
		/// <returns><see langword="true"/>, если элемент разрешен к отображению; иначе — <see langword="false"/>.</returns>
		public bool IsDisplayedTags(
			string? rawActiveTags)
		{
			if (HidesArray is null && ShowsArray is null)
				return true;
			var parsedTags1 = _ParseTags(rawActiveTags);
			return IsDisplayedTags(parsedTags1);
		}


		/// <summary>
		/// Определяет, является ли текущий элемент структуры «сквозным» (прозрачным) для указанного тега.
		/// </summary>
		/// <param name="tag">Тег для проверки сквозного поведения. Если равен <see langword="null"/>, возвращает <see langword="false"/>.</param>
		/// <returns><see langword="true"/>, если элемент является прозрачным; иначе — <see langword="false"/>.</returns>
		public bool IsPassthrough(
			string? tag = null)
		{
			if (PassthroughsArray is null || string.IsNullOrWhiteSpace(tag))
				return false;
			return Array.IndexOf(PassthroughsArray, tag.ToLowerInvariant()) != -1;
		}


		/// <summary>
		/// Определяет, является ли текущий элемент структуры «сквозным» (прозрачным) при наличии хотя бы одного из активных тегов.
		/// </summary>
		/// <param name="activeTags">Коллекция активных тегов контекста.</param>
		/// <returns><see langword="true"/>, если сработал сквозной проход; иначе — <see langword="false"/>.</returns>
		public bool IsPassthroughTags(
			IEnumerable<string>? activeTags)
		{
			if (PassthroughsArray is null || activeTags is null)
				return false;
			foreach (var tag1 in activeTags)
				if (tag1 is not null && Array.IndexOf(PassthroughsArray, tag1.ToLowerInvariant()) != -1)
					return true;
			return false;
		}


		/// <summary>
		/// Определяет, является ли текущий элемент структуры «сквозным» (прозрачным) при наличии хотя бы одного из тегов в переданной строке.
		/// </summary>
		/// <param name="rawActiveTags">Строка с активными тегами контекста, разделенными запятыми или точками с запятой (например, "system,archive").</param>
		/// <returns><see langword="true"/>, если сработал сквозной проход; иначе — <see langword="false"/>.</returns>
		public bool IsPassthroughTags(
			string? rawActiveTags)
		{
			if (PassthroughsArray is null || string.IsNullOrWhiteSpace(rawActiveTags))
				return false;

			var parsedTags1 = _ParseTags(rawActiveTags);
			return IsPassthroughTags(parsedTags1);
		}


		/* internals */


		/// <summary>
		/// Выполняет разбор сырой строки тегов на очищенный массив уникальных элементов. 
		/// Если строка пуста или не содержит валидных тегов, возвращает <see langword="null"/> для экономии памяти.
		/// </summary>
		/// <param name="rawTags">Сырая строка с тегами, разделенными запятыми или точками с запятой.</param>
		/// <returns>Массив очищенных уникальных тегов в нижнем регистре либо <see langword="null"/>, если теги не обнаружены.</returns>
		internal static string[]? _ParseTags(
			string? rawTags)
		{
			if (string.IsNullOrWhiteSpace(rawTags))
				return null;
			var array1 = rawTags.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim().ToLowerInvariant())
				.Distinct()
				.ToArray();
			return array1.Length == 0
				? null : array1;
		}

	}

}
