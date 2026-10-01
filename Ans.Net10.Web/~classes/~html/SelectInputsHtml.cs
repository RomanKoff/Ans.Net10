// rev 2026-10-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Высокоуровневый HTML-компонент, отображающий элементы реестра в виде группы инлайновых 
	/// Bootstrap-флажков (<see cref="CheckboxHtml"/>) или переключателей (<see cref="RadioHtml"/>).
	/// </summary>
	/// <remarks>
	/// Применяется в веб-формах в качестве альтернативы стандартным элементам <c>&lt;select&gt;</c>. 
	/// Поддерживает автоматический вывод заголовков групп элементов через теги <c>&lt;p&gt;</c>.
	/// </remarks>
	public class SelectInputsHtml
		: TagBuilderExt
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="SelectInputsHtml"/>, формируя контейнер и коллекцию дочерних элементов управления.
		/// </summary>
		/// <param name="name">Системное имя группы элементов управления для привязки данных формы (атрибут <c>name</c>).</param>
		/// <param name="value">Массив предварительно выбранных строковых значений (ключей).</param>
		/// <param name="registry">Реестр данных (справочник), содержащий элементы для генерации списка.</param>
		/// <param name="isMultiple">Признак множественного выбора. Если <see langword="true"/>, генерируются чекбоксы; иначе — радиокнопки.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="name"/> равен <see langword="null"/>.</exception>
		public SelectInputsHtml(
			string name,
			string?[]? value,
			RegistryList? registry,
			bool isMultiple)
			: base("div", TagRenderMode.Normal)
		{
			ArgumentNullException.ThrowIfNull(name);
			Name = name;
			Value = value ?? [];
			Registry = registry;
			IsMultiple = isMultiple;
			InnerHtml.AppendLine("");
			if (Registry?.HasItems ?? false)
				InnerHtml.AppendHtml(GetItems());
			else
				InnerHtml.AppendHtml($"{Common.Resources.Common.Text_EmptyItem}");
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает системное имя группы элементов управления.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает массив предварительно выбранных значений.
		/// </summary>
		public string?[] Value { get; }


		/// <summary>
		/// Возвращает реестр данных, используемый для построения группы переключателей.
		/// </summary>
		public RegistryList? Registry { get; }


		/// <summary>
		/// Возвращает признак поддержки множественного выбора (вывода чекбоксов).
		/// </summary>
		public bool IsMultiple { get; }


		/* functions */


		/// <summary>
		/// Генерирует и возвращает строковую HTML-разметку внутренних переключателей и заголовков групп.
		/// </summary>
		/// <remarks>
		/// Если включен режим множественного выбора (<see cref="IsMultiple"/>), элементы дополнительно сортируются по алфавиту.
		/// </remarks>
		/// <returns>Строка с HTML-разметкой внутренних элементов. Если реестр пуст, возвращает <see cref="string.Empty"/>.</returns>
		public string GetItems()
		{
			if (Registry?.HasItems ?? false)
			{
				var sb1 = new StringBuilder();
				var items1 = IsMultiple
					? Registry.Items.OrderBy(x => x.Value)
					: Registry.Items;
				foreach (var item1 in items1)
				{
					var itemValue1 = item1.Value ?? string.Empty;
					var itemKey1 = item1.Key ?? string.Empty;
					if (item1.IsLabel)
						sb1.AppendLine($"<p>{itemValue1}</p>");
					else
					{
						bool isCurrentChecked1 = Value != null && Value.Contains(itemKey1);
						TagBuilderExt input1 = IsMultiple
							? new CheckboxHtml(
								Name,
								$"{Name}_{itemKey1}",
								itemKey1,
								itemValue1,
								true,
								isCurrentChecked1)
							: new RadioHtml(
								Name,
								itemKey1,
								itemValue1,
								null,
								true,
								isCurrentChecked1);
						sb1.AppendLine(input1.ToString());
					}
				}
				return sb1.ToString();
			}
			return string.Empty;
		}

	}

}
