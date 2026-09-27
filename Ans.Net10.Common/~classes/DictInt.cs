// rev 2026-09-25

using System.Runtime.CompilerServices;

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
			IEnumerable<string> serialization)
			: base(serialization)
		{
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DictInt"/>, десериализуя данные из переданного списка аргументов (массива строк).
		/// </summary>
		/// <param name="serialization">Массив строк, каждая из которых должна соответствовать формату <c>"ключ=значение"</c>.</param>
		public DictInt(
			params string[] serialization)
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
			string serialization)
			: base(serialization)
		{
		}


		/* overrides */


		/// <inheritdoc />
		/// <param name="key">Строковое представление ключа, извлеченное при парсинге.</param>
		/// <returns>Целочисленное значение типа <see cref="int"/>.</returns>
		/// <exception cref="FormatException">
		/// Вызывается, если переданная строка <paramref name="key"/> не может быть корректно преобразована в 32-битное целое число со знаком с помощью метода расширения <c>ToInt()</c>.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int StringToKey(
			string key)
		{
			return key.ToInt()
				?? throw new FormatException(
					$"Failed to convert the string '{key}' to an integer key of type Int32.");
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
		/// <param name="key">Целочисленный ключ типа <see cref="int"/>.</param>
		/// <returns>Строковое представление числа.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string KeyToString(
			int key)
		{
			return key.ToString();
		}


		/// <inheritdoc />
		/// <param name="value">Строковое значение типа <see cref="string"/>.</param>
		/// <returns>Строка для записи в сериализуемый поток.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ValueToString(
			string value)
		{
			return value;
		}

	}

}
