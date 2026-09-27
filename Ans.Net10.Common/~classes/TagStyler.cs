// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Объединенный контейнер стилей и классов для централизованного
	/// управления визуальным оформлением HTML-тегов.
	/// </summary>
	public class TagStyler
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagStyler"/> на основе готовых строителей классов и стилей.
		/// </summary>
		/// <param name="classes">Экземпляр строителя CSS-классов. Если равен <see langword="null"/>, создается новый пустой строитель.</param>
		/// <param name="styles">Экземпляр строителя инлайновых CSS-стилей. Если равен <see langword="null"/>, создается новый пустой строитель.</param>
		public TagStyler(
			TagClassesBuilder? classes,
			TagStylesBuilder? styles = null)
		{
			Classes = classes ?? new TagClassesBuilder();
			Styles = styles ?? new TagStylesBuilder();
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagStyler"/> на основе строковых определений классов и стилей.
		/// </summary>
		/// <param name="cssClasses">Строка CSS-классов, разделенная пробелами.</param>
		/// <param name="cssStyles">Строка инлайновых CSS-стилей (например, <c>"color:red;padding:10px;"</c>).</param>
		public TagStyler(
			string? cssClasses,
			string? cssStyles = null)
			: this(new TagClassesBuilder(cssClasses), new TagStylesBuilder(cssStyles))
		{
		}


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="TagStyler"/>.
		/// </summary>
		public TagStyler()
		{
			Classes = new TagClassesBuilder();
			Styles = new TagStylesBuilder();
		}


		/* properties */


		/// <summary>
		/// Возвращает или задает строитель CSS-классов.
		/// </summary>
		/// <value>
		/// Текущий экземпляр <see cref="TagClassesBuilder"/>, используемый для управления классами HTML-тега.
		/// </value>
		public TagClassesBuilder Classes { get; set; }


		/// <summary>
		/// Возвращает или задает строитель инлайновых CSS-стилей.
		/// </summary>
		/// <value>
		/// Текущий экземпляр <see cref="TagStylesBuilder"/>, используемый для управления инлайновыми стилями HTML-тега.
		/// </value>
		public TagStylesBuilder Styles { get; set; }


		/* methods */


		/// <summary>
		/// Безопасно применяет базовые (оригинальные) классы и стили из другого стайлера,
		/// не затирая текущие уникальные параметры.
		/// </summary>
		/// <remarks>
		/// Метод вызывает оригинальные методы <c>ApplyOriginal</c> у вложенных строителей, благодаря чему 
		/// новые классы и стили добавляются только в том случае, если их префиксы или свойства отсутствуют в текущем контейнере.
		/// </remarks>
		/// <param name="styler">Стайлер-донор, чьи базовые настройки необходимо перенести в текущий экземпляр.</param>
		public void ApplyBase(
			TagStyler styler)
		{
			Classes.ApplyOriginal(styler.Classes.Items);
			Styles.ApplyOriginal(styler.Styles.Items);
		}

	}

}
