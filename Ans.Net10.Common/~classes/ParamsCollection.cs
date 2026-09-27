// rev 2026-09-25

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Специализированная коллекция параметров для формирования строки URL-запроса (Query String) 
	/// с автоматической встроенной фильтрацией дефолтных и пустых значений.
	/// </summary>
	public class ParamsCollection
	{

		private readonly Dictionary<string, string> _items;


		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="ParamsCollection"/> со словарем по умолчанию.
		/// </summary>
		public ParamsCollection()
		{
			_items = [];
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ParamsCollection"/> с заданной начальной емкостью и компаратором ключей.
		/// </summary>
		/// <param name="capacity">Начальная емкость внутреннего словаря параметров для оптимизации выделения памяти.</param>
		/// <param name="comparer">Реализация <see cref="IEqualityComparer{String}"/> для сравнения имен параметров (например, для обеспечения регистронезависимости).</param>
		public ParamsCollection(
			int capacity,
			IEqualityComparer<string> comparer)
		{
			_items = new Dictionary<string, string>(capacity, comparer);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает внутренний словарь, содержащий сырые строковые пары параметров.
		/// </summary>
		/// <value>Объект класса <see cref="Dictionary{String, String}"/>.</value>
		public Dictionary<string, string> Items
			=> _items;


		/* methods */


		/// <summary>
		/// Добавляет новый или принудительно обновляет существующий строковый параметр без применения фильтрации.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Строковое значение параметра запроса.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			string value)
		{
			_items[name] = value;
		}


		/// <summary>
		/// Добавляет параметр со строковым значением <c>"1"</c> только в том случае, если переданное логическое значение истинно.
		/// </summary>
		/// <remarks>
		/// Если параметр <paramref name="value"/> равен <see langword="false"/>, добавление или обновление записи не производится (параметр игнорируется).
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Логическое значение типа <see cref="bool"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			bool value)
		{
			if (value)
				Append(name, "1");
		}


		/// <summary>
		/// Добавляет целочисленный параметр в виде строки, если его значение не равно <c>0</c>.
		/// </summary>
		/// <remarks>
		/// Значение <c>0</c> считается дефолтным для Query String и автоматически отфильтровывается (не добавляется в словарь).
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">32-битное целое число со знаком типа <see cref="int"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			int value)
		{
			if (value != 0)
				Append(name, value.ToString());
		}


		/// <summary>
		/// Добавляет числовой параметр типа long в виде строки, если его значение не равно <c>0</c>.
		/// </summary>
		/// <remarks>
		/// Значение <c>0</c> автоматически отфильтровывается и полностью игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">64-битное целое число со знаком типа <see cref="long"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			long value)
		{
			if (value != 0)
				Append(name, value.ToString());
		}


		/// <summary>
		/// Добавляет числовой параметр типа double в виде строки, если его значение не равно <c>0.0</c>.
		/// </summary>
		/// <remarks>
		/// Значение <c>0</c> автоматически отфильтровывается и полностью игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Число с плавающей запятой двойной точности типа <see cref="double"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			double value)
		{
			if (value != 0)
				Append(name, value.ToString());
		}


		/// <summary>
		/// Добавляет числовой параметр типа float в виде строки, если его значение не равно <c>0.0f</c>.
		/// </summary>
		/// <remarks>
		/// Значение <c>0</c> автоматически отфильтровывается и полностью игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Число с плавающей запятой одинарной точности типа <see cref="float"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			float value)
		{
			if (value != 0)
				Append(name, value.ToString());
		}


		/// <summary>
		/// Добавляет числовой параметр типа decimal в виде строки, если его значение не равно <c>0.0m</c>.
		/// </summary>
		/// <remarks>
		/// Значение <c>0</c> автоматически отфильтровывается и полностью игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Десятичное число с высокой точностью типа <see cref="decimal"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			decimal value)
		{
			if (value != 0)
				Append(name, value.ToString());
		}


		/// <summary>
		/// Добавляет параметр даты и времени в универсальном формате сортируемой строки (исходя из маски <c>"u"</c>), если значение задано.
		/// </summary>
		/// <remarks>
		/// Пример результирующего значения в URL: <c>"2026-09-19 20:15:00Z"</c>. Если передан <see langword="null"/>, запись игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Значение структуры <see cref="DateTime"/>, допускающее <see langword="null"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			DateTime? value)
		{
			if (value != null)
				Append(name, value.Value.ToString("u"));
		}


		/// <summary>
		/// Добавляет параметр даты, отформатированный по стандарту ISO <c>"yyyy-MM-dd"</c>, если значение задано.
		/// </summary>
		/// <remarks>
		/// Пример результирующего значения в URL: <c>"2026-09-19"</c>. Если передан <see langword="null"/>, запись игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) haematology параметра URL.</param>
		/// <param name="value">Значение структуры <see cref="DateOnly"/>, допускающее <see langword="null"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			DateOnly? value)
		{
			if (value != null)
				Append(name, value.Value.ToString("yyyy-MM-dd"));
		}


		/// <summary>
		/// Добавляет параметр времени в 24-часовом строковом формате <c>"HH:mm:ss"</c>, если значение задано.
		/// </summary>
		/// <remarks>
		/// Пример результирующего значения в URL: <c>"14:30:00"</c>. Если передан <see langword="null"/>, запись игнорируется.
		/// </remarks>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Значение структуры <see cref="TimeOnly"/>, допускающее <see langword="null"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			TimeOnly? value)
		{
			if (value != null)
				Append(name, value.Value.ToString("HH:mm:ss"));
		}


		/* functions */


		/// <summary>
		/// Преобразует всю накопленную коллекцию параметров в валидную и безопасную строку HTTP-запроса (Query String), начинающуюся с символа префикса <c>'?'</c>.
		/// </summary>
		/// <returns>
		/// Готовая к конкатенации строка URL-запроса вида <c>"?param1=val1&amp;param2=val2"</c>. 
		/// Если коллекция пуста и не содержит элементов, возвращается <see cref="string.Empty"/>.
		/// </returns>
		public override string ToString()
		{
			if (_items.Count == 0)
				return string.Empty;
			var sb1 = new StringBuilder();
			sb1.Append('?');
			bool isFirst1 = true;
			foreach (var item1 in _items)
			{
				if (!isFirst1)
					sb1.Append('&');
				sb1.Append(item1.Key);
				sb1.Append('=');
				sb1.Append(item1.Value);
				isFirst1 = false;
			}
			return sb1.ToString();
		}

	}

}
