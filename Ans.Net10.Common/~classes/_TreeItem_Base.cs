// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Интерфейс для элементов, поддерживающих древовидную (иерархическую) структуру.
	/// </summary>
	public interface ITreeItem
	{
		/* properties */

		/// <summary>
		/// Получает или задает родительский элемент текущего узла.
		/// </summary>
		/// <value>
		/// Объект, реализующий <see cref="ITreeItem"/> и являющийся родителем текущего узла, 
		/// или <see langword="null"/>, если узел является корневым.
		/// </value>
		ITreeItem? Parent { get; set; }

		/* readonly properties */

		/// <summary>
		/// Получает ленивое перечисление всех вышестоящих родительских элементов по цепочке вверх до корня дерева.
		/// </summary>
		/// <value>
		/// Последовательность <see cref="IEnumerable{ITreeItem}"/>, содержащая родительские элементы в порядке удаления от текущего узла.
		/// </value>
		IEnumerable<ITreeItem> Parents { get; }

		/// <summary>
		/// Получает перечисление прямых дочерних элементов текущего узла.
		/// </summary>
		/// <value>
		/// Последовательность <see cref="IEnumerable{ITreeItem}"/>, представляющая дочерние узлы первого уровня вложенности.
		/// </value>
		IEnumerable<ITreeItem> Children { get; }

		/// <summary>
		/// Возвращает признак наличия родительского элемента у текущего узла.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если текущий узел не является корневым (свойство <see cref="Parent"/> не равно <see langword="null"/>); в противном случае — <see langword="false"/>.
		/// </value>
		bool HasParent { get; }

		/// <summary>
		/// Возвращает признак наличия дочерних элементов у текущего узла.
		/// </summary>
		/// <value>
		/// Значение <see langword="true"/>, если узел содержит хотя бы один дочерний элемент; в противном случае — <see langword="false"/>.
		/// </value>
		bool HasChildren { get; }

		/* methods */

		/// <summary>
		/// Добавляет дочерний узел в конец списка текущего элемента.
		/// </summary>
		/// <param name="item">Добавляемый дочерний элемент, реализующий <see cref="ITreeItem"/>.</param>
		void AppendChild(ITreeItem item);

		/// <summary>
		/// Массово добавляет дочерние узлы в конец списка текущего элемента.
		/// </summary>
		/// <param name="items">Массив добавляемых дочерних элементов. Элементы со значением <see langword="null"/> автоматически игнорируются.</param>
		void AppendChildren(params ITreeItem[] items);

		/* functions */

		/// <summary>
		/// Выполняет рекурсивный поиск первого дочернего элемента заданного типа, удовлетворяющего условию предиката.
		/// </summary>
		/// <typeparam name="T">Целевой ссылочный тип искомого объекта, наследуемый от <see cref="ITreeItem"/>.</typeparam>
		/// <param name="func">Делегат функции-предиката для проверки условий соответствия элемента.</param>
		/// <returns>
		/// Найденный объект типа <typeparamref name="T"/> или <see langword="null"/>, если элемент не найден или предикат вернул <see langword="false"/> для всех узлов поддерева.
		/// </returns>
		T? FindItem<T>(Func<T, bool> func) where T : class;
	}



	/// <summary>
	/// Базовый класс элемента древовидной структуры с защитой от циклической зависимости.
	/// </summary>
	public class _TreeItem_Base
		: ITreeItem
	{

		private List<ITreeItem>? _children;


		/* properties */


		/// <inheritdoc />
		public ITreeItem? Parent { get; set; }


		/* readonly properties */


		/// <inheritdoc />
		public IEnumerable<ITreeItem> Parents
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _getParents();
		}


		/// <inheritdoc />
		public IEnumerable<ITreeItem> Children
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _children ?? Enumerable.Empty<ITreeItem>();
		}


		/// <inheritdoc />
		public bool HasChildren
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _children?.Count > 0;
		}


		/// <inheritdoc />
		public bool HasParent
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Parent != null;
		}


		/* methods */


		/// <inheritdoc />
		/// <remarks>
		/// Перед добавлением метод производит валидацию иерархии, проверяя, что объект не добавляет сам себя, 
		/// а также проверяет всю цепочку предков вверх, исключая возможность образования замкнутых циклов.
		/// </remarks>
		/// <param name="item">Добавляемый дочерний элемент. Если равен <see langword="null"/>, добавление не выполняется.</param>
		/// <exception cref="ArgumentException">
		/// Вызывается, если переданный объект <paramref name="item"/> совпадает по ссылке с текущим экземпляром (<c>item == this</c>).
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Вызывается, если при обходе предков обнаружено, что текущий узел или его родители уже содержат ссылку на <paramref name="item"/>, 
		/// что привело бы к циклической зависимости в структуре дерева.
		/// </exception>
		public void AppendChild(
			ITreeItem item)
		{
			if (item == null)
				return;
			if (item == this)
				throw new ArgumentException(
					"[Ans.Net10.Common] An object cannot be its own Child.");
			var temp1 = Parent;
			while (temp1 != null)
			{
				if (temp1 == item)
					throw new InvalidOperationException(
						"[Ans.Net10.Common] The detected loop: this object is already a Parent in the chain above, the object cannot be its own Child.");
				temp1 = temp1.Parent;
			}
			_children ??= [];
			_children.Add(item);
			item.Parent = this;
		}


		/// <inheritdoc />
		public void AppendChildren(
			params ITreeItem[] items)
		{
			if (items == null)
				return;
			for (int i1 = 0; i1 < items.Length; i1++)
				if (items[i1] != null)
					AppendChild(items[i1]);
		}


		/* functions */


		/// <inheritdoc />
		/// <remarks>
		/// Поиск осуществляется методом обхода в глубину (Depth-First Search, DFS) по всей вложенной структуре дочерних элементов.
		/// </remarks>
		public T? FindItem<T>(
			Func<T, bool> func)
			where T : class
		{
			if (func == null
				|| _children == null
				|| _children.Count == 0)
				return null;
			for (int i1 = 0; i1 < _children.Count; i1++)
			{
				var current1 = _children[i1];
				if (current1 is T typedItem1 && func(typedItem1))
					return typedItem1;
				var foundInside1 = current1.FindItem(func);
				if (foundInside1 != null)
					return foundInside1;
			}
			return null;
		}


		/* privates */


		private IEnumerable<ITreeItem> _getParents()
		{
			var current1 = Parent;
			while (current1 != null)
			{
				yield return current1;
				current1 = current1.Parent;
			}
		}

	}

}
