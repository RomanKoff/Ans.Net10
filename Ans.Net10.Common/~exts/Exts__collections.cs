// rev 2026-09-25

using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для работы с массивами, коллекциями и последовательностями строк.
	/// </summary>
	public static partial class Exts__collections
	{

		/// <summary>
		/// Возвращает новый массив, добавляя указанный элемент в конец исходного массива.
		/// </summary>
		/// <typeparam name="T">Тип элементов обрабатываемого массива.</typeparam>
		/// <param name="source">Исходный экземпляр массива, к которому добавляется элемент. Допускает значение <see langword="null"/>.</param>
		/// <param name="item">Объект, вставляемый в конец целевого массива.</param>
		/// <returns>Новый массив, содержащий все элементы исходного набора и добавленный элемент на последней позиции.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] GetArrayAdd<T>(
			this T[]? source,
			T item)
		{
			if (source == null || source.Length == 0)
				return [item];
			return [.. source, item];
		}


		/// <summary>
		/// Возвращает новый массив, вставляя указанный элемент по заданному индексу.
		/// </summary>
		/// <typeparam name="T">Тип элементов обрабатываемого массива.</typeparam>
		/// <param name="source">Исходный экземпляр массива для модификации. Допускает значение <see langword="null"/>.</param>
		/// <param name="index">Порядковый номер позиции (начиная с 0), по которой необходимо осуществить вставку.</param>
		/// <param name="item">Элемент, подлежащий вставке в указанную позицию.</param>
		/// <returns>Новый массив, в который внедрен элемент на позицию <paramref name="index"/> со сдвигом последующих элементов.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Выбрасывается, если значение <paramref name="index"/> меньше нуля или строго больше фактической длины исходного массива.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] GetArrayInsert<T>(
			this T[]? source,
			int index,
			T item)
		{
			int len1 = source?.Length ?? 0;
			if (index < 0 || index > len1)
				throw new ArgumentOutOfRangeException(
					nameof(index), "The index is outside the boundaries of the array.");
			if (source == null || len1 == 0)
				return [item];
			return [.. source[..index], item, .. source[index..]];
		}


		/// <summary>
		/// Возвращает новый массив, удаляя элемент по указанному индексу.
		/// </summary>
		/// <typeparam name="T">Тип элементов обрабатываемого массива.</typeparam>
		/// <param name="source">Исходный экземпляр массива для удаления элемента. Допускает значение <see langword="null"/>.</param>
		/// <param name="index">Порядковый индекс элемента (начиная с 0), подлежащего удалению.</param>
		/// <returns>Новый усеченный массив, не содержащий элемент, ранее находившийся по индексу <paramref name="index"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Выбрасывается, если исходный массив равен <see langword="null"/>, пуст, или если <paramref name="index"/> выходит за фактические границы массива.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] GetArrayRemoveAt<T>(
			this T[]? source,
			int index)
		{
			if (source == null
				|| source.Length == 0
				|| index < 0
				|| index >= source.Length)
				throw new ArgumentOutOfRangeException(
					nameof(index), "The index is outside the boundaries of the array.");
			if (source.Length == 1)
				return [];
			return [.. source[..index], .. source[(index + 1)..]];
		}


		/// <summary>
		/// Возвращает новый массив, удаляя первое вхождение указанного элемента. 
		/// Если элемент не найден, возвращает исходный массив.
		/// </summary>
		/// <typeparam name="T">Тип элементов обрабатываемого массива.</typeparam>
		/// <param name="source">Исходный экземпляр массива. Допускает значение <see langword="null"/>.</param>
		/// <param name="item">Целевой объект, первое вхождение которого необходимо исключить из массива.</param>
		/// <returns>Новый уменьшенный массив без первого вхождения указанного элемента или оригинальный массив, если совпадений не найдено.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] GetArrayRemove<T>(
			this T[]? source,
			T item)
		{
			if (source == null || source.Length == 0)
				return [];
			int index1 = Array.IndexOf(source, item);
			if (index1 == -1)
				return source;
			return source.GetArrayRemoveAt(index1);
		}


		/// <summary>
		/// Синхронизирует коллекции и возвращает изменения в виде именованного кортежа с ленивыми потоками.
		/// </summary>
		/// <typeparam name="T">Тип элементов сопоставляемых коллекций.</typeparam>
		/// <typeparam name="TKey">Тип уникального идентификатора (ключа) элементов.</typeparam>
		/// <param name="current">Исходная (текущая) последовательность элементов.</param>
		/// <param name="newest">Целевая (новая) последовательность элементов, с которой производится сверка.</param>
		/// <param name="keySelector">Делегат функции для извлечения уникального ключа типа <typeparamref name="TKey"/> из элемента.</param>
		/// <param name="comparer">Кастомный компаратор для проверки эквивалентности ключей. Если передано значение <see langword="null"/>, применяется компаратор по умолчанию.</param>
		/// <returns>Именованный кортеж, содержащий ленивые потоки добавленных (<c>Added</c>), удаленных (<c>Removed</c>) и оставшихся актуальными (<c>Updated</c>) элементов.</returns>
		public static
			(IEnumerable<T> Added, IEnumerable<T> Removed, IEnumerable<T> Updated)
			GetDiffLazy<T, TKey>(
				this IEnumerable<T> current,
				IEnumerable<T> newest,
				Func<T, TKey> keySelector,
				IEqualityComparer<TKey>? comparer = null)
			where TKey : notnull
		{
			comparer ??= EqualityComparer<TKey>.Default;
			var currentKeys1 = current.Select(keySelector).ToHashSet(comparer);
			var newestKeys1 = newest.Select(keySelector).ToHashSet(comparer);
			var added1 = newest.Where(x => !currentKeys1.Contains(keySelector(x)));
			var removed1 = current.Where(x => !newestKeys1.Contains(keySelector(x)));
			var updated1 = newest.Where(x => currentKeys1.Contains(keySelector(x)));
			return (added1, removed1, updated1);
		}


		/// <summary>
		/// Синхронизирует коллекции и возвращает изменения в виде именованного кортежа с материализованными списками.
		/// </summary>
		/// <typeparam name="T">Тип элементов сопоставляемых коллекций.</typeparam>
		/// <typeparam name="TKey">Тип уникального идентификатора (ключа) элементов.</typeparam>
		/// <param name="current">Исходная (текущая) последовательность элементов.</param>
		/// <param name="newest">Целевая (новая) последовательность элементов, с которой производится сверка.</param>
		/// <param name="keySelector">Делегат функции для извлечения уникального ключа типа <typeparamref name="TKey"/> из элемента.</param>
		/// <param name="comparer">Кастомный компаратор для проверки эквивалентности ключей. Если передано значение <see langword="null"/>, применяется компаратор по умолчанию.</param>
		/// <returns>Именованный кортеж, содержащий материализованные списки добавленных (<c>Added</c>), удаленных (<c>Removed</c>) и обновленных (<c>Updated</c>) элементов.</returns>
		public static
			(List<T> Added, List<T> Removed, List<T> Updated)
			GetDiffList<T, TKey>(
				this IEnumerable<T> current,
				IEnumerable<T> newest,
				Func<T, TKey> keySelector,
				IEqualityComparer<TKey>? comparer = null)
			where TKey : notnull
		{
			comparer ??= EqualityComparer<TKey>.Default;
			var newestMap1 = new Dictionary<TKey, T>(comparer);
			foreach (var item1 in newest)
				newestMap1[keySelector(item1)] = item1;
			var added1 = new List<T>();
			var updated1 = new List<T>();
			var removed1 = new List<T>();
			var currentKeys1 = new HashSet<TKey>(comparer);
			foreach (var item1 in current)
			{
				var key1 = keySelector(item1);
				currentKeys1.Add(key1);
				if (newestMap1.TryGetValue(key1, out var newItem1))
					updated1.Add(newItem1);
				else
					removed1.Add(item1);
			}
			foreach (var kvp1 in newestMap1)
				if (!currentKeys1.Contains(kvp1.Key))
					added1.Add(kvp1.Value);
			return (added1, removed1, updated1);
		}


		/// <summary>
		/// Возвращает текущие версии элементов, которые все еще присутствуют в новой коллекции.
		/// </summary>
		/// <typeparam name="T">Тип элементов сопоставляемых коллекций.</typeparam>
		/// <typeparam name="TKey">Тип уникального ключа элементов.</typeparam>
		/// <param name="current">Исходная (текущая) последовательность элементов.</param>
		/// <param name="newest">Целевая (новая) последовательность для проверки присутствия.</param>
		/// <param name="keySelector">Делегат функции извлечения уникального ключа.</param>
		/// <returns>Ленивый поток элементов из старой коллекции, чьи ключи присутствуют в новой коллекции.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> GetActual<T, TKey>(
			this IEnumerable<T> current,
			IEnumerable<T> newest,
			Func<T, TKey> keySelector)
			where TKey : notnull
		{
			return current.IntersectBy(
				newest.Select(keySelector), keySelector);
		}


		/// <summary>
		/// Возвращает обновленные версии элементов из новой коллекции, которые уже существовали в текущей коллекции.
		/// </summary>
		/// <typeparam name="T">Тип элементов сопоставляемых коллекций.</typeparam>
		/// <typeparam name="TKey">Тип уникального ключа элементов.</typeparam>
		/// <param name="current">Исходная (текущая) последовательность элементов для проверки существования.</param>
		/// <param name="newest">Целевая (новая) последовательность, из которой извлекаются актуальные элементы.</param>
		/// <param name="keySelector">Делегат функции извлечения уникального ключа.</param>
		/// <returns>Ленивый поток элементов из новой коллекции, чьи ключи присутствуют в текущей коллекции.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> GetUpdated<T, TKey>(
			this IEnumerable<T> current,
			IEnumerable<T> newest,
			Func<T, TKey> keySelector)
			where TKey : notnull
		{
			return newest.IntersectBy(
				current.Select(keySelector), keySelector);
		}


		/// <summary>
		/// Возвращает элементы, которые появились в новой коллекции, но полностью отсутствовали в текущей.
		/// </summary>
		/// <typeparam name="T">Тип элементов сопоставляемых коллекций.</typeparam>
		/// <typeparam name="TKey">Тип уникального ключа элементов.</typeparam>
		/// <param name="current">Исходная (текущая) последовательность элементов, выполняющая роль базиса.</param>
		/// <param name="newest">Целевая (новая) последовательность элементов для поиска новинок.</param>
		/// <param name="keySelector">Делегат функции извлечения уникального ключа.</param>
		/// <returns>Ленивый поток добавленных элементов, присутствующих исключительно в новой коллекции.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> GetAdded<T, TKey>(
			this IEnumerable<T> current,
			IEnumerable<T> newest,
			Func<T, TKey> keySelector)
			where TKey : notnull
		{
			return newest.ExceptBy(
				current.Select(keySelector), keySelector);
		}


		/// <summary>
		/// Возвращает элементы, которые присутствовали в текущей коллекции, но отсутствуют в новой.
		/// </summary>
		/// <typeparam name="T">Тип элементов сопоставляемых коллекций.</typeparam>
		/// <typeparam name="TKey">Тип уникального ключа элементов.</typeparam>
		/// <param name="current">Исходная (текущая) последовательность элементов.</param>
		/// <param name="newest">Целевая (новая) последовательность, определяющая исключаемые ключи.</param>
		/// <param name="keySelector">Делегат функции извлечения уникального ключа.</param>
		/// <returns>Ленивый поток удаленных элементов, которые были утеряны в новой коллекции.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> GetRemoved<T, TKey>(
			this IEnumerable<T> current,
			IEnumerable<T> newest,
			Func<T, TKey> keySelector)
			where TKey : notnull
		{
			return current.ExceptBy(
				newest.Select(keySelector), keySelector);
		}


		/// <summary>
		/// Очищает элементы последовательности строк от начальных и конечных пробелов, полностью удаляя пустые элементы и <see langword="null"/>.
		/// </summary>
		/// <param name="source">Исходная последовательность строк, допускающих значение <see langword="null"/>.</param>
		/// <returns>Ленивый итератор <see cref="IEnumerable{T}"/>, возвращающий очищенные непустые строки.</returns>
		public static IEnumerable<string> GetItemsClean(
			this IEnumerable<string?> source)
		{
			foreach (var item1 in source)
			{
				if (item1 is null)
					continue;
				var s1 = item1.Trim();
				if (s1.Length > 0)
					yield return s1;
			}
		}


		/// <summary>
		/// Обрезает начальные и конечные пробелы у всех элементов последовательности строк, сохраняя <see langword="null"/>-элементы.
		/// </summary>
		/// <param name="source">Исходная последовательность строк, допускающих значение <see langword="null"/>.</param>
		/// <returns>Последовательность строк с обрезанными пробелами, где позиции элементов со значением <see langword="null"/> не изменяются.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string?> GetItemsTrim(
			this IEnumerable<string?> source)
		{
			return source.Select(
				x => x?.Trim());
		}


		/// <summary>
		/// Исключает из последовательности строк все пустые элементы и элементы со значением <see langword="null"/>.
		/// </summary>
		/// <param name="source">Исходная последовательность строк, допускающих значение <see langword="null"/>.</param>
		/// <returns>Последовательность строк, гарантированно не содержащая пустых значений или <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> GetItemsWithoutEmpty(
			this IEnumerable<string?> source)
		{
			return source.Where(
				x => !string.IsNullOrEmpty(x))!;
		}


		/// <summary>
		/// Возвращает уникальные элементы последовательности строк с возможностью гибкой настройки компаратора.
		/// </summary>
		/// <param name="source">Исходная последовательность строк, допускающих значение <see langword="null"/>.</param>
		/// <param name="comparer">Кастомный компаратор для определения уникальности строк. Если не задан, используется <see cref="StringComparer.Ordinal"/>.</param>
		/// <returns>Последовательность уникальных строк, отфильтрованная в соответствии с правилами сравнения.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string?> GetItemsUnique(
			this IEnumerable<string?> source,
			StringComparer? comparer = null)
		{
			return source.Distinct(
				comparer ?? StringComparer.Ordinal);
		}


		/// <summary>
		/// Очищает элементы от пробелов, удаляет пустые и <see langword="null"/> значения и возвращает только уникальные строки.
		/// </summary>
		/// <param name="source">Исходная последовательность строк, допускающих значение <see langword="null"/>.</param>
		/// <param name="comparer">Компаратор для определения уникальности строк. Если передано значение <see langword="null"/>, используется <see cref="StringComparer.Ordinal"/>.</param>
		/// <returns>Ленивый итератор, возвращающий очищенные от пробелов уникальные непустые строки.</returns>
		public static IEnumerable<string> GetItemsCleanUnique(
			this IEnumerable<string?> source,
			StringComparer? comparer = null)
		{
			comparer ??= StringComparer.Ordinal;
			var seen1 = new HashSet<string>(comparer);
			foreach (var item1 in source)
			{
				if (item1 is null)
					continue;
				var s1 = item1.Trim();
				if (s1.Length > 0 && seen1.Add(s1))
					yield return s1;
			}
		}

	}

}
