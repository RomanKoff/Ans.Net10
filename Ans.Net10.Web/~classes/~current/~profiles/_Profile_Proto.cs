using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	public abstract class _Profile_Proto
	{

		private string? _shortTitle = null;
		private HtmlString? _titleHtml = null;
		private HtmlString? _shortTitleHtml = null;


		/* ctor */


		protected _Profile_Proto()
		{
		}


		/* properties */


		public string? LocalUrl { get; set; } = null;
		public string? ExternalUrl { get; set; } = null;


		/// <summary>
		/// Получает или задает основной заголовок элемента контента.
		/// </summary>
		/// <value>Строковое значение заголовка. Не может быть равен <see langword="null"/>.</value>
		public string Title
		{
			get;
			set
			{
				field = value ?? string.Empty;
				_titleHtml = null;
			}
		} = string.Empty;


		/// <summary>
		/// Получает или задает сокращенный заголовок элемента контента для компактных представлений.
		/// </summary>
		/// <value>Сокращенный текст заголовка. Если сокращенная форма не задана явно, возвращает значение свойства <see cref="Title"/>.</value>
		public string? ShortTitle
		{
			get => _shortTitle ?? Title;
			set
			{
				_shortTitle = string.IsNullOrEmpty(value)
					? null : value;
				_shortTitleHtml = null;
			}
		}


		/* readonly properties */


		/// <summary>
		/// Получает безопасное HTML-представление основного заголовка с примененными правилами типографики.
		/// </summary>
		/// <value>Объект <see cref="HtmlString"/>, готовый к выводу в Razor-представлениях без экранирования.</value>
		public HtmlString TitleHtml
			=> _titleHtml ??= Title.ToHtml(true);


		/// <summary>
		/// Получает безопасное HTML-представление сокращенного заголовка с примененными правилами типографики. 
		/// Если сокращенный заголовок не задан, возвращает HTML-представление основного заголовка.
		/// </summary>
		/// <value>Объект <see cref="HtmlString"/> для вывода в компактных блоках интерфейса.</value>
		public HtmlString ShortTitleHtml
			=> _shortTitleHtml ??= ShortTitle.ToHtml(true);


		/// <summary>
		/// Возвращает признак того, является ли сокращенный заголовок уникальным (установлен отличным от <see cref="Title"/>).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сокращенный заголовок задан и не совпадает с основным; иначе — <see langword="false"/>.</value>
		public bool IsShortTitleUnique
			=> _shortTitle != null
				&& !Title.Equals(_shortTitle, StringComparison.Ordinal);


		/* methods */


		public void SetFace(
			string? face)
		{
			if (string.IsNullOrWhiteSpace(face))
			{
				Title = string.Empty;
				ShortTitle = null;
				return;
			}
			var a1 = face.Split('|', 2);
			if (a1[1] == null)
				Title = a1[0];
			else
			{
				Title = a1[1];
				ShortTitle = string.Format(a1[1], a1[0]);
			}
		}

	}

}
