// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Представляет легковесный контейнер-обертку для связывания целочисленного идентификатора и произвольного объекта.
	/// </summary>
	/// <remarks>
	/// Реализован в виде неизменяемой структуры (<see langword="readonly record struct"/>) для минимизации аллокаций памяти в куче.
	/// </remarks>
	/// <typeparam name="T">Тип инкапсулируемого объекта, привязанного к идентификатору.</typeparam>
	/// <param name="Id">Уникальный целочисленный идентификатор (<see cref="int"/>), ассоциированный с объектом.</param>
	/// <param name="Item">Инкапсулируемый объект типа <typeparamref name="T"/>, который может быть равен <see langword="null"/>.</param>
	public readonly record struct ClassIdWrapper<T>(
		int Id,
		T? Item)
	{
	}

}
