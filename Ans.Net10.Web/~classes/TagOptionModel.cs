// rev 2026-09-28

namespace Ans.Net10.Web
{

	/// <summary>
	/// Модель данных, описывающая отдельный элемент (опцию) <c>&lt;option&gt;</c> в HTML-структуре выпадающего списка.
	/// </summary>
	public class TagOptionModel
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagOptionModel"/> со значениями по умолчанию.
		/// </summary>
		public TagOptionModel()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TagOptionModel"/> с базовым наполнением параметров.
		/// </summary>
		/// <param name="value">Техническое значение атрибута <c>value</c>.</param>
		/// <param name="inner">Отображаемый текстовый контент элемента.</param>
		/// <param name="isSelected">Флаг, определяющий, выбран ли элемент по умолчанию.</param>
		/// <param name="level">Уровень вложенности элемента в иерархической структуре (по умолчанию 0).</param>
		public TagOptionModel(
			string value,
			string inner,
			bool isSelected = false,
			int level = 0)
		{
			Value = value ?? string.Empty;
			Inner = inner ?? string.Empty;
			IsSelected = isSelected;
			Level = level;
		}


		/* properties */


		/// <summary>
		/// Получает или задает техническое значение элемента, передаваемое на сервер (атрибут <c>value</c>).
		/// </summary>
		public string Value { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает видимый пользователю внутренний текст элемента.
		/// </summary>
		public string Inner { get; set; } = string.Empty;


		/// <summary>
		/// Получает или задает флаг, указывающий на то, что данный элемент является выбранным (атрибут <c>selected</c>).
		/// </summary>
		public bool IsSelected { get; set; } = false;


		/// <summary>
		/// Получает или задает уровень вложенности элемента, используемый для визуального смещения 
		/// или группировки в иерархических списках.
		/// </summary>
		public int Level { get; set; } = 0;

	}

}
