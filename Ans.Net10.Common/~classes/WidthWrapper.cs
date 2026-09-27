// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень вариантов стандартной ширины для контентных блоков и элементов интерфейса.
	/// </summary>
	public enum WidthsEnum
	{
		/// <summary>
		/// Вся доступная ширина (100%).
		/// </summary>
		Full,

		/// <summary>
		/// Очень широкая область (40rem).
		/// </summary>
		ExtraLarge,

		/// <summary>
		/// Широкая область (30rem).
		/// </summary>
		Large,

		/// <summary>
		/// Средняя область (20rem).
		/// </summary>
		Medium,

		/// <summary>
		/// Узкая область (15rem).
		/// </summary>
		Small,

		/// <summary>
		/// Очень узкая область (10rem).
		/// </summary>
		ExtraSmall,

		/// <summary>
		/// Ширина не задается (свободное поведение элемента).
		/// </summary>
		Nothing
	}



	/// <summary>
	/// Компонент-обертка для создания HTML-контейнеров с фиксированной или адаптивной шириной.
	/// </summary>
	/// <param name="html">Внутреннее HTML-содержимое блока.</param>
	/// <param name="width">Режим ширины из перечисления <see cref="WidthsEnum"/>.</param>
	/// <param name="cssClass">CSS-класс, добавляемый к тегу контейнера.</param>
	public class WidthWrapper(
		string? html,
		WidthsEnum width,
		string? cssClass)
	{

		/* readonly properties */


		/// <summary>
		/// Возвращает внутреннее HTML-содержимое блока.
		/// </summary>
		/// <value>Строка с HTML-кодом. Если при инициализации был передан <see langword="null"/>, возвращает пустую строку.</value>
		public string Html { get; } = html ?? string.Empty;


		/// <summary>
		/// Возвращает выбранный режим ширины контейнера.
		/// </summary>
		/// <value>Значение из перечисления <see cref="WidthsEnum"/>.</value>
		public WidthsEnum Width { get; } = width;


		/// <summary>
		/// Возвращает CSS-класс контейнера.
		/// </summary>
		/// <value>Имя CSS-класса. Если при инициализации был передан <see langword="null"/>, возвращает пустую строку.</value>
		public string CssClass { get; } = cssClass ?? string.Empty;


		/// <summary>
		/// Возвращает инлайновый инкапсулированный CSS-стиль ширины на основе выбранного режима.
		/// </summary>
		/// <value>
		/// Строка вида <c>"width:X"</c> для инлайнового атрибута <c>style</c>, 
		/// либо <see langword="null"/>, если выбран режим <see cref="WidthsEnum.Nothing"/>.
		/// </value>
		public string? AutoStyle
			=> field ??= Width switch
			{
				WidthsEnum.Full => "width:100%",
				WidthsEnum.ExtraLarge => "width:40rem",
				WidthsEnum.Large => "width:30rem",
				WidthsEnum.Medium => "width:20rem",
				WidthsEnum.Small => "width:15rem",
				WidthsEnum.ExtraSmall => "width:10rem",
				WidthsEnum.Nothing => null,
				_ => "width:6rem"
			};


		/* functions */


		/// <summary>
		/// Формирует итоговую HTML-строку контейнера &lt;div&gt; с подстановкой класса и инлайновых стилей.
		/// </summary>
		/// <returns>Строка валидного HTML-кода разметки блочного элемента.</returns>
		public override string ToString()
		{
			string styleAttribute1 = string.IsNullOrEmpty(AutoStyle)
				? string.Empty
				: $" style=\"{AutoStyle}\"";
			string classAttribute1 = string.IsNullOrEmpty(CssClass)
				? string.Empty
				: $" class=\"{CssClass}\"";
			return $"<div{classAttribute1}{styleAttribute1}>{Html}</div>";
		}

	}

}
