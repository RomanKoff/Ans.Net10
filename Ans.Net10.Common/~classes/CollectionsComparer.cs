// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Компаратор для детального сопоставления, сравнения и синхронизации двух коллекций объектов по уникальному ключевому полю.
	/// Разделяет элементы на добавленные, удаленные, актуальные и измененные на основе пользовательского предиката различий.
	/// </summary>
	/// <typeparam name="TCurrent">Тип элементов в исходной (текущей) коллекции.</typeparam>
	/// <typeparam name="TNewest">Тип элементов в целевой (новой) коллекции.</typeparam>
	/// <typeparam name="TKey">Тип уникального ключа, по которому сопоставляются элементы. Не должен быть равен <see langword="null"/>.</typeparam>
	public class CollectionsComparer<TCurrent, TNewest, TKey>
		where TKey : notnull
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="CollectionsComparer{TCurrent, TNewest, TKey}"/> и сразу выполняет сопоставление и дифференциацию коллекций.
		/// </summary>
		/// <remarks>
		/// Если параметры <paramref name="current"/> или <paramref name="newest"/> равны <see langword="null"/>, они автоматически инициализируются как пустые наборы.
		/// </remarks>
		/// <param name="current">Исходная (текущая) коллекция элементов или <see langword="null"/>.</param>
		/// <param name="newest">Целевая (новая) коллекция элементов или <see langword="null"/>.</param>
		/// <param name="currentKeySelector">Делегат функции извлечения уникального ключа типа <typeparamref name="TKey"/> из элемента текущей коллекции.</param>
		/// <param name="newestKeySelector">Делегат функции извлечения уникального ключа типа <typeparamref name="TKey"/> из элемента новой коллекции.</param>
		/// <param name="funcDataDiff">Делегат функции проверки изменений. Возвращает <see langword="true"/>, если у объектов с одинаковыми ключами различаются внутренние данные и элемент требует обновления.</param>
		public CollectionsComparer(
			IEnumerable<TCurrent>? current,
			IEnumerable<TNewest>? newest,
			Func<TCurrent, TKey> currentKeySelector,
			Func<TNewest, TKey> newestKeySelector,
			Func<TCurrent, TNewest, bool> funcDataDiff)
		{
			var dictCurrents1 = (current ?? []).ToDictionary(currentKeySelector);
			var dictNewests1 = (newest ?? []).ToDictionary(newestKeySelector);
			CurrentCount = dictCurrents1.Count;
			NewestCount = dictNewests1.Count;
			var addeds1 = new List<TNewest>();
			var deleteds1 = new List<TCurrent>();
			var actualCurrents1 = new List<TCurrent>();
			var actualNewests1 = new List<TNewest>();
			var changeds1 = new List<(TCurrent Current, TNewest Newest)>();
			foreach (var currentPair1 in dictCurrents1)
				if (dictNewests1.TryGetValue(currentPair1.Key, out var newestItem1))
				{
					actualCurrents1.Add(currentPair1.Value);
					actualNewests1.Add(newestItem1);
					if (funcDataDiff(currentPair1.Value, newestItem1))
						changeds1.Add((currentPair1.Value, newestItem1));
				}
				else
					deleteds1.Add(currentPair1.Value);
			foreach (var newestPair1 in dictNewests1)
				if (!dictCurrents1.ContainsKey(newestPair1.Key))
					addeds1.Add(newestPair1.Value);
			Added = addeds1;
			Deleted = deleteds1;
			ActualCurrent = actualCurrents1;
			ActualNewest = actualNewests1;
			Changed = changeds1;
			AddedCount = addeds1.Count;
			DeletedCount = deleteds1.Count;
			ActualCount = actualCurrents1.Count;
			ChangedCount = changeds1.Count;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает общее количество элементов в исходной (текущей) коллекции.
		/// </summary>
		/// <value>Целочисленное количество элементов.</value>
		public int CurrentCount { get; }


		/// <summary>
		/// Возвращает общее количество элементов в целевой (новой) коллекции.
		/// </summary>
		/// <value>Целочисленное количество элементов.</value>
		public int NewestCount { get; }


		/// <summary>
		/// Возвращает коллекцию добавленных элементов, которые появились в новой коллекции, но полностью отсутствовали в текущей.
		/// </summary>
		/// <value>Коллекция <see cref="IReadOnlyCollection{TNewest}"/> добавленных элементов.</value>
		public IReadOnlyCollection<TNewest> Added { get; }


		/// <summary>
		/// Возвращает общее количество добавленных элементов.
		/// </summary>
		/// <value>Количество элементов в коллекции <see cref="Added"/>.</value>
		public int AddedCount { get; }


		/// <summary>
		/// Возвращает признак наличия добавленных элементов.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если добавлено больше 0 элементов; в противном случае — <see langword="false"/>.</value>
		public bool HasAdded => AddedCount > 0;


		/// <summary>
		/// Возвращает коллекцию удаленных элементов, которые присутствовали в текущей коллекции, но отсутствуют в новой.
		/// </summary>
		/// <value>Коллекция <see cref="IReadOnlyCollection{TCurrent}"/> удаленных элементов.</value>
		public IReadOnlyCollection<TCurrent> Deleted { get; }


		/// <summary>
		/// Возвращает общее количество удаленных элементов.
		/// </summary>
		/// <value>Количество элементов в коллекции <see cref="Deleted"/>.</value>
		public int DeletedCount { get; }


		/// <summary>
		/// Возвращает признак наличия удаленных элементов.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если удалено больше 0 элементов; в противном случае — <see langword="false"/>.</value>
		public bool HasDeleted
			=> DeletedCount > 0;


		/// <summary>
		/// Возвращает элементы из текущей коллекции, которые сохранили свое присутствие (по совпадению ключа) в новой коллекции.
		/// </summary>
		/// <value>Коллекция элементов старого типа, оставшихся актуальными.</value>
		public IReadOnlyCollection<TCurrent> ActualCurrent { get; }


		/// <summary>
		/// Возвращает элементы из новой коллекции, которые уже существовали (по совпадению ключа) в текущей коллекции.
		/// </summary>
		/// <value>Коллекция элементов нового типа, сопоставленных со старыми.</value>
		public IReadOnlyCollection<TNewest> ActualNewest { get; }


		/// <summary>
		/// Возвращает общее количество актуальных элементов, у которых совпали ключи в обеих коллекциях.
		/// </summary>
		/// <value>Целочисленное количество сопоставленных элементов.</value>
		public int ActualCount { get; }


		/// <summary>
		/// Возвращает признак наличия актуальных элементов.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сопоставлен хотя бы один элемент; в противном случае — <see langword="false"/>.</value>
		public bool HasActual
			=> ActualCount > 0;


		/// <summary>
		/// Возвращает коллекцию именованных пар элементов, у которых совпадают уникальные ключи, но внутренние данные различаются по результатам проверки делегатом.
		/// </summary>
		/// <value>
		/// Коллекция кортежей, где <c>Current</c> — ссылка на старое состояние объекта, а <c>Newest</c> — ссылка на новое состояние объекта.
		/// </value>
		public IReadOnlyCollection<(TCurrent Current, TNewest Newest)> Changed { get; }


		/// <summary>
		/// Возвращает общее количество измененных элементов.
		/// </summary>
		/// <value>Количество элементов в коллекции <see cref="Changed"/>.</value>
		public int ChangedCount { get; }


		/// <summary>
		/// Возвращает признак наличия измененных элементов, требующих обновления в хранилище данных.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если измененных элементов больше 0; в противном случае — <see langword="false"/>.</value>
		public bool HasChanged
			=> ChangedCount > 0;


		/* methods */


		/// <summary>
		/// Выводит диагностическую, отладочную и статистическую информацию о результатах сравнения коллекций в стандартный поток вывода консоли.
		/// </summary>
		public void TestDebug()
		{
			Console.WriteLine("[Ans.Net10.Common] CollectionsComparer.TestDebug()");
			Console.WriteLine($"   Current: {CurrentCount}");
			Console.WriteLine($"   Newest: {NewestCount}");
			Console.WriteLine($"   Added: {AddedCount}");
			Console.WriteLine($"   Deleted: {DeletedCount}");
			Console.WriteLine($"   Actual: {ActualCount}");
			Console.WriteLine($"   Changed: {ChangedCount}");
		}

	}

}
