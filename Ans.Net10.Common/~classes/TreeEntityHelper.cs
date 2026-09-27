// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Интерфейс сущности, поддерживающей древовидную (иерархическую) структуру в базе данных.
	/// </summary>
	public interface ITreeEntity
	{
		/// <summary>
		/// Получает или задает уникальный идентификатор сущности.
		/// </summary>
		/// <value>Уникальный целочисленный первичный ключ.</value>
		int Id { get; set; }

		/// <summary>
		/// Получает или задает идентификатор родительской сущности.
		/// </summary>
		/// <value>Идентификатор родительской записи, либо <see langword="null"/>, если элемент является корневым.</value>
		int? ParentPtr { get; set; }

		/// <summary>
		/// Получает или задает порядковый номер для сортировки элементов на одном уровне вложенности.
		/// </summary>
		/// <value>Индекс или вес сортировки (обычно по возрастанию).</value>
		int Order { get; set; }
	}



	/// <summary>
	/// Обертка над сущностью древовидной структуры, содержащая вычисленные метаданные иерархии.
	/// </summary>
	/// <typeparam name="TEntity">Тип исходной доменной сущности, реализующей интерфейс <see cref="ITreeEntity"/>.</typeparam>
	public class TreeEntityWrapper<TEntity>
		where TEntity : class, ITreeEntity
	{
		/// <summary>
		/// Получает или задает уникальный идентификатор сущности.
		/// </summary>
		/// <value>Копия идентификатора оригинальной сущности.</value>
		public int Id { get; set; }

		/// <summary>
		/// Получает или задает идентификатор родительской сущности.
		/// </summary>
		/// <value>Идентификатор родительской записи, либо <see langword="null"/> для корневого уровня.</value>
		public int? ParentPtr { get; set; }

		/// <summary>
		/// Получает или задает порядковый номер для сортировки.
		/// </summary>
		/// <value>Порядковый номер элемента внутри текущего уровня иерархии.</value>
		public int Order { get; set; }

		/// <summary>
		/// Получает или задает вычисленный уровень вложенности в иерархическом дереве.
		/// </summary>
		/// <value>Уровень вложенности, где 0 соответствует корневым элементам.</value>
		public int Level { get; set; }

		/// <summary>
		/// Получает или задает ссылку на родительский узел-обертку.
		/// </summary>
		/// <value>Экземпляр <see cref="TreeEntityWrapper{TEntity}"/> родительского узла, либо <see langword="null"/>, если это корень.</value>
		public TreeEntityWrapper<TEntity>? Parent { get; set; }

		/// <summary>
		/// Получает или задает коллекцию прямых дочерних узлов-оберток.
		/// </summary>
		/// <value>Перечисление дочерних объектов текущего узла, либо <see langword="null"/>, если дочерние элементы отсутствуют.</value>
		public IEnumerable<TreeEntityWrapper<TEntity>>? Children { get; set; }

		/// <summary>
		/// Получает или задает ссылку на оригинальный экземпляр доменной сущности.
		/// </summary>
		/// <value>Объект исходной сущности типа <typeparamref name="TEntity"/>.</value>
		public TEntity? Value { get; set; }
	}



	/// <summary>
	/// Вспомогательный класс для построения, индексации и линеаризации иерархических
	/// древовидных структур из плоских коллекций объектов.
	/// </summary>
	/// <typeparam name="TEntity">Тип доменной сущности, реализующей интерфейс <see cref="ITreeEntity"/>.</typeparam>
	public class TreeEntityHelper<TEntity>
		where TEntity : class, ITreeEntity
	{

		private readonly IEnumerable<TEntity> _source;
		private readonly Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> _funcOrder;

		private readonly List<TreeEntityWrapper<TEntity>> _topItems = [];
		private readonly List<TreeEntityWrapper<TEntity>> _allItems = [];


		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="TreeEntityHelper{TEntity}"/>
		/// и запускает рекурсивное построение иерархического дерева.
		/// </summary>
		/// <param name="source">Исходная плоская коллекция доменных сущностей.</param>
		/// <param name="currentId">
		/// Опциональный идентификатор элемента, который (вместе со всеми своими дочерними поддеревьями) 
		/// будет исключен из процесса построения (используется для предотвращения циклических ссылок при редактировании).
		/// </param>
		/// <param name="funcOrder">Делегат, задающий правила сортировки элементов на каждом уровне дерева.</param>
		public TreeEntityHelper(
			IEnumerable<TEntity> source,
			int? currentId,
			Func<IEnumerable<TEntity>, IOrderedEnumerable<TEntity>> funcOrder)
		{
			_source = source;
			_funcOrder = funcOrder;
			_scanTable(null, 0, currentId);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает коллекцию корневых (наивысших) элементов построенного дерева.
		/// </summary>
		/// <value>Перечисление объектов-оберток <see cref="TreeEntityWrapper{TEntity}"/>, не имеющих родителя.</value>
		public IEnumerable<TreeEntityWrapper<TEntity>> TopItems
			=> _topItems;


		/// <summary>
		/// Возвращает плоский линеаризованный список всех элементов дерева в порядке их обхода.
		/// </summary>
		/// <value>Плоская последовательность элементов, упорядоченная методом рекурсивного обхода дерева сверху вниз (Pre-order traversal).</value>
		public IEnumerable<TreeEntityWrapper<TEntity>> AllItems
			=> _allItems;


		/* privates */


		private void _scanTable(
			TreeEntityWrapper<TEntity>? parent,
			int level,
			int? currentId)
		{
			var items1 = _funcOrder(
				_source.Where(x => x.ParentPtr == parent?.Value?.Id && x.Id != currentId));
			var children1 = new List<TreeEntityWrapper<TEntity>>();
			foreach (var item1 in items1)
			{
				var item2 = new TreeEntityWrapper<TEntity>
				{
					Id = item1.Id,
					ParentPtr = item1.ParentPtr,
					Parent = parent,
					Order = item1.Order,
					Level = level,
					Value = item1
				};
				if (parent == null)
					_topItems.Add(item2);
				else
					children1.Add(item2);

				_allItems.Add(item2);
				_scanTable(item2, level + 1, currentId);
			}
			if (children1.Count > 0)
				parent?.Children = children1;
		}
	}

}
