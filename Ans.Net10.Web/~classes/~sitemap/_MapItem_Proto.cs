// rev 2026-10-21

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

	}

}
