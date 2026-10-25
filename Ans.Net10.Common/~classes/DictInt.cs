// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Специализированный словарь с целочисленным ключом (<see cref="int"/>) и строковым значением (<see cref="string"/>),
	/// поддерживающий текстовую сериализацию и десериализацию пар данных.
	/// </summary>
	public class DictInt
		: _Dict_Proto<int, string>
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый пустой экземпляр класса <see cref="DictInt"/> с параметрами емкости по умолчанию.
		/// </summary>
		public DictInt()
			: base()
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictInt"/>, десериализуя данные из перечисления строк формата <c>"ключ=значение"</c>.
		/// </summary>
		/// <param name="serialization">Последовательность сериализованных строк. Если коллекция равна <see langword="null"/>, словарь останется пустым.</param>
		public DictInt(
			IEnumerable<string>? serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictInt"/>, десериализуя данные из переданного списка аргументов (массива строк).
		/// </summary>
		/// <param name="serialization">Массив строк, каждая из которых должна соответствовать формату <c>"ключ=значение"</c>.</param>
		public DictInt(
			params string[]? serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictInt"/>, десериализуя его из единой текстовой строки с разделителями пар.
		/// </summary>
		/// <remarks>
		/// Пример входной строки: <c>"1=Значение1;2=Значение2;3=Значение3"</c>.
		/// </remarks>
		/// <param name="serialization">Единая строка сериализованных данных. Если строка пуста или равна <see langword="null"/>, инициализируется пустой словарь.</param>
		public DictInt(
			string? serialization)
			: base(serialization)
		{
		}


		/* overrides */


		/// <inheritdoc />
		public override int StringToKey(
			string key)
		{
			return key.ToInt()
				?? throw new FormatException(
					$"Failed to convert the string '{key}' to an integer key of type Int32.");
		}


		/// <inheritdoc />
		public override string StringToValue(
			string value)
		{
			return value;
		}


		/// <inheritdoc />
		public override string KeyToString(
			int key)
		{
			return key.ToString();
		}


		/// <inheritdoc />
		public override string ValueToString(
			string value)
		{
			return value;
		}

	}

}
