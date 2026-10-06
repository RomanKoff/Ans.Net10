namespace Ans.Net10.Web.Nodes
{

	/// <summary>
	/// Описывает результат успешного анализа и сопоставления входящего URL-запроса 
	/// с конкретным иерархическим узлом и страницей в файловой структуре представлений.
	/// </summary>
	public sealed class NodePageResolveResult
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodePageResolveResult"/> 
		/// с указанием параметров сопоставления.
		/// </summary>
		/// <param name="nodeName">Имя (идентификатор) вычисленного Узла контента.</param>
		/// <param name="viewPath">Полный виртуальный путь к файлу представления для движка Razor.</param>
		/// <param name="pagePath">Относительный путь к странице внутри дерева страниц Узла.</param>
		/// <param name="isStartPage">Признак того, является ли выбранная страница стартовой (start.cshtml).</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если один из обязательных строковых параметров равен <see langword="null"/>.
		/// </exception>
		public NodePageResolveResult(
			string nodeName,
			string viewPath,
			string pagePath,
			bool isStartPage)
		{
			NodeName = nodeName
				?? throw new ArgumentNullException(nameof(nodeName));
			ViewPath = viewPath
				?? throw new ArgumentNullException(nameof(viewPath));
			PagePath = pagePath
				?? throw new ArgumentNullException(nameof(pagePath));
			IsStartPage = isStartPage;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает имя (идентификатор) вычисленного Узла контента (например, "dep1" или "_main").
		/// </summary>
		public string NodeName { get; }

		/// <summary>
		/// Возвращает полный виртуальный путь к файлу представления, пригодный для передачи 
		/// в движок рендеринга Razor (например, "~/Views/Nodes/dep1/about.cshtml").
		/// </summary>
		public string ViewPath { get; }

		/// <summary>
		/// Возвращает относительный путь к странице внутри дерева страниц текущего Узла (например, "about/contacts").
		/// </summary>
		public string PagePath { get; }

		/// <summary>
		/// Возвращает признак того, является ли выбранная страница стартовой страницей по умолчанию (start.cshtml) 
		/// для корня Узла или одного из его внутренних подразделов (подкаталогов).
		/// </summary>
		public bool IsStartPage { get; }

	}

}
