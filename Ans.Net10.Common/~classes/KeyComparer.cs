// rev 2026-09-25

namespace Ans.Net10.Common
{

	/// <summary>
	/// Компаратор для быстрого сопоставления двух наборов целочисленных ключей (идентификаторов).
	/// Автоматически вычисляет разницу, разделяя элементы на добавленные и удаленные.
	/// </summary>
	public class KeysComparer
	{

		/* ctors */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="KeysComparer"/> на основе двух коллекций целых чисел.
		/// </summary>
		/// <remarks>
		/// Если оба параметра равны <see langword="null"/>, коллекции добавленных и удаленных ключей инициализируются как пустые.
		/// </remarks>
		/// <param name="oldKeys">Исходная (старая) коллекция целочисленных ключей или <see langword="null"/>.</param>
		/// <param name="newKeys">Целевая (новая) коллекция целочисленных ключей или <see langword="null"/>.</param>
		public KeysComparer(
			IEnumerable<int>? oldKeys,
			IEnumerable<int>? newKeys)
		{
			_keysComparer(oldKeys, newKeys);
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="KeysComparer"/> на основе двух массивов строковых представлений чисел.
		/// </summary>
		/// <remarks>
		/// Для безопасного преобразования строк в числа используется метод расширения <c>ToIntArray()</c>. 
		/// Некорректные строковые значения, которые не могут быть приведены к <see cref="int"/>, автоматически отсекаются.
		/// </remarks>
		/// <param name="oldKeys">Исходный массив строковых ключей-идентификаторов или <see langword="null"/>.</param>
		/// <param name="newKeys">Целевой массив строковых ключей-идентификаторов или <see langword="null"/>.</param>
		public KeysComparer(
			string[]? oldKeys,
			string[]? newKeys)
		{
			_keysComparer(oldKeys, newKeys);
		}


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="KeysComparer"/> на основе двух строк, содержащих списки чисел.
		/// </summary>
		/// <remarks>
		/// В качестве разделителя элементов в строке строго ожидается символ запятой (<c>','</c>). 
		/// Пустые или равные <see langword="null"/> строки обрабатываются как пустые наборы данных.
		/// </remarks>
		/// <param name="oldKeys">Строка исходных ключей, разделенных запятой (например, <c>"1,2,3"</c>).</param>
		/// <param name="newKeys">Строка целевых ключей, разделенных запятой (например, <c>"3,4,5"</c>).</param>
		public KeysComparer(
			string? oldKeys,
			string? newKeys)
		{
			var a1 = string.IsNullOrEmpty(oldKeys)
				? [] : oldKeys.Split(',');
			var a2 = string.IsNullOrEmpty(newKeys)
				? [] : newKeys.Split(',');
			_keysComparer(a1, a2);
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает неизменяемую коллекцию новых целочисленных ключей, которые появились в целевом наборе, но отсутствовали в исходном.
		/// </summary>
		/// <value>Коллекция <see cref="IReadOnlyCollection{Int32}"/>, содержащая добавленные идентификаторы.</value>
		public IReadOnlyCollection<int> Added { get; private set; } = [];


		/// <summary>
		/// Возвращает неизменяемую коллекцию старых целочисленных ключей, которые присутствовали в исходном наборе, но были удалены в целевом.
		/// </summary>
		/// <value>Коллекция <see cref="IReadOnlyCollection{Int32}"/>, содержащая удаленные идентификаторы.</value>
		public IReadOnlyCollection<int> Deleted { get; private set; } = [];


		/// <summary>
		/// Возвращает строковое представление списка добавленных ключей, объединенных через запятую.
		/// </summary>
		/// <remarks>
		/// Сборка строки осуществляется с помощью метода расширения <c>MakeFromCollection()</c>.
		/// </remarks>
		/// <value>Строка с перечислением идентификаторов (например, <c>"4,5,6"</c>). Если добавленные ключи отсутствуют, возвращается <see cref="string.Empty"/>.</value>
		public string AddedString
			=> Added.MakeFromCollection(x => x.ToString(), null, null, ",");


		/// <summary>
		/// Возвращает строковое представление списка удаленных ключей, объединенных через запятую.
		/// </summary>
		/// <remarks>
		/// Сборка строки осуществляется с помощью метода расширения <c>MakeFromCollection()</c>.
		/// </remarks>
		/// <value>Строка с перечислением идентификаторов (например, <c>"1,2"</c>). Если удаленные ключи отсутствуют, возвращается <see cref="string.Empty"/>.</value>
		public string DeletedString
			=> Deleted.MakeFromCollection(x => x.ToString(), null, null, ",");


		/// <summary>
		/// Возвращает признак наличия хотя бы одного добавленного ключа в целевом наборе.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если количество элементов в коллекции <see cref="Added"/> больше нуля; в противном случае — <see langword="false"/>.</value>
		public bool HasAdded
			=> Added.Count > 0;


		/// <summary>
		/// Возвращает признак наличия хотя бы одного удаленного ключа из исходного набора.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если количество элементов в коллекции <see cref="Deleted"/> больше нуля; в противном случае — <see langword="false"/>.</value>
		public bool HasDeleted
			=> Deleted.Count > 0;


		/* privates */


		private void _keysComparer(
			IEnumerable<int>? oldKeys,
			IEnumerable<int>? newKeys)
		{
			if (oldKeys == null && newKeys == null)
			{
				Added = [];
				Deleted = [];
			}
			else if (oldKeys == null)
			{
				Added = [.. newKeys!];
				Deleted = [];
			}
			else if (newKeys == null)
			{
				Added = [];
				Deleted = [.. oldKeys];
			}
			else
			{
				Added = [.. newKeys.Except(oldKeys)];
				Deleted = [.. oldKeys.Except(newKeys)];
			}
		}


		private void _keysComparer(
			string[]? oldKeys,
			string[]? newKeys)
		{
			var oldInts = oldKeys == null
				? [] : oldKeys.ToIntArray();
			var newInts = newKeys == null
				? [] : newKeys.ToIntArray();
			_keysComparer(oldInts, newInts);
		}

	}

}
