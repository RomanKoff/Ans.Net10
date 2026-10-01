// rev 2026-10-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Специализированный HTML-компонент для генерации элемента выбора <c>&lt;select&gt;</c>.
	/// </summary>
	/// <remarks>
	/// Автоматически поддерживает одиночный и множественный выбор, динамическое построение 
	/// опций на основе <see cref="RegistryList"/> и иерархическую группировку через теги <c>&lt;optgroup&gt;</c>.
	/// </remarks>
	public class SelectTag
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="SelectTag"/>, формируя структуру выпадающего списка и его опций.
		/// </summary>
		/// <param name="name">Системное имя и уникальный идентификатор HTML-элемента (атрибуты <c>id</c> и <c>name</c>).</param>
		/// <param name="value">Массив предварительно выбранных строковых значений (ключей) элементов.</param>
		/// <param name="registry">Реестр данных (справочник), содержащий элементы для наполнения списка.</param>
		/// <param name="isMultiple">Признак поддержки множественного выбора элементов (атрибут <c>multiple</c>).</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public SelectTag(
			string name,
			string?[]? value,
			RegistryList? registry,
			bool isMultiple)
			: base("select", TagRenderMode.Normal)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			Value = value ?? [];
			Registry = registry;
			IsMultiple = isMultiple;
			MergeAttribute("id", Name);
			MergeAttribute("name", Name);
			if (IsMultiple)
				MergeAttribute("multiple", "multiple");
			InnerHtml.AppendLine("");
			if (Registry?.HasItems ?? false)
				InnerHtml.AppendHtml(GetSelectOptions());
			else
				InnerHtml.AppendHtml($"<option>{Common.Resources.Common.Text_EmptyItem}</option>");
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя и идентификатор элемента выбора.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает массив предварительно выбранных значений.
		/// </summary>
		public string?[] Value { get; }


		/// <summary>
		/// Возвращает реестр данных, используемый для построения списка опций.
		/// </summary>
		public RegistryList? Registry { get; }


		/// <summary>
		/// Возвращает признак поддержки множественного выбора элементов.
		/// </summary>
		public bool IsMultiple { get; }


		/* functions */


		/// <summary>
		/// Генерирует и возвращает строковую HTML-разметку всех внутренних элементов списка (<c>&lt;option&gt;</c> и <c>&lt;optgroup&gt;</c>).
		/// </summary>
		/// <remarks>
		/// Если элемент реестра имеет флаг <c>IsLabel</c>, открывается группа <c>&lt;optgroup&gt;</c>. 
		/// Все неразрывные пробелы в текстах опций автоматически нормализуются.
		/// </remarks>
		/// <returns>Строка с HTML-разметкой внутренних элементов списка. Если реестр пуст, возвращает <see cref="string.Empty"/>.</returns>
		public string GetSelectOptions()
		{
			if (Registry?.HasItems ?? false)
			{
				var sb1 = new StringBuilder();
				var items1 = Registry.Items;
				bool isGroupOpen1 = false;
				foreach (var item1 in items1)
				{
					var itemValue = item1.Value ?? string.Empty;
					if (item1.IsLabel)
					{
						if (isGroupOpen1)
							sb1.AppendLine("</optgroup>");
						sb1.AppendLine($"<optgroup label=\"{itemValue}\">");
						isGroupOpen1 = true;
					}
					else
					{
						var cleanText1 = itemValue
							.Replace("&nbsp;", " ")
							.Replace('\u00a0', ' ');
						var option1 = new OptionTag(
							item1.Key,
							cleanText1,
							IsMultiple
								? 0 : item1.Level,
							Value != null && Value.Contains(item1.Key));
						sb1.AppendLine(option1.ToString());
					}
				}
				if (isGroupOpen1)
					sb1.AppendLine("</optgroup>");
				return sb1.ToString();
			}
			return string.Empty;
		}

	}

}
