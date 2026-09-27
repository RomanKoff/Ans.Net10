// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Интерфейс для описания визуального отображения, локализации 
	/// и метаданных полей ввода в рамках CRUD-интерфейса.
	/// </summary>
	public interface ICrudFace
	{
		/// <summary>
		/// Системное (программное) имя поля.
		/// </summary>
		/// <value>Строка, содержащая идентификатор свойства или поля формы.</value>
		string Name { get; }

		/// <summary>
		/// Основной заголовок поля (Label), отображаемый пользователю в интерфейсе.
		/// </summary>
		/// <value>Текстовое значение заголовка.</value>
		string Title { get; }

		/// <summary>
		/// Сокращенный заголовок поля для использования в компактных блоках (например, в шапках таблиц GRID).
		/// </summary>
		/// <value>Сокращенное текстовое значение заголовка.</value>
		string ShortTitle { get; }

		/// <summary>
		/// Подробное описание, примечание или всплывающая текстовая подсказка (Tooltip) для поля.
		/// </summary>
		/// <value>Текст описания или подсказки.</value>
		string Description { get; }

		/// <summary>
		/// Пример заполнения поля, используемый в качестве водяного знака / подсказки внутри элемента ввода (Placeholder).
		/// </summary>
		/// <value>Строка с примером данных.</value>
		string Sample { get; }

		/// <summary>
		/// URL-ссылка на внешнюю справочную веб-документацию или контекстную справку по данному полю.
		/// </summary>
		/// <value>Строка с адресом гиперссылки.</value>
		string HelpLink { get; }
	}



	/// <summary>
	/// Реализация интерфейса отображения полей CRUD с поддержкой парсинга 
	/// из сериализованных строк и автоматическим расчетом заголовков по умолчанию.
	/// </summary>
	public class CrudFace
		: ICrudFace
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="CrudFace"/>, 
		/// разбирая параметры отображения из единой строки с разделителями.
		/// </summary>
		/// <remarks>
		/// Для разбора строки используется внутренний <see cref="StringParser"/> со стандартным разделителем <c>'|'</c>.
		/// </remarks>
		/// <param name="name">Системное имя поля.</param>
		/// <param name="face">
		/// Сериализованная строка формата <c>"title|shortTitle|description|sample|helpLink"</c>. 
		/// Пустые или недостающие сегменты инициализируются как <see cref="string.Empty"/>.
		/// </param>
		public CrudFace(
			string name,
			string face)
		{
			var parser1 = new StringParser(face);
			Name = name;
			TitleRaw = parser1.Get(0) ?? string.Empty;
			ShortTitleRaw = parser1.Get(1) ?? string.Empty;
			Description = parser1.Get(2) ?? string.Empty;
			Sample = parser1.Get(3) ?? string.Empty;
			HelpLink = parser1.Get(4) ?? string.Empty;
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="CrudFace"/> с явным указанием всех параметров отображения.
		/// </summary>
		/// <param name="name">Системное имя поля.</param>
		/// <param name="title">Основной заголовок поля.</param>
		/// <param name="shortTitle">Сокращенный заголовок поля.</param>
		/// <param name="description">Подробное описание или подсказка поля.</param>
		/// <param name="sample">Пример заполнения поля (плейсхолдер).</param>
		/// <param name="helpLink">Ссылка на справочную документацию.</param>
		public CrudFace(
			string name,
			string title,
			string shortTitle,
			string description,
			string sample,
			string helpLink)
		{
			Name = name;
			TitleRaw = title;
			ShortTitleRaw = shortTitle;
			Description = description;
			Sample = sample;
			HelpLink = helpLink;
		}


		/* readonly properties */


		/// <inheritdoc />
		public string Name { get; } = string.Empty;


		/// <summary>
		/// Возвращает исходное сырое значение основного заголовка, переданное при инициализации.
		/// </summary>
		/// <value>Исходная строка заголовка без применения правил подстановки по умолчанию.</value>
		public string TitleRaw { get; } = string.Empty;


		/// <summary>
		/// Возвращает исходное сырое значение короткого заголовка, переданное при инициализации.
		/// </summary>
		/// <value>Исходная строка короткого заголовка без применения правил подстановки по умолчанию.</value>
		public string ShortTitleRaw { get; } = string.Empty;


		/// <inheritdoc />
		public string Description { get; } = string.Empty;


		/// <inheritdoc />
		public string Sample { get; } = string.Empty;


		/// <inheritdoc />
		public string HelpLink { get; } = string.Empty;


		/// <summary>
		/// Возвращает рассчитанный основной заголовок поля.
		/// </summary>
		/// <remarks>
		/// Если свойство <see cref="TitleRaw"/> пусто или не задано, метод автоматически 
		/// возвращает системное имя поля из свойства <see cref="Name"/>.
		/// </remarks>
		/// <value>Рассчитанный заголовок для отображения.</value>
		public string Title
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => HasTitle
				? TitleRaw : Name;
		}


		/// <summary>
		/// Возвращает рассчитанный короткий заголовок поля.
		/// </summary>
		/// <remarks>
		/// Если свойство <see cref="ShortTitleRaw"/> пусто или не задано, метод автоматически 
		/// возвращает вычисленный заголовок из свойства <see cref="Title"/>.
		/// </remarks>
		/// <value>Рассчитанный короткий заголовок для компактных представлений.</value>
		public string ShortTitle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => HasShortTitle
				? ShortTitleRaw : Title;
		}


		/// <summary>
		/// Возвращает признак наличия заполненного основного заголовка.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="TitleRaw"/> не содержит пустую строку; иначе — <see langword="false"/>.</value>
		public bool HasTitle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !string.IsNullOrEmpty(TitleRaw);
		}


		/// <summary>
		/// Возвращает признак наличия заполненного короткого заголовка.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="ShortTitleRaw"/> не содержит пустую строку; иначе — <see langword="false"/>.</value>
		public bool HasShortTitle
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !string.IsNullOrEmpty(ShortTitleRaw);
		}


		/// <summary>
		/// Возвращает признак наличия подробного описания или подсказки поля.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="Description"/> не содержит пустую строку; иначе — <see langword="false"/>.</value>
		public bool HasDescription
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !string.IsNullOrEmpty(Description);
		}


		/// <summary>
		/// Возвращает признак наличия примера заполнения (плейсхолдера).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="Sample"/> не содержит пустую строку; иначе — <see langword="false"/>.</value>
		public bool HasSample
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !string.IsNullOrEmpty(Sample);
		}


		/// <summary>
		/// Возвращает признак наличия ссылки на внешнюю документацию.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если <see cref="HelpLink"/> не содержит пустую строку; иначе — <see langword="false"/>.</value>
		public bool HasHelpLink
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => !string.IsNullOrEmpty(HelpLink);
		}


		/// <summary>
		/// Возвращает признак наличия хотя бы одного заполненного визуального параметра отображения.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если хотя бы один из флагов проверки параметров 
		/// (<see cref="HasTitle"/>, <see cref="HasShortTitle"/>, <see cref="HasDescription"/>, <see cref="HasSample"/> или <see cref="HasHelpLink"/>) 
		/// равен <see langword="true"/>; в противном случае — <see langword="false"/>.
		/// </value>
		public bool HasFace
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => HasTitle
				|| HasShortTitle
				|| HasDescription
				|| HasSample
				|| HasHelpLink;
		}


		/* functions */


		/// <summary>
		/// Формирует сериализованную текстовую строку на основе текущих сырых параметров отображения поля.
		/// </summary>
		/// <returns>Строка параметров, объединенная разделителем <c>'|'</c> в формате <c>"title|shortTitle|description|sample|helpLink"</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return $"{TitleRaw}|{ShortTitleRaw}|{Description}|{Sample}|{HelpLink}";
		}

	}

}
