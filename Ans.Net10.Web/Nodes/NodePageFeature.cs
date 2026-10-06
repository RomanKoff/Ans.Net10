namespace Ans.Net10.Web.Nodes
{

	/// <summary>
	/// Определяет фичу HTTP-контекста для хранения и передачи результата 
	/// сопоставления Узла и Страницы CMS в конвейере обработки запроса.
	/// </summary>
	public interface INodePageFeature
	{
		/// <summary>
		/// Получает результат успешного сопоставления текущего запроса с Узлом и Страницей.
		/// </summary>
		NodePageResolveResult ResolveResult { get; }
	}



	/// <summary>
	/// Реализация фичи HTTP-контекста для хранения результата сопоставления CMS.
	/// </summary>
	public sealed class NodePageFeature
		: INodePageFeature
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="NodePageFeature"/> 
		/// с указанием вычисленного результата сопоставления.
		/// </summary>
		/// <param name="resolveResult">Результат сопоставления Узла и Страницы.</param>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="resolveResult"/> равен <see langword="null"/>.
		/// </exception>
		public NodePageFeature(
			NodePageResolveResult resolveResult)
		{
			ResolveResult = resolveResult
				?? throw new ArgumentNullException(nameof(resolveResult));
		}


		/* readonly properties */


		/// <inheritdoc />
		public NodePageResolveResult ResolveResult { get; }

	}

}
