// rev 2026-09-25

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет отдельный атомарный элемент цепочки сортировки, связывающий имя свойства (колонки) и направление.
	/// </summary>
	public class OrderItem
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="OrderItem"/> на основе строкового определения.
		/// </summary>
		/// <remarks>
		/// Если строка <paramref name="order"/> начинается с символа дефиса <c>'-'</c>, направление сортировки устанавливается 
		/// как убывающее (<see cref="IsDescending"/> = <see langword="true"/>), а сам дефис отсекается от имени свойства.
		/// </remarks>
		/// <param name="order">Строковое имя свойства (колонки), опционально начинающееся с дефиса <c>'-'</c> для сортировки по убыванию (например, <c>"Name"</c> или <c>"-Age"</c>).</param>
		public OrderItem(
			string order)
		{
			if (!string.IsNullOrEmpty(order) && order[0] == '-')
			{
				IsDescending = true;
				Column = order[1..];
			}
			else
				Column = order ?? string.Empty;
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает очищенное системное имя свойства (колонки) объекта, по которому осуществляется сортировка.
		/// </summary>
		/// <value>Строковое значение имени свойства. Если переданный аргумент был равен <see langword="null"/>, возвращается <see cref="string.Empty"/>.</value>
		public string Column { get; }


		/// <summary>
		/// Возвращает признак того, что сортировка должна выполняться по убыванию (Descending).
		/// </summary>
		/// <value>Значение <see langword="true"/>, если сортировка выполняется по убыванию; в противном случае (по возрастанию) — <see langword="false"/>.</value>
		public bool IsDescending { get; }


		/* functions */


		/// <summary>
		/// Возвращает каноническое строковое представление текущего элемента сортировки.
		/// </summary>
		/// <remarks>
		/// При убывающем направлении к началу строки автоматически добавляется префикс дефиса с помощью метода расширения <c>Make("-")</c>.
		/// </remarks>
		/// <returns>Строка формата <c>"ИмяПоля"</c> для возрастания или <c>"-ИмяПоля"</c> для убывания.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return $"{IsDescending.Make("-")}{Column}";
		}

	}



	/// <summary>
	/// Строит, валидирует и инкапсулирует коллекцию элементов <see cref="OrderItem"/> для управления многокритериальным порядком сортировки данных.
	/// </summary>
	public class OrderBuilder
	{

		private readonly List<OrderItem> _items = [];
		private readonly OrderItem[] _itemsCache;



		/* consts */


		/// <summary>
		/// Коллекция стандартных символов-разделителей, используемых для парсинга перечня правил сортировки.
		/// </summary>
		/// <value>Массив символов, содержащий точку с запятой (<c>;</c>) и запятую (<c>,</c>).</value>
		public static readonly char[] SEP_ITEMS = [';', ','];



		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="OrderBuilder"/>, разбирая переданную строку с перечислением полей.
		/// </summary>
		/// <remarks>
		/// В качестве разделителей подсетей поддерживаются символы из поля <see cref="SEP_ITEMS"/>.
		/// Пустые сегменты между разделителями автоматически игнорируются.
		/// </remarks>
		/// <param name="order">Строка с перечислением полей и направлений сортировки (например, <c>"LastName,-Age,FirstName"</c>). Если строка пуста или равна <see langword="null"/>, коллекция останется пустой.</param>
		public OrderBuilder(
			string order)
		{
			if (!string.IsNullOrEmpty(order))
			{
				var a1 = order.Split(SEP_ITEMS, StringSplitOptions.RemoveEmptyEntries);
				foreach (var item1 in a1)
					_items.Add(new OrderItem(item1));
			}
			_itemsCache = [.. _items];
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает кэшированный фиксированный массив всех распознанных элементов многокритериальной сортировки.
		/// </summary>
		/// <value>Массив объектов <see cref="OrderItem"/>.</value>
		public OrderItem[] Items
			=> _itemsCache;


		/* functions */


		/// <summary>
		/// Возвращает строковое представление всей цепочки правил сортировок, где элементы объединены через запятую.
		/// </summary>
		/// <returns>Строка сериализованных правил сортировки (например, <c>"LastName,-Age"</c>). Если правила отсутствуют, возвращается <see cref="string.Empty"/>.</returns>
		public override string ToString()
		{
			return string.Join(",", _items.Select(x => x.ToString()));
		}


		/// <summary>
		/// Генерирует и возвращает форматированный псевдокод цепочки методов LINQ (<c>.OrderBy().ThenBy()</c>), соответствующий текущему набору правил.
		/// </summary>
		/// <remarks>
		/// Метод используется преимущественно для диагностических целей, трассировки SQL-генерации или логирования сложных динамических запросов.
		/// </remarks>
		/// <param name="offset">Строковый префикс отступа (например, пробелы или знаки табуляции) для красивого выравнивания многострочного кода.</param>
		/// <returns>Форматированная многострочная строка псевдокода LINQ с символами переноса строк, либо <see cref="string.Empty"/>, если коллекция элементов сортировки пуста.</returns>
		public string GetLinqCode(
			string offset)
		{
			if (_items.Count == 0)
				return string.Empty;
			var sb1 = new StringBuilder();
			var s1 = (_items[0].IsDescending)
				? $"\n{offset}.OrderByDescending(x => x.{_items[0].Column})"
				: $"\n{offset}.OrderBy(x => x.{_items[0].Column})";
			sb1.Append(s1);
			for (int i1 = 1; i1 < _items.Count; i1++)
			{
				var item1 = _items[i1];
				var s2 = (item1.IsDescending)
					? $"\n{offset}.ThenByDescending(x => x.{item1.Column})"
					: $"\n{offset}.ThenBy(x => x.{item1.Column})";
				sb1.Append(s2);
			}
			return sb1.ToString();
		}

	}

}