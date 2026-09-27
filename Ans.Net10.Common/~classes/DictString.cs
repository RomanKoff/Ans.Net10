// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Специализированный строковый словарь (строковые ключи и строковые значения),
	/// поддерживающий текстовую сериализацию и десериализацию пар данных.
	/// </summary>
	public class DictString : _Dict_Proto<string, string>
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="DictString"/> с параметрами емкости по умолчанию.
		/// </summary>
		public DictString()
			: base()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictString"/>, десериализуя данные из перечисления строк формата <c>"ключ=значение"</c>.
		/// </summary>
		/// <param name="serialization">Последовательность сериализованных строк. Если коллекция равна <see langword="null"/>, словарь останется пустым.</param>
		public DictString(
			IEnumerable<string> serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictString"/>, десериализуя данные из переданного списка аргументов (массива строк).
		/// </summary>
		/// <param name="serialization">Массив строк, каждая из которых должна соответствовать формату <c>"ключ=значение"</c>.</param>
		public DictString(
			params string[] serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictString"/>, десериализуя его из единой текстовой строки с разделителями пар.
		/// </summary>
		/// <remarks>
		/// Пример входной строки: <c>"k1=v1;k2=v2;k3=v3"</c>.
		/// </remarks>
		/// <param name="serialization">Единая строка сериализованных данных. Если строка пуста или равна <see langword="null"/>, инициализируется пустой словарь.</param>
		public DictString(
			string serialization)
			: base(serialization)
		{
		}


		/* overrides */


		/// <inheritdoc />
		/// <param name="key">Строковое представление ключа, извлеченное при парсинге.</param>
		/// <returns>Оригинальное строковое значение ключа типа <see cref="string"/> без изменений.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string StringToKey(
			string key)
		{
			return key;
		}


		/// <inheritdoc />
		/// <param name="value">Строковое представление значения, извлеченное при парсинге.</param>
		/// <returns>Оригинальное строковое значение типа <see cref="string"/> без изменений.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string StringToValue(
			string value)
		{
			return value;
		}


		/// <inheritdoc />
		/// <param name="key">Строковый ключ типа <see cref="string"/>.</param>
		/// <returns>Оригинальная строка ключа для записи в сериализуемый поток.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string KeyToString(
			string key)
		{
			return key;
		}


		/// <inheritdoc />
		/// <param name="value">Строковое значение типа <see cref="string"/>.</param>
		/// <returns>Оригинальная строка значения для записи в сериализуемый поток.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ValueToString(
			string value)
		{
			return value;
		}

	}

}
