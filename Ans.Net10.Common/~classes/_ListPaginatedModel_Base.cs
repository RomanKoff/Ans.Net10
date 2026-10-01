// rev 2026-09-25

using Microsoft.EntityFrameworkCore;

namespace Ans.Net10.Common
{

	/*
		Пример использования:
		 
		public class UserCatalogViewModel
			: _ListPaginatedModel_Base<UserEntity, UserModel>
		{
			private UserCatalogViewModel(
				PaginationModel pagination,
				IReadOnlyCollection<UserModel> items) 
				: base(pagination, items)
			{
			}
	
			public static async Task<UserCatalogViewModel> CreateAsync(
				IQueryable<UserEntity> query,
				int page,
				int size)
			{
				var data1 = await LoadDataAsync(query, x => new UserModel(x.Name), page, size);
				return new UserCatalogViewModel(data1.Pagination, data1.Items);
			}
		}

	 */


	/// <summary>
	/// Базовый класс для формирования постраничных моделей данных (DTO/Read-моделей) 
	/// с автоматическим асинхронным маппингом элементов.
	/// </summary>
	/// <typeparam name="TEntity">Тип исходной доменной сущности базы данных. Должен относиться к ссылочным типам (<see langword="class"/>).</typeparam>
	/// <typeparam name="TModel">Тип результирующей UI-модели или объекта переноса данных (DTO). Должен относиться к ссылочным типам (<see langword="class"/>).</typeparam>
	public abstract class _ListPaginatedModel_Base<TEntity, TModel>
		where TEntity : class
		where TModel : class
	{

		/* ctor */


		/// <summary>
		/// Инициализирует свойства базового класса <see cref="_ListPaginatedModel_Base{TEntity, TModel}"/> 
		/// на основе предварительно вычисленных и материализованных данных.
		/// </summary>
		/// <param name="pagination">Вычисленная неизменяемая модель состояния постраничной навигации.</param>
		/// <param name="items">Материализованная коллекция спроецированных выходных моделей текущей страницы.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="items"/> равен <see langword="null"/>.</exception>
		protected _ListPaginatedModel_Base(
			PaginationModel pagination,
			IReadOnlyCollection<TModel> items)
		{
			ArgumentNullException.ThrowIfNull(items);
			Pagination = pagination;
			Items = items;
			ItemsCount = items.Count;
			HasItems = ItemsCount > 0;
		}


		/// <summary>
		/// Вспомогательный метод для внешней асинхронной инициализации данных. Выполняет асинхронную пагинацию 
		/// исходного LINQ-запроса, извлекает срез данных из БД и формирует готовую коллекцию DTO-моделей.
		/// </summary>
		/// <remarks>
		/// Метод полностью берет на себя неблокирующие дисковые операции ввода-вывода, используя 
		/// возможности <see cref="PaginatedQueryableHelper{TEntity}.CreateAsync"/> и <see cref="EntityFrameworkQueryableExtensions.ToListAsync"/>.
		/// </remarks>
		/// <param name="query">Исходный запрос <see cref="IQueryable{TEntity}"/> до применения ограничений пагинации.</param>
		/// <param name="func">Делегат функции маппинга (проекции) из доменной сущности <typeparamref name="TEntity"/> в выходную модель <typeparamref name="TModel"/>.</param>
		/// <param name="page">Номер запрашиваемой страницы. Индексация начинается с 1.</param>
		/// <param name="itemsOnPage">Максимальное количество отображаемых элементов на одной странице. Должно быть больше 0.</param>
		/// <param name="cancellationToken">Токен отмены асинхронной операции.</param>
		/// <returns>
		/// Задача, результатом которой является кортеж, содержащий вычисленную структуру 
		/// <see cref="PaginationModel"/> и материализованный массив готовых моделей <see cref="IReadOnlyCollection{TModel}"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Вызывается, если параметр <paramref name="query"/> или <paramref name="func"/> равен <see langword="null"/>.</exception>
		public static async Task<(PaginationModel Pagination, IReadOnlyCollection<TModel> Items)> LoadDataAsync(
			IQueryable<TEntity> query,
			Func<TEntity, TModel> func,
			int page,
			int itemsOnPage,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(query);
			ArgumentNullException.ThrowIfNull(func);
			var helper1 = await PaginatedQueryableHelper<TEntity>.CreateAsync(
				query, page, itemsOnPage, cancellationToken);
			var pagination1 = new PaginationModel(helper1.PaginationHelper);
			var entities1 = await helper1.Query.ToListAsync(cancellationToken);
			IReadOnlyCollection<TModel> items1 = [.. entities1.Select(func)];
			return (pagination1, items1);
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
