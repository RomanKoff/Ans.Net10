using Microsoft.AspNetCore.Html;

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
				IsShortTitleUnique = !field.Equals(ShortTitle, StringComparison.Ordinal);
			}
		} = string.Empty;


		/// <summary>
		/// Получает или задает сокращенный заголовок элемента контента для компактных представлений.
		/// Если сокращенная форма не задана явно, возвращает значение свойства <see cref="Title"/>.
		/// </summary>
		public string ShortTitle
		{
			get => field is not { Length: > 0 }
				? Title : field;
			set
			{
				field = value ?? string.Empty;
				ShortTitleHtml = null!;
				IsShortTitleUnique = !Title.Equals(field, StringComparison.Ordinal);
			}
		} = string.Empty;


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
			get => IsShortTitleUnique
				? (field ??= ShortTitle.ToHtml(true))
				: TitleHtml;
			private set;
		}


		/// <summary>
		/// Возвращает признак того, является ли сокращенный заголовок уникальным (установлен отличным от <see cref="Title"/>).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сокращенный заголовок задан и не совпадает с основным; иначе — <see langword="false"/>.</value>
		public bool IsShortTitleUnique { get; private set; }

	}

}
