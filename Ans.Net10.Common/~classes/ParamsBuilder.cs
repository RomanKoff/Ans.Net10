// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Строитель параметров URL-строки запроса (Query String), поддерживающий безопасное 
	/// динамическое добавление и генерацию адресов со сквозным сохранением или временным наложением параметров.
	/// </summary>
	public class ParamsBuilder
	{

		/* readonly properties */


		/// <summary>
		/// Возвращает базовую внутреннюю коллекцию параметров запроса.
		/// </summary>
		/// <value>Объект класса <see cref="ParamsCollection"/>, хранящий текущий набор пар ключ-значение.</value>
		public ParamsCollection Parameters { get; } = new();


		/* methods */


		/// <summary>
		/// Добавляет новый или обновляет существующий строковый параметр в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Строковое значение параметра. Если передана пустая строка или <see langword="null"/>, параметр будет обработан согласно правилам фильтрации коллекции.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			string value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет логический параметр в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Логическое значение типа <see cref="bool"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			bool value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет целочисленный параметр в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">32-битное целое число со знаком типа <see cref="int"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			int value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет числовой параметр типа long в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">64-битное целое число со знаком типа <see cref="long"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			long value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет числовой параметр типа double в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Число с плавающей запятой двойной точности типа <see cref="double"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			double value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет числовой параметр типа float в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Число с плавающей запятой одинарной точности типа <see cref="float"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			float value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет числовой параметр типа decimal в коллекции.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Десятичное число с высокой точностью типа <see cref="decimal"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			decimal value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет параметр даты и времени в коллекции с поддержкой значений <see langword="null"/>.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Значение структуры <see cref="DateTime"/>, допускающее значение <see langword="null"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			DateTime? value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет параметр даты в коллекции с поддержкой значений <see langword="null"/>.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Значение структуры <see cref="DateOnly"/>, допускающее значение <see langword="null"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			DateOnly? value)
		{
			Parameters.Append(name, value);
		}


		/// <summary>
		/// Добавляет или обновляет параметр времени в коллекции с поддержкой значений <see langword="null"/>.
		/// </summary>
		/// <param name="name">Уникальное имя (ключ) параметра URL.</param>
		/// <param name="value">Значение структуры <see cref="TimeOnly"/>, допускающее значение <see langword="null"/>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(
			string name,
			TimeOnly? value)
		{
			Parameters.Append(name, value);
		}


		/* functions */


		/// <summary>
		/// Генерирует итоговую строку URL-параметров, временно объединяя текущую коллекцию с одним 
		/// дополнительным строковым параметром без изменения основного внутреннего состояния строителя.
		/// </summary>
		/// <remarks>
		/// Метод полностью изолирован: он копирует текущие элементы в новый экземпляр <see cref="ParamsCollection"/>, 
		/// добавляет к нему временную пару и производит сериализацию. Содержимое свойства <see cref="Parameters"/> остается прежним.
		/// </remarks>
		/// <param name="additionName">Имя временного строкового параметра, накладываемого на текущий запрос.</param>
		/// <param name="additionValue">Значение временного строкового параметра.</param>
		/// <returns>Результирующая форматированная строка URL-запроса (например, <c>"id=5&amp;mode=edit&amp;temp=true"</c>).</returns>
		public string GetString(
			string additionName,
			string additionValue)
		{
			var tempCollection1 = new ParamsCollection(
				Parameters.Items.Count + 1, Parameters.Items.Comparer);
			foreach (var item1 in Parameters.Items)
				tempCollection1.Append(item1.Key, item1.Value);
			tempCollection1.Append(additionName, additionValue);
			return tempCollection1.ToString();
		}


		/// <summary>
		/// Генерирует итоговую строку URL-параметров, временно объединяя текущую коллекцию с одним 
		/// дополнительным целочисленным параметром без изменения основного внутреннего состояния строителя.
		/// </summary>
		/// <remarks>
		/// Значение <paramref name="additionValue"/> автоматически приводится к строковому представлению. Метод полностью безопасен для многократного использования с разными индексами (например, при рендеринге ссылок постраничной пагинации).
		/// </remarks>
		/// <param name="additionName">Имя временного целочисленного параметра (например, <c>"page"</c>).</param>
		/// <param name="additionValue">Числовое значение временного параметра (номер страницы).</param>
		/// <returns>Результирующая форматированная строка URL-запроса.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetString(
			string additionName,
			int additionValue)
		{
			return GetString(
				additionName, additionValue.ToString());
		}


		/// <summary>
		/// Возвращает полную сериализованную строку параметров на основе текущего зафиксированного состояния строителя.
		/// </summary>
		/// <returns>Готовая строка параметров URL-запроса, сформированная методом <see cref="ParamsCollection.ToString()"/>.</returns>
		public override string ToString()
		{
			return Parameters.ToString();
		}

	}

}
