// rev 2026-10-07

using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Служба для работы со строкой запроса (Query String) текущего HTTP-запроса, 
	/// предоставляющая инструменты фильтрации, маппинга и генерации элементов управления интерфейса.
	/// </summary>
	/// <remarks>
	/// Инициализирует новый экземпляр класса <see cref="QueryStringService"/> на основе первичного конструктора C#.
	/// </remarks>
	/// <param name="current">Текущий контекст обработки запроса.</param>
	/// <exception cref="ArgumentNullException">
	/// Вызывается, если параметр <paramref name="current"/> равен <see langword="null"/>.
	/// </exception>
	public class QueryStringService(
		CurrentContext current)
		: QueryStringHelper(
			(current ?? throw new ArgumentNullException(nameof(current)))
				.HttpContext?.Request.Query ?? QueryCollection.Empty)
	{

		private readonly CurrentContext _current = current;


		/* functions */


		/// <summary>
		/// Создает изолированный независимый экземпляр <see cref="QueryStringHelper"/> 
		/// на основе текущего запроса с возможностью исключения определенных параметров.
		/// </summary>
		/// <param name="ignoreParams">Список имен параметров, которые необходимо исключить из обработки.</param>
		/// <returns>Новый инициализированный объект хелпера строки запроса.</returns>
		public QueryStringHelper GetHelper(
			params string[] ignoreParams)
		{
			return new QueryStringHelper(
				_current.HttpContext?.Request.Query ?? QueryCollection.Empty,
				ignoreParams);
		}


		/// <summary>
		/// Генерирует HTML-разметку безопасной ссылки-кнопки сортировки для использования в шапках таблиц данных.
		/// </summary>
		/// <remarks>
		/// Метод автоматически анализирует текущий параметр <c>order</c> в URL, инвертирует направление сортировки 
		/// (добавляя или удаляя префикс дефиса <c>-</c>) и подставляет соответствующие CSS-классы и иконки стрелок Bootstrap Icons.
		/// </remarks>
		/// <param name="name">Системное имя поля/колонки для выполнения сортировки.</param>
		/// <param name="innerHtml">Отображаемый текстовый контент или HTML-код внутри кнопки.</param>
		/// <param name="useTypograf">Признак необходимости применения экранной типографики к содержимому кнопки.</param>
		/// <returns>
		/// Объект <see cref="HtmlString"/>, содержащий готовую к выводу валидную HTML-разметку ссылки.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="name"/> или <paramref name="innerHtml"/> равен <see langword="null"/>.
		/// </exception>
		public HtmlString GetSortingButton(
			string name,
			string innerHtml,
			bool useTypograf)
		{
			ArgumentNullException.ThrowIfNull(name);
			ArgumentNullException.ThrowIfNull(innerHtml);
			var queryHelper1 = GetHelper();
			string nameDesc1 = $"-{name}";
			if (useTypograf)
				innerHtml = SuppTypograph.GetTypografMin(innerHtml) ?? string.Empty;
			var cssBuilder1 = new TagClassesBuilder("text-nowrap sorting");
			string finalInnerHtml1;
			if (TestValue("order", name))
			{
				queryHelper1.AppendString("order", nameDesc1);
				cssBuilder1.Append("sorting-asc");
				finalInnerHtml1 = $"<span class=\"text-wrap\">{innerHtml}</span><i class=\"bi-arrow-down\"></i>";
			}
			else if (TestValue("order", nameDesc1))
			{
				queryHelper1.AppendString("order", name);
				cssBuilder1.Append("sorting-desc");
				finalInnerHtml1 = $"<span class=\"text-wrap\">{innerHtml}</span><i class=\"bi-arrow-up\"></i>";
			}
			else
			{
				queryHelper1.Remove("page");
				queryHelper1.AppendString("order", name);
				finalInnerHtml1 = $"<span class=\"text-wrap\">{innerHtml}</span>";
			}
			string resultHtml1 = $"<a class=\"{cssBuilder1}\" href=\"{queryHelper1.MakeQueryString(string.Empty)}\">{finalInnerHtml1}</a>";
			return new HtmlString(resultHtml1);
		}

	}

}