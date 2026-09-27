// rev 2026-09-25

using System.Runtime.CompilerServices;
using System.Text;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Абстрактный прототип класса для словарей, поддерживающих кастомную текстовую сериализацию
	/// с автоматическим экранированием разделителей ключей (<c>=</c>) и пар (<c>;</c>).
	/// </summary>
	/// <typeparam name="TKey">Тип уникального ключа словаря. Не должен быть равен <see langword="null"/>.</typeparam>
	/// <typeparam name="TValue">Тип значения, ассоциированного с ключом в словаре.</typeparam>
	public abstract class _Dict_Proto<TKey, TValue>
		: Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
		where TKey : notnull
	{

		/* abstracts */


		/// <summary>
		/// Преобразует строковое представление ключа в целевой тип <typeparamref name="TKey"/>.
		/// </summary>
		/// <param name="key">Строковое представление ключа, извлеченное при десериализации.</param>
		/// <returns>Экземпляр типа <typeparamref name="TKey"/>, представляющий ключ.</returns>
		public abstract TKey StringToKey(string key);


		/// <summary>
		/// Преобразует строковое представление значения в целевой тип <typeparamref name="TValue"/>.
		/// </summary>
		/// <param name="value">Строковое представление значения, извлеченное при десериализации.</param>
		/// <returns>Экземпляр типа <typeparamref name="TValue"/>, представляющий значение.</returns>
		public abstract TValue StringToValue(string value);


		/// <summary>
		/// Преобразует ключ типа <typeparamref name="TKey"/> в строковое представление для последующей сериализации.
		/// </summary>
		/// <param name="key">Экземпляр ключа типа <typeparamref name="TKey"/>.</param>
		/// <returns>Строковое представление переданного ключа.</returns>
		public abstract string KeyToString(TKey key);


		/// <summary>
		/// Преобразует значение типа <typeparamref name="TValue"/> в строковое представление для последующей сериализации.
		/// </summary>
		/// <param name="value">Экземпляр значения типа <typeparamref name="TValue"/>.</param>
		/// <returns>Строковое представление переданного значения.</returns>
		public abstract string ValueToString(TValue value);


		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр словаря с параметрами по умолчанию.
		/// </summary>
		protected _Dict_Proto()
			: base()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр словаря на основе коллекции сериализованных строк пар вида <c>"ключ=значение"</c>.
		/// </summary>
		/// <param name="serialization">Коллекция сериализованных строк. Если коллекция равна <see langword="null"/> или пуста, словарь останется пустым.</param>
		protected _Dict_Proto(
			IEnumerable<string>? serialization)
			: this()
		{
			if (serialization != null && serialization.Any())
				AddItems(serialization);
		}


		/// <summary>
		/// Инициализирует новый экземпляр словаря, десериализуя его из единой текстовой строки.
		/// </summary>
		/// <remarks>
		/// Пример строки: <c>"k1=v1;k2=v2;k3=v3"</c>. Поддерживается обработка экранированных разделителей пар (<c>\;</c>).
		/// </remarks>
		/// <param name="serialization">Общая строка сериализованных данных. Если строка равна <see langword="null"/> или пуста, инициализируется пустой словарь.</param>
		protected _Dict_Proto(
			string? serialization)
			: this()
		{
			if (string.IsNullOrEmpty(serialization))
				return;
			var parts1 = serialization.Split(';');
			var currentItem1 = new StringBuilder();
			foreach (var part1 in parts1)
			{
				if (currentItem1.Length > 0)
					currentItem1.Append(';');
				currentItem1.Append(part1);
				if (part1.EndsWith('\\'))
				{
					currentItem1.Length--;
					continue;
				}
				Add(currentItem1.ToString());
				currentItem1.Clear();
			}
		}


		/* methods */


		/// <summary>
		/// Добавляет в словарь пару ключ-значение, разбирая одну сериализованную строку вида <c>"ключ=значение"</c>.
		/// </summary>
		/// <remarks>
		/// Метод автоматически выполняет обратное преобразование (деэкранирование) для последовательностей <c>\=</c> и <c>\;</c> в оригинальные символы.
		/// Если строка пуста или не содержит разделителя <c>=</c>, добавление не производится.
		/// </remarks>
		/// <param name="serialization">Сериализованная строка одной пары данных.</param>
		public void Add(
			string serialization)
		{
			if (string.IsNullOrEmpty(serialization))
				return;
			var pair = serialization.Split('=', 2);
			if (pair.Length < 2)
				return;
			var rawKey1 = pair[0]
				.Replace("\\=", "=")
				.Replace("\\;", ";");
			var rawValue1 = pair[1]
				.Replace("\\=", "=")
				.Replace("\\;", ";");
			Add(StringToKey(rawKey1), StringToValue(rawValue1));
		}


		/// <summary>
		/// Массово десериализует и добавляет элементы в текущий словарь из коллекции строк.
		/// </summary>
		/// <param name="serialization">Последовательность сериализованных строк, каждая из которых имеет формат <c>"ключ=значение"</c>.</param>
		public void AddItems(
			IEnumerable<string> serialization)
		{
			foreach (var item1 in serialization)
				Add(item1);
		}


		/* functions */


		/// <summary>
		/// Сериализует текущий словарь в единую текстовую строку, разделяя пары символом <c>;</c>, а ключи и значения — символом <c>=</c>.
		/// </summary>
		/// <remarks>
		/// При сборки строки все входящие служебные символы <c>=</c> и <c>;</c> внутри ключей и значений принудительно экранируются символом обратного слэша (<c>\</c>).
		/// </remarks>
		/// <returns>Результирующая строка сериализованного словаря. Если словарь не содержит элементов, возвращается <see cref="string.Empty"/>.</returns>
		public override string ToString()
		{
			if (Count == 0)
				return string.Empty;
			var sb1 = new StringBuilder();
			foreach (var key1 in Keys)
			{
				var escapedKey = KeyToString(key1)
					.Replace("=", "\\=")
					.Replace(";", "\\;");
				var escapedValue = ValueToString(this[key1])
					.Replace("=", "\\=")
					.Replace(";", "\\;");
				sb1
					.Append(escapedKey)
					.Append('=')
					.Append(escapedValue)
					.Append(';');
			}

			return sb1.ToString()[..^1];
		}


		/// <summary>
		/// Безопасно возвращает значение по ключу. Если указанный ключ отсутствует в словаре, 
		/// пытается преобразовать строковое имя ключа в тип значения.
		/// </summary>
		/// <param name="key">Искомый ключ типа <typeparamref name="TKey"/>.</param>
		/// <returns>
		/// Найденное значение типа <typeparamref name="TValue"/>, либо результат применения метода <see cref="StringToValue"/> к строковому представлению ключа. 
		/// Если переданный ключ равен <see langword="null"/>, возвращается значение по умолчанию <see langword="default"/> для типа <typeparamref name="TValue"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue? GetValueOrKey(
			TKey key)
		{
			if (key == null)
				return default;
			return TryGetValue(key, out var value1)
				? value1 : StringToValue(KeyToString(key));
		}

	}

}
