// rev 2026-09-26

using System.Buffers;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для трансформации типов, разбиения коллекций на блоки и быстрого парсинга строк в числовые массивы.
	/// </summary>
	public static partial class Exts__to
	{

		/* consts */


		/// <summary>
		/// Массив стандартных символов-разделителей (запятая, точка с запятой, вертикальная черта), используемых при парсинге строк.
		/// </summary>
		/// <value>Массив символов, содержащий разделители <c>','</c>, <c>';'</c> и <c>'|'</c>.</value>
		public static readonly char[] SEP_ITEMS = [',', ';', '|'];


		/// <summary>
		/// Оптимизированная структура поиска разделителей для высокопроизводительных строковых операций ввода-вывода.
		/// </summary>
		/// <value>Экземпляр <see cref="SearchValues{Char}"/>, созданный на основе базового массива разделителей.</value>
		public static readonly SearchValues<char> SEP_ITEMS2 = SearchValues.Create(SEP_ITEMS);

	}

}
