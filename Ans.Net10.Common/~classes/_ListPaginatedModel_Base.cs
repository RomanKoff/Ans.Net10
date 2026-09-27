// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Базовый класс для формирования постраничных моделей данных
	/// (DTO/Read-моделей) с автоматическим маппингом элементов.
	/// </summary>
	/// <typeparam name="TEntity">Тип исходной доменной сущности базы данных. Должен быть ссылочным типом (<see langword="class"/>).</typeparam>
	/// <typeparam name="TModel">Тип результирующей UI-модели или объекта переноса данных (DTO). Должен быть ссылочным типом (<see langword="class"/>).</typeparam>
	public class _ListPaginatedModel_Base<TEntity, TModel>
		where TEntity : class
		where TModel : class
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_ListPaginatedModel_Base{TEntity, TModel}"/>, 
		/// выполняя пагинацию исходного LINQ-запроса и безопасную однократную материализацию элементов текущей страницы.
		/// </summary>
		/// <remarks>
		/// Внутри конструктора используется хелпер <see cref="PaginatedQueryableHelper{TEntity}"/> для применения 
		/// операторов секционирования (Skip/Take) и вычисления общего состояния постраничной навигации.
		/// </remarks>
		/// <param name="query">Исходный запрос <see cref="IQueryable{TEntity}"/> до применения ограничений пагинации.</param>
		/// <param name="func">Делегат функции маппинга (проекции) из доменной сущности <typeparamref name="TEntity"/> в выходную модель <typeparamref name="TModel"/>.</param>
		/// <param name="page">Номер запрашиваемой страницы. Индексация начинается с 1.</param>
		/// <param name="itemsOnPage">Максимальное количество отображаемых элементов на одной странице. Должно быть больше 0.</param>
		protected _ListPaginatedModel_Base(
			IQueryable<TEntity> query,
			Func<TEntity, TModel> func,
			int page,
			int itemsOnPage)
		{
			var helper1 = new PaginatedQueryableHelper<TEntity>(query, page, itemsOnPage);
			Pagination = new PaginationModel(helper1.PaginationHelper);
			Items = [.. helper1.Query.AsEnumerable().Select(func)];
			ItemsCount = Items.Count;
			HasItems = ItemsCount > 0;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает неизменяемую модель состояния пагинации (<see cref="PaginationModel"/>) для интерфейсного слоя.
		/// </summary>
		/// <value>
		/// Объект структуры <see cref="PaginationModel"/>, содержащий информацию о количестве страниц, текущей позиции и навигационных флагах.
		/// </value>
		public PaginationModel Pagination { get; }


		/// <summary>
		/// Возвращает материализованную коллекцию спроецированных выходных моделей текущей страницы.
		/// </summary>
		/// <value>
		/// Интерфейс <see cref="IReadOnlyCollection{TModel}"/>, предоставляющий доступ к элементам текущей страницы только для чтения.
		/// </value>
		public IReadOnlyCollection<TModel> Items { get; }


		/// <summary>
		/// Возвращает фактическое количество элементов, материализованных на текущей странице.
		/// </summary>
		/// <value>
		/// Целочисленное значение (<see cref="int"/>), равное размеру коллекции <see cref="Items"/>.
		/// </value>
		public int ItemsCount { get; }


		/// <summary>
		/// Возвращает признак наличия элементов на текущей странице.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если текущая страница содержит хотя бы один элемент; в противном случае — <see langword="false"/>.
		/// </value>
		public bool HasItems { get; }

	}

}
