// rev 2026-09-25

using System.Resources;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Перечень режимов визуального отображения элементов реестра в пользовательском интерфейсе.
	/// </summary>
	public enum RegistryModeEnum
	{
		/// <summary>
		/// Автоматический выбор режима на основе анализа максимальной ширины и количества элементов.
		/// </summary>
		Auto,

		/// <summary>
		/// Отображение в виде группы элементов ввода (радиокнопки для одиночного или чекбоксы для множественного выбора).
		/// </summary>
		Inputs,

		/// <summary>
		/// Отображение в виде классического компактного выпадающего списка выбора (<c>&lt;select&gt;</c> / Dropdown).
		/// </summary>
		Select
	}



	/// <summary>
	/// Предоставляет структурированные данные для событий, связанных с операциями над одиночным элементом реестра.
	/// </summary>
	/// <param name="item">Связанный с событием экземпляр элемента реестра.</param>
	public class RegistryItemEventArgs(
		RegistryItem item)
		: EventArgs
	{
		/// <summary>
		/// Возвращает связанный с текущим событием элемент реестра.
		/// </summary>
		/// <value>Объект класса <see cref="RegistryItem"/>.</value>
		public RegistryItem Item
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		} = item;
	}



	/// <summary>
	/// Делегат для обработки событий, оперирующих элементами реестра.
	/// </summary>
	/// <param name="sender">Источник (объект), инициировавший данное событие.</param>
	/// <param name="e">Аргументы события, содержащие целевой элемент реестра <see cref="RegistryItem"/>.</param>
	public delegate void RegistryItemEventHandler(
		object sender,
		RegistryItemEventArgs e);



	/// <summary>
	/// Представляет специализированный реактивный список элементов реестра с поддержкой 
	/// событийной модели, каскадной локализации, автофильтрации и иерархических операций.
	/// </summary>
	public class RegistryList
	{

		private readonly List<RegistryItem> _items = [];


		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="RegistryList"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryList()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RegistryList"/> на основе существующей коллекции элементов.
		/// </summary>
		/// <remarks>
		/// Элементы исходной коллекции, имеющие значение <see langword="null"/>, автоматически отсекаются и не добавляются в список.
		/// </remarks>
		/// <param name="items">Исходная коллекция элементов <see cref="RegistryItem"/> для наполнения списка.</param>
		public RegistryList(
			IEnumerable<RegistryItem> items)
			: this()
		{
			if (items != null)
				_items.AddRange(items.Where(x => x != null));
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RegistryList"/> на основе переданного массива элементов.
		/// </summary>
		/// <param name="items">Список аргументов (массив) готовых объектов <see cref="RegistryItem"/>.</param>
		public RegistryList(
			params RegistryItem[] items)
			: this(items?.AsEnumerable()!)
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RegistryList"/>, десериализуя его из единой текстовой строки с разделителями.
		/// </summary>
		/// <remarks>
		/// Строка разбирается по разделителю точек с запятой с учетом деэкранирования последовательностей <c>\;</c>.
		/// </remarks>
		/// <param name="serialization">Строка сериализации реестра в виде <c>"item1;item2;..."</c>.</param>
		public RegistryList(
			string serialization)
			: this()
		{
			FillFromString(serialization);
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="RegistryList"/> на основе массива предварительно сериализованных строк.
		/// </summary>
		/// <param name="items">Массив строк, каждая из которых представляет отдельный сериализованный элемент в формате реестра.</param>
		public RegistryList(
			params string[] items)
			: this()
		{
			FillFromString(items);
		}


		/* events */


		/// <summary>
		/// Наступает непосредственно после успешного добавления или вставки нового элемента в текущий список реестра.
		/// </summary>
		public event RegistryItemEventHandler? AddedItem;


		/* readonly properties */


		/// <summary>
		/// Возвращает перечисляемую коллекцию всех элементов реестра в текущем списке.
		/// </summary>
		/// <value>Интерфейс <see cref="IEnumerable{RegistryItem}"/> для последовательного обхода записей.</value>
		public IEnumerable<RegistryItem> Items
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items;
		}


		/// <summary>
		/// Возвращает значение, указывающее, содержит ли текущий список реестра хотя бы один элемент.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если список не пуст; в противном случае — <see langword="false"/>.</value>
		public bool HasItems
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _items.Count > 0;
		}


		/* methods */


		/// <summary>
		/// Выполняет десериализацию и наполнение реестра из перечисления строк с обработкой системных маркеров <c>"!"</c> и <c>"*"</c>.
		/// </summary>
		/// <remarks>
		/// Метод очищает список перед началом работы. Маркер <c>"!"</c> однократно добавляет системный пустой элемент, а маркер <c>"*"</c> — элемент "Все значения".
		/// </remarks>
		/// <param name="items">Коллекция строк элементов для разбора.</param>
		public void FillFromString(
			IEnumerable<string> items)
		{
			_items.Clear();
			if (items == null)
				return;
			bool hasNull1 = false;
			bool hasAll1 = false;
			foreach (var item1 in items)
			{
				if (string.IsNullOrEmpty(item1))
					continue;
				if (item1 == "!")
				{
					if (!hasNull1)
					{
						AddNullItem();
						hasNull1 = true;
					}
				}
				else if (item1 == "*")
				{
					if (!hasAll1)
					{
						AddAllItems();
						hasAll1 = true;
					}
				}
				else
				{
					Add(new RegistryItem(item1));
				}
			}
		}


		/// <summary>
		/// Выполняет десериализацию и наполнение реестра из массива строк.
		/// </summary>
		/// <param name="items">Массив строк элементов для разбора. При передаче <see langword="null"/> список просто очищается.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FillFromString(
			params string[] items)
		{
			if (items == null)
			{
				_items.Clear();
				return;
			}
			FillFromString(items.AsEnumerable());
		}


		/// <summary>
		/// Выполняет десериализацию реестра из единой текстовой строки, разделяя элементы по символу <c>';'</c> с защитой экранированных разделителей.
		/// </summary>
		/// <param name="serialization">Полная строка сериализации реестра.</param>
		public void FillFromString(
			string serialization)
		{
			_items.Clear();
			if (string.IsNullOrEmpty(serialization))
				return;
			var masked1 = serialization.Replace("\\;", RegistryItem.MASK_semicolon);
			var tokens1 = masked1.Split(';', StringSplitOptions.RemoveEmptyEntries);
			var unmaskedTokens1 = tokens1.Select(t => t.Replace(RegistryItem.MASK_semicolon, "\\;"));
			FillFromString(unmaskedTokens1);
		}


		/// <summary>
		/// Безопасно вызывает событие <see cref="AddedItem"/>, извещая подписчиков о добавлении элемента.
		/// </summary>
		/// <param name="e">Заполненные аргументы события <see cref="RegistryItemEventArgs"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void OnAddedItem(
			RegistryItemEventArgs e)
		{
			AddedItem?.Invoke(this, e);
		}


		/// <summary>
		/// Добавляет готовый элемент в конец текущего списка реестра и генерирует реактивное событие добавления.
		/// </summary>
		/// <param name="item">Добавляемый объект элемента реестра. Если равен <see langword="null"/>, операция игнорируется.</param>
		public void Add(
			RegistryItem item)
		{
			if (item == null)
				return;
			_items.Add(item);
			OnAddedItem(new RegistryItemEventArgs(item));
		}


		/// <summary>
		/// Вставляет элемент в список реестра по строго указанному порядковому индексу и генерирует событие добавления.
		/// </summary>
		/// <param name="index">Целочисленный индекс позиции вставки (начиная с 0).</param>
		/// <param name="item">Вставляемый объект элемента реестра.</param>
		public void Insert(
			int index,
			RegistryItem item)
		{
			if (item == null)
				return;
			_items.Insert(index, item);
			OnAddedItem(new RegistryItemEventArgs(item));
		}


		/// <summary>
		/// Создает, инициализирует и добавляет новый элемент в конец списка реестра по заданным параметрам метаданных.
		/// </summary>
		/// <param name="key">Уникальный ключ создаваемого элемента.</param>
		/// <param name="value">Текстовое значение контента элемента.</param>
		/// <param name="level">Уровень вложенности в иерархии (от 0 до 9).</param>
		/// <param name="isLabel">Флаг, определяющий, является ли элемент чисто текстовым заголовком.</param>
		public void Add(
			string key,
			string value,
			int level,
			bool isLabel)
		{
			var item1 = new RegistryItem(key, value, level, isLabel);
			Add(item1);
		}


		/// <summary>
		/// Быстро добавляет в реестр элемент-метку, служащую визуальным текстовым заголовком для последующих подразделов.
		/// </summary>
		/// <param name="title">Текст отображаемого заголовка метки.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddLabel(
			string title)
		{
			Add(string.Empty, title, 0, true);
		}


		/// <summary>
		/// Вставляет на самую первую (нулевую) позицию списка специальный системный элемент "Нулевое/пустое значение".
		/// </summary>
		/// <remarks>
		/// Текст элемента извлекается из глобальных ресурсов локализации библиотеки (<c>Resources.Common.Text_EmptyItem</c>).
		/// </remarks>
		/// <param name="key">Опциональный кастомный ключ элемента. Если не задан, подставляется <see cref="string.Empty"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddNullItem(
			string? key = null)
		{
			Insert(0, new RegistryItem(
				key ?? string.Empty,
				Resources.Common.Text_EmptyItem,
				0, false));
		}


		/// <summary>
		/// Вставляет на самую первую (нулевую) позицию списка специальный системный элемент "Все значения".
		/// </summary>
		/// <remarks>
		/// Текст элемента извлекается из глобальных ресурсов локализации библиотеки (<c>Resources.Common.Text_AllItems</c>).
		/// </remarks>
		/// <param name="key">Опциональный кастомный ключ элемента. Если не задан, подставляется <see cref="string.Empty"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddAllItems(
			string? key = null)
		{
			Insert(0, new RegistryItem(
				key ?? string.Empty,
				Resources.Common.Text_AllItems,
				0, false));
		}


		/// <summary>
		/// Выполняет текстовую локализацию значений элементов текущего реестра на основе сопоставления ключей из другой строки сериализации.
		/// </summary>
		/// <param name="source">Строка сериализации словаря-источника локализации (формата <c>"key1=val1;key2=val2"</c>).</param>
		public void Localization(
			string source)
		{
			if (string.IsNullOrEmpty(source))
				return;
			var reg1 = new RegistryList(string.Empty, source);
			if (reg1.HasItems)
				foreach (var item1 in _items)
				{
					var s1 = reg1.GetValue(item1.Key);
					if (!string.IsNullOrEmpty(s1))
						item1.Value = s1;
				}
		}


		/// <summary>
		/// Извлекает каскадные метаданные CRUD-интерфейса из ресурсов и применяет их для автоматической локализации элементов реестра.
		/// </summary>
		/// <param name="key">Ключ свойства или поля в файлах ресурсов.</param>
		/// <param name="resources">Массив задействованных менеджеров ресурсов <see cref="ResourceManager"/>.</param>
		public void Localization(
			string key,
			params ResourceManager[] resources)
		{
			var helper1 = new ResourcesHelper(resources);
			var face1 = helper1.GetCrudFace(key);
			if (face1 != null)
				Localization(face1.ToString());
		}


		/* functions */


		/// <summary>
		/// Сериализует весь текущий список реестра в единую текстовую строку, объединяя элементы через точку с запятой.
		/// </summary>
		/// <returns>Итоговая строка сериализованных элементов реестра.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return string.Join(";", _items);
		}


		/// <summary>
		/// Возвращает элемент реестра по его строковому ключу с использованием строгого побайтового сравнения.
		/// </summary>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <returns>Найденный объект <see cref="RegistryItem"/> или <see langword="null"/>, если элемент с таким ключом отсутствует.</returns>
		public RegistryItem? GetItem(
			string? key)
		{
			if (key == null)
				return null;
			return _items.FirstOrDefault(
				x => x.Key.Equals(key, StringComparison.Ordinal));
		}


		/// <summary>
		/// Возвращает элемент реестра по его целочисленному ключу.
		/// </summary>
		/// <param name="key">Целочисленный идентификатор ключа.</param>
		/// <returns>Найденный объект или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryItem? GetItem(
			int key)
		{
			return GetItem(key.ToString());
		}


		/// <summary>
		/// Возвращает элемент реестра по его целочисленному nullable-ключу.
		/// </summary>
		/// <param name="key">Идентификатор ключа, допускающий значение <see langword="null"/>.</param>
		/// <returns>Найденный объект или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryItem? GetItem(
			int? key)
		{
			return GetItem(key?.ToString());
		}


		/// <summary>
		/// Возвращает строковый ключ самого первого элемента, чье текстовое значение контента совпадает с переданным.
		/// </summary>
		/// <param name="value">Текстовое значение для поиска.</param>
		/// <returns>Строковый ключ сопоставленного элемента, либо <see langword="null"/>, если совпадений не найдено.</returns>
		public string? GetKey(
			string value)
		{
			if (value == null)
				return null;
			var item1 = _items.FirstOrDefault(
				x => x.Value.Equals(value, StringComparison.Ordinal));
			return item1?.Key;
		}


		/// <summary>
		/// Возвращает ключ элемента по его текстовому значению с автоматической регистрацией и генерацией ключа при его отсутствии.
		/// </summary>
		/// <param name="value">Текстовое значение элемента.</param>
		/// <param name="newKey">Опциональный новый ключ для регистрации. Если равен <see langword="null"/> — ключом становится текущий размер списка.</param>
		/// <returns>Финальный строковый ключ, под которым зарегистрировано значение в реестре.</returns>
		public string GetKeyForValue(
			string value,
			string? newKey = null)
		{
			var key1 = GetKey(value);
			if (key1 != null)
				return key1;
			var actualKey1 = newKey ?? _items.Count.ToString();
			Add(actualKey1, value, 0, false);
			return actualKey1;
		}


		/// <summary>
		/// Безопасно возвращает строковое значение контента элемента по его строковому ключу.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <returns>Значение контента или <see langword="null"/>, если ключ отсутствует в списке.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetValue(
			string key)
		{
			var item1 = GetItem(key);
			return item1?.Value;
		}


		/// <summary>
		/// Безопасно возвращает строковое значение контента элемента по его целочисленному ключу.
		/// </summary>
		/// <param name="key">Целочисленный ключ элемента.</param>
		/// <returns>Значение контента или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetValue(
			int key)
		{
			return GetValue(key.ToString());
		}


		/// <summary>
		/// Безопасно возвращает строковое значение контента элемента по его целочисленному nullable-ключу.
		/// </summary>
		/// <param name="key">Идентификатор ключа, допускающий значение <see langword="null"/>.</param>
		/// <returns>Значение контента или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string? GetValue(
			int? key)
		{
			return GetValue(key?.ToString() ?? string.Empty);
		}


		/// <summary>
		/// Возвращает значение по ключу, а в случае промаха возвращает сам ключ, оформленный по шаблону декоратора пропущенных данных.
		/// </summary>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <returns>Значение элемента, либо декорированная строка вида <c>"–{key}–"</c>.</returns>
		public string GetValueOrKey(
			string key)
		{
			var item1 = GetItem(key);
			return item1?.Value ?? key.Make("–{0}–");
		}


		/// <summary>
		/// Возвращает значение по целочисленному ключу, а в случае промаха возвращает числовой ключ с декоратором пропущенных данных.
		/// </summary>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <returns>Значение или декорированный ключ.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetValueOrKey(
			int key)
		{
			return GetValueOrKey(key.ToString());
		}


		/// <summary>
		/// Возвращает уровень вложенности элемента по его строковому ключу.
		/// </summary>
		/// <param name="key">Строковый ключ элемента.</param>
		/// <returns>Уровень вложенности в диапазоне от 0 до 9, либо <c>0</c>, если элемент не зарегистрирован.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetLevel(
			string key)
		{
			var item1 = GetItem(key);
			return item1?.Level ?? 0;
		}


		/// <summary>
		/// Возвращает уровень вложенности элемента по его целочисленному ключу.
		/// </summary>
		/// <param name="key">Целочисленный ключ элемента.</param>
		/// <returns>Уровень вложенности в диапазоне от 0 до 9, либо <c>0</c>, если элемент не зарегистрирован.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetLevel(
			int key)
		{
			return GetLevel(key.ToString());
		}


		/// <summary>
		/// Возвращает уровень вложенности элемента по его целочисленному nullable-ключу.
		/// </summary>
		/// <param name="key">Целочисленный ключ элемента, допускающий значение <see langword="null"/>.</param>
		/// <returns>Уровень вложенности в диапазоне от 0 до 9, либо <c>0</c>, если элемент не зарегистрирован.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetLevel(
			int? key)
		{
			return !key.HasValue
				? 0 : GetLevel(key.Value);
		}


		/// <summary>
		/// Возвращает значение первого встреченного элемента, чей ключ содержит в себе указанную подстроку (поиск по частичному совпадению).
		/// </summary>
		/// <param name="inclusion">Искомая подстрока, входящая в состав ключа.</param>
		/// <returns>Значение элемента или <see langword="null"/>, если совпадений нет.</returns>
		public string? GetValueInclusion(
			string inclusion)
		{
			if (string.IsNullOrEmpty(inclusion))
				return null;
			var item1 = _items.FirstOrDefault(
				x => x.Key.Contains(inclusion, StringComparison.Ordinal));
			return item1?.Value;
		}


		/// <summary>
		/// Возвращает значение первого встреченного элемента, чей ключ целиком входит в состав переданного текстового контейнера.
		/// </summary>
		/// <param name="expansion">Большая строка-контейнер, в которой проверяется наличие зарегистрированных ключей.</param>
		/// <returns>Значение элемента или <see langword="null"/>.</returns>
		public string? GetValueExpansion(
			string expansion)
		{
			if (string.IsNullOrEmpty(expansion))
				return null;
			var item1 = _items.FirstOrDefault(
				x => expansion.Contains(x.Key, StringComparison.Ordinal));
			return item1?.Value;
		}


		/// <summary>
		/// Рассчитывает и возвращает максимальную ширину текстовых значений элементов реестра в пикселях/символах с учетом фиксированного базового отступа.
		/// </summary>
		/// <returns>Максимальная длина строкового значения в списке плюс фиксированная константа базового отступа (<c>9</c>).</returns>
		public int GetMaxWidth()
		{
			const int basePadding = 9;
			if (_items.Count == 0)
				return basePadding;
			int maxLen1 = 1;
			for (int i1 = 0; i1 < _items.Count; i1++)
			{
				int currentLen1 = _items[i1].Value?.Length ?? 1;
				if (currentLen1 > maxLen1)
					maxLen1 = currentLen1;
			}
			return maxLen1 + basePadding;
		}


		/// <summary>
		/// Возвращает интеллектуально рекомендуемый режим отображения реестра в веб-интерфейсе на основе корреляции его расчетной ширины и плотности элементов.
		/// </summary>
		/// <returns>Одно из значений перечисления <see cref="RegistryModeEnum"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RegistryModeEnum GetProposeMode()
		{
			return GetProposeWidth() switch
			{
				WidthsEnum.Full => _items.Count < 10
					? RegistryModeEnum.Inputs
					: RegistryModeEnum.Select,
				WidthsEnum.Large => _items.Count < 20
					? RegistryModeEnum.Inputs
					: RegistryModeEnum.Select,
				WidthsEnum.Medium => _items.Count < 30
					? RegistryModeEnum.Inputs
					: RegistryModeEnum.Select,
				_ => _items.Count < 40
					? RegistryModeEnum.Inputs
					: RegistryModeEnum.Select
			};
		}


		/// <summary>
		/// Возвращает рекомендуемую абстрактную ширину элемента ввода для интерфейса на основе переданного числового значения максимального размера.
		/// </summary>
		/// <param name="width">Числовое значение максимальной ширины поля.</param>
		/// <returns>Одно из значений перечисления <see cref="WidthsEnum"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static WidthsEnum GetProposeWidth(
			int width)
		{
			return width switch
			{
				< 11 => WidthsEnum.Small,
				< 41 => WidthsEnum.Medium,
				< 101 => WidthsEnum.Large,
				_ => WidthsEnum.Full,
			};
		}


		/// <summary>
		/// Возвращает рекомендуемую ширину элемента ввода в интерфейсе на основе автоматического анализа максимальной длины значений текущего списка реестра.
		/// </summary>
		/// <returns>Одно из значений перечисления <see cref="WidthsEnum"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WidthsEnum GetProposeWidth()
		{
			return GetProposeWidth(GetMaxWidth());
		}

	}

}
