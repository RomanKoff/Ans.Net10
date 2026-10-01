// rev 2026-10-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации элемента выбора <c>&lt;option&gt;</c> 
	/// в составе выпадающих списков, поддерживающий визуальные иерархические отступы.
	/// </summary>
	public class OptionTag
		: TagBuilderExt
	{

		/* consts */


		/// <summary>
		/// Строковая HTML-сущность, используемая в качестве одного шага отступа для отображения иерархии (вложенности) элементов.
		/// </summary>
		public const string OPTION_TABS = "&nbsp;&nbsp;&nbsp;";


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="OptionTag"/>, формируя значение, текст, уровень вложенности и состояние выбора элемента.
		/// </summary>
		/// <param name="value">Передаваемое на сервер значение элемента списка (атрибут <c>value</c>).</param>
		/// <param name="inner">Отображаемый пользовательский текст элемента.</param>
		/// <param name="level">Уровень вложенности (иерархии) элемента. Определяет количество повторений отступов <see cref="OPTION_TABS"/>.</param>
		/// <param name="selected">Признак начальной выбранности элемента. Если <see langword="true"/>, добавляется атрибут <c>selected="selected"</c>.</param>
		public OptionTag(
			string? value,
			string? inner,
			int level,
			bool selected)
			: base("option", TagRenderMode.Normal)
		{
			Value = value ?? string.Empty;
			Inner = inner ?? string.Empty;
			Level = level < 0 ? 0 : level;
			IsSelected = selected;
			MergeAttribute("value", Value);
			if (IsSelected)
				MergeAttribute("selected", "selected");
			InnerHtml.AppendHtml(
				$"{OPTION_TABS.MakeRepeats(Level)}{Inner}");
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает передаваемое системное значение элемента списка.
		/// </summary>
		public string Value { get; }


		/// <summary>
		/// Возвращает исходный отображаемый текст элемента без учета иерархических отступов.
		/// </summary>
		public string Inner { get; }


		/// <summary>
		/// Возвращает уровень вложенности текущего элемента в иерархии списка.
		/// </summary>
		public int Level { get; }


		/// <summary>
		/// Возвращает признак того, выбран ли данный элемент в текущий момент.
		/// </summary>
		public bool IsSelected { get; }

	}

}
