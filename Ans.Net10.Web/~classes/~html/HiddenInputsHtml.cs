// rev 2026-10-01

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using System.Text;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Компонент для автоматической генерации группы скрытых полей ввода <c>&lt;input type="hidden" /&gt;</c> 
	/// на основе массива строковых значений для передачи коллекций через HTML-формы.
	/// </summary>
	/// <remarks>
	/// Каждому элементу массива сопоставляется отдельный HTML-тег с именем в формате <c>имя[]</c>.
	/// </remarks>
	public class HiddenInputsHtml
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="HiddenInputsHtml"/> с указанием имени группы полей и массива их значений.
		/// </summary>
		/// <param name="name">Общее системное имя для группы скрытых полей ввода (атрибут <c>name</c>).</param>
		/// <param name="value">Массив строковых значений, для каждого из которых будет сгенерирован отдельный инпут.</param>
		public HiddenInputsHtml(
			string? name,
			string?[]? value)
		{
			Name = name ?? string.Empty;
			Value = value ?? [];
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает базовое имя группы скрытых полей ввода.
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// Возвращает массив значений, преобразуемых в скрытые поля ввода.
		/// </summary>
		public string?[] Value { get; }


		/* functions */


		/// <summary>
		/// Сериализует текущий объект в плоскую HTML-строку, содержащую последовательность тегов скрытых полей ввода.
		/// </summary>
		/// <returns>Строка, содержащая сгенерированную HTML-разметку скрытых полей. Если значений нет, возвращает <see cref="string.Empty"/>.</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Name) || Value.Length == 0)
				return string.Empty;
			var sb1 = new StringBuilder();
			foreach (var item1 in Value)
			{
				if (item1 == null)
					continue;
				sb1.Append($"<input type=\"hidden\" name=\"{Name}[]\"{item1.Make(" value=\"{0}\"")} />");
			}
			return sb1.ToString();
		}


		/// <summary>
		/// Преобразует сгенерированную разметку в безопасный объект <see cref="HtmlString"/>, готовый к прямому выводу в Razor-представлениях.
		/// </summary>
		/// <returns>Экземпляр <see cref="HtmlString"/>, инкапсулирующий сгенерированные скрытые поля ввода.</returns>
		public HtmlString ToHtml()
		{
			return new HtmlString(ToString());
		}

	}

}
