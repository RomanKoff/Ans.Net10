using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Абстрактный прототип класса для профилей контента (Сайт, Узел, Страница).
	/// </summary>
	public abstract class _Profile_Proto
	{

		/* abstracts */


		/// <summary>
		/// Выполняет расчет иерархической навигации, хлебных крошек и заголовков браузера 
		/// для текущего профиля контента.
		/// </summary>
		public abstract void CalcPartNavigation();


		/* ctor */


		/// <summary>
		/// Инициализирует базовые параметры прототипа профиля контента.
		/// </summary>
		protected _Profile_Proto()
		{
		}


		/* properties */


		/// <summary>
		/// Получает или задает основной заголовок элемента контента.
		/// </summary>
		public string Title
		{
			get;
			set
			{
				field = value ?? string.Empty;
				TitleHtml = null!;
			}
		} = string.Empty;


		/// <summary>
		/// Получает или задает сокращенный заголовок элемента контента для компактных представлений.
		/// Если сокращенная форма не задана явно, возвращает значение свойства <see cref="Title"/>.
		/// </summary>
		public string? ShortTitle
		{
			get => field ?? Title;
			set
			{
				field = string.IsNullOrEmpty(value)
					? null! : value;
				ShortTitleHtml = null!;
			}
		}


		/* readonly properties */


		/// <summary>
		/// Получает безопасное HTML-представление основного заголовка с примененными правилами типографики.
		/// </summary>
		/// <value>Объект <see cref="HtmlString"/>, готовый к выводу в Razor-представлениях без экранирования.</value>
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
		public HtmlString ShortTitleHtml
		{
			get => field ??= ShortTitle.ToHtml(true);
			private set;
		}


		/// <summary>
		/// Возвращает признак того, является ли сокращенный заголовок уникальным (установлен отличным от <see cref="Title"/>).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сокращенный заголовок задан и не совпадает с основным; иначе — <see langword="false"/>.</value>
		public bool IsShortTitleUnique
			=> ShortTitle != Title;


		/* methods */


		/// <summary>
		/// Декларативно устанавливает основной и сокращенный заголовки профиля на основе переданной форматированной строки.
		/// </summary>
		/// <param name="face">
		/// Строка разметки заголовков. Может содержать разделитель <c>'|'</c> в форматах: 
		/// <c>"Заголовок"</c>, <c>"Сокр|ПолноеНазвание"</c> или <c>"Сокр|Шаблон с {0}"</c> 
		/// (например: <c>"УЦР|Управление цифрового развития"</c> или <c>"Ученый совет|{0} университета"</c>).
		/// </param>
		public void SetFace(
			string? face)
		{
			if (string.IsNullOrWhiteSpace(face))
			{
				Title = string.Empty;
				ShortTitle = null!;
				return;
			}
			var a1 = face.SplitFix('|', 2);
			if (string.IsNullOrEmpty(a1[1]))
			{
				Title = a1[0];
				ShortTitle = null!;
			}
			else
			{
				Title = a1[1].Contains("{0}")
					? a1[1].Replace("{0}", a1[0])
					: a1[1];
				ShortTitle = a1[0];
			}
		}

	}

}
