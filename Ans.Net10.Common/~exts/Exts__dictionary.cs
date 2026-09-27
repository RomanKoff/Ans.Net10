// rev 2026-09-25

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для безопасного и типизированного извлечения данных из словарей.
	/// </summary>
	public static partial class Exts__dictionary
	{

		/// <summary>
		/// Преобразует пару "ключ-значение" в сериализованную строку с опциональным форматированием.
		/// </summary>
		/// <param name="item">Элемент пары "ключ-значение".</param>
		/// <param name="valueTemplate">Опциональный шаблон форматирования значения.</param>
		/// <returns>Строка, объединяющая ключ и обработанное значение.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetSerialization(
			this KeyValuePair<string, string> item,
			string? valueTemplate = null)
		{
			string? value = valueTemplate == null
				? item.Value
				: item.Value?.Make(valueTemplate);
			return $"{item.Key}{value}";
		}


		/// <summary>
		/// Безопасно возвращает значение по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <typeparam name="T">Тип значения в словаре.</typeparam>
		/// <param name="dictionary">Исходный словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <returns>Найденное значение типа <typeparamref name="T"/> или <see langword="default"/>, если словарь пуст или ключ не найден.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetFromDict<T>(
			this IDictionary<string, T>? dictionary,
			string key)
		{
			if (dictionary == null)
				return default;
			return dictionary.TryGetValue(key, out T? value1)
				? value1 : default;
		}


		/// <summary>
		/// Безопасно возвращает значение по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <typeparam name="T">Тип значения в словаре.</typeparam>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <returns>Найденное значение типа <typeparamref name="T"/> или <see langword="default"/>, если словарь пуст или ключ не найден.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? Get<T>(
			this IReadOnlyDictionary<string, T>? dictionary,
			string key)
		{
			if (dictionary == null)
				return default;
			return dictionary.TryGetValue(key, out T? value1)
				? value1 : default;
		}


		/*--- string ---*/


		/// <summary>
		/// Преобразует найденное по строковому ключу значение в строку с помощью переданной функции, возвращая ключ в случае отсутствия.
		/// </summary>
		/// <typeparam name="T">Тип значения в словаре.</typeparam>
		/// <param name="dictionary">Исходный словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="func">Функция преобразования значения в строку.</param>
		/// <param name="keyTemplate">Опциональный шаблон строки для форматирования ключа, если значение не найдено.</param>
		/// <returns>Строковый результат преобразования значения или исходный/отформатированный ключ.</returns>
		public static string GetValueStringOrKey<T>(
			this IDictionary<string, T>? dictionary,
			string key,
			Func<T, string?> func,
			string? keyTemplate = null)
		{
			if (dictionary != null
				&& dictionary.TryGetValue(key, out T? value1)
				&& value1 != null)
			{
				var s1 = func(value1);
				if (s1 != null)
					return s1;
			}
			return keyTemplate == null
				? key : string.Format(keyTemplate, key);
		}


		/// <summary>
		/// Возвращает строковое представление значения по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Строковое значение из словаря или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetStringFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			string defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getString(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает строковое значение по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Строковое значение из словаря или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetStringFromDict(
			this IDictionary<int, string>? dictionary,
			int key,
			string defaultValue)
		{
			if (dictionary == null)
				return defaultValue;
			return dictionary.TryGetValue(key, out string? value1)
				? value1 : defaultValue;
		}


		/// <summary>
		/// Возвращает строковое представление значения по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Строковое значение из словаря или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetString(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			string defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getString(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает строковое значение по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Строковое значение из словаря или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetString(
			this IReadOnlyDictionary<int, string>? dictionary,
			int key,
			string defaultValue)
		{
			if (dictionary == null)
				return defaultValue;
			return dictionary.TryGetValue(key, out string? value1)
				? value1 : defaultValue;
		}


		/*--- int ---*/


		/// <summary>
		/// Возвращает целочисленное значение по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Целое число типа <see cref="int"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIntFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			int defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getInt(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает целочисленное значение по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Целое число типа <see cref="int"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIntFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			int defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getInt(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает целочисленное значение по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Целое число типа <see cref="int"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetInt(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			int defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getInt(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает целочисленное значение по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Целое число типа <see cref="int"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetInt(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			int defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getInt(value1, defaultValue);
		}


		/*--- long ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="long"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Большое целое число типа <see cref="long"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetLongFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			long defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getLong(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="long"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Большое целое число типа <see cref="long"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetLongFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			long defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getLong(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="long"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Большое целое число типа <see cref="long"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetLong(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			long defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getLong(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="long"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Большое целое число типа <see cref="long"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long GetLong(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			long defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getLong(value1, defaultValue);
		}


		/*--- double ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="double"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="double"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetDoubleFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			double defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDouble(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="double"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="double"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetDoubleFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			double defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDouble(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="double"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="double"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetDouble(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			double defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDouble(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="double"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="double"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double GetDouble(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			double defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDouble(value1, defaultValue);
		}


		/*--- float ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="float"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="float"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFloatFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			float defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getFloat(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="float"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="float"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFloatFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			float defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getFloat(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="float"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="float"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFloat(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			float defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getFloat(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="float"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Число с плавающей запятой типа <see cref="float"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float GetFloat(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			float defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getFloat(value1, defaultValue);
		}


		/*--- decimal ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="decimal"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Высокоточное десятичное число <see cref="decimal"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal GetDecimalFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			decimal defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDecimal(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="decimal"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Высокоточное десятичное число <see cref="decimal"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal GetDecimalFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			decimal defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDecimal(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="decimal"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Высокоточное десятичное число <see cref="decimal"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal GetDecimal(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			decimal defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDecimal(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="decimal"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Высокоточное десятичное число <see cref="decimal"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal GetDecimal(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			decimal defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDecimal(value1, defaultValue);
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateTime"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Объект даты и времени <see cref="DateTime"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTimeFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			DateTime defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateTime(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateTime"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Объект даты и времени <see cref="DateTime"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTimeFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			DateTime defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateTime(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateTime"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Объект даты и времени <see cref="DateTime"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTime(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			DateTime defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateTime(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateTime"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Объект даты и времени <see cref="DateTime"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime GetDateTime(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			DateTime defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateTime(value1, defaultValue);
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateOnly"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Календарная дата <see cref="DateOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly GetDateOnlyFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			DateOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateOnly(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateOnly"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Календарная дата <see cref="DateOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly GetDateOnlyFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			DateOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateOnly(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateOnly"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Календарная дата <see cref="DateOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly GetDateOnly(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			DateOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateOnly(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="DateOnly"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Календарная дата <see cref="DateOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateOnly GetDateOnly(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			DateOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getDateOnly(value1, defaultValue);
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="TimeOnly"/> по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Компонент времени <see cref="TimeOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly GetTimeOnlyFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			TimeOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getTimeOnly(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="TimeOnly"/> по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Компонент времени <see cref="TimeOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly GetTimeOnlyFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			TimeOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getTimeOnly(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="TimeOnly"/> по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Компонент времени <see cref="TimeOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly GetTimeOnly(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			TimeOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getTimeOnly(value1, defaultValue);
		}


		/// <summary>
		/// Безопасно возвращает значение типа <see cref="TimeOnly"/> по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Значение по умолчанию, возвращаемое при отсутствии ключа.</param>
		/// <returns>Компонент времени <see cref="TimeOnly"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeOnly GetTimeOnly(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			TimeOnly defaultValue)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getTimeOnly(value1, defaultValue);
		}


		/*--- bool ---*/


		/// <summary>
		/// Возвращает логическое значение по строковому ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Логическое значение по умолчанию, если ключ не найден.</param>
		/// <returns>Логическое значение <see cref="bool"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBoolFromDict(
			this IDictionary<string, object>? dictionary,
			string key,
			bool defaultValue = false)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getBool(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает логическое значение по целочисленному ключу из изменяемого словаря.
		/// </summary>
		/// <param name="dictionary">Исходный изменяемый словарь. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Логическое значение по умолчанию, если ключ не найден.</param>
		/// <returns>Логическое значение <see cref="bool"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBoolFromDict(
			this IDictionary<int, object>? dictionary,
			int key,
			bool defaultValue = false)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getBool(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает логическое значение по строковому ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Строковый ключ для поиска.</param>
		/// <param name="defaultValue">Логическое значение по умолчанию, если ключ не найден.</param>
		/// <returns>Логическое значение <see cref="bool"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBool(
			this IReadOnlyDictionary<string, object>? dictionary,
			string key,
			bool defaultValue = false)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getBool(value1, defaultValue);
		}


		/// <summary>
		/// Возвращает логическое значение по целочисленному ключу из словаря только для чтения.
		/// </summary>
		/// <param name="dictionary">Исходный словарь только для чтения. Допускает значение <see langword="null"/>.</param>
		/// <param name="key">Целочисленный ключ для поиска.</param>
		/// <param name="defaultValue">Логическое значение по умолчанию, если ключ не найден.</param>
		/// <returns>Логическое значение <see cref="bool"/> или <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool GetBool(
			this IReadOnlyDictionary<int, object>? dictionary,
			int key,
			bool defaultValue = false)
		{
			if (dictionary == null
				|| !dictionary.TryGetValue(key, out object? value1))
				return defaultValue;
			return _getBool(value1, defaultValue);
		}


		/* privates */


		private const double _DEC_MIN_DOUBLE = (double)decimal.MinValue;
		private const double _DEC_MAX_DOUBLE = (double)decimal.MaxValue;
		private const float _DEC_MIN_FLOAT = (float)decimal.MinValue;
		private const float _DEC_MAX_FLOAT = (float)decimal.MaxValue;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string _getString(
			object value,
			string defaultValue)
		{
			return value?.ToString() ?? defaultValue;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int _getInt(
			object? value,
			int defaultValue)
		{
			return value switch
			{
				int i => i,
				long l and >= int.MinValue and <= int.MaxValue => (int)l,
				short s => s,
				byte b => b,
				null => defaultValue,
				_ => value.ToString().ToInt(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool _getBool(
			object? value,
			bool defaultValue)
		{
			return value switch
			{
				bool b => b,
				int i => i == 1,
				long l => l == 1,
				byte b => b == 1,
				short s => s == 1,
				string str when bool.TryParse(str, out bool parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToBool()
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long _getLong(
			object? value,
			long defaultValue)
		{
			return value switch
			{
				long l => l,
				int i => i,
				short s => s,
				byte b => b,
				string str when long.TryParse(str, CultureInfo.InvariantCulture, out long parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToLong(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double _getDouble(
			object? value,
			double defaultValue)
		{
			return value switch
			{
				double d => d,
				float f => f,
				int i => i,
				long l => l,
				short s => s,
				byte b => b,
				string str when double.TryParse(str, CultureInfo.InvariantCulture, out double parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToDouble(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float _getFloat(
			object? value,
			float defaultValue)
		{
			return value switch
			{
				float f => f,
				double d and >= float.MinValue and <= float.MaxValue => (float)d,
				int i => i,
				long l => (float)l,
				short s => s,
				byte b => b,
				string str when float.TryParse(str, CultureInfo.InvariantCulture, out float parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToFloat(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static decimal _getDecimal(
			object? value,
			decimal defaultValue)
		{
			return value switch
			{
				decimal dec => dec,
				int i => i,
				long l => l,
				short s => s,
				byte b => b,
				double d and >= _DEC_MIN_DOUBLE and <= _DEC_MAX_DOUBLE => (decimal)d,
				float f and >= _DEC_MIN_FLOAT and <= _DEC_MAX_FLOAT => (decimal)f,
				string str when decimal.TryParse(str, CultureInfo.InvariantCulture, out decimal parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToDecimal(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DateTime _getDateTime(
			object? value,
			DateTime defaultValue)
		{
			return value switch
			{
				DateTime dt => dt,
				string str when DateTime.TryParse(str, CultureInfo.InvariantCulture, out DateTime parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToDateTime(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static DateOnly _getDateOnly(
			object? value,
			DateOnly defaultValue)
		{
			return value switch
			{
				DateOnly d => d,
				DateTime dt => DateOnly.FromDateTime(dt),
				string str when DateOnly.TryParse(str, CultureInfo.InvariantCulture, out DateOnly parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToDateOnly(defaultValue)
			};
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TimeOnly _getTimeOnly(
			object? value,
			TimeOnly defaultValue)
		{
			return value switch
			{
				TimeOnly t => t,
				DateTime dt => TimeOnly.FromDateTime(dt),
				TimeSpan ts => TimeOnly.FromTimeSpan(ts),
				string str when TimeOnly.TryParse(str, CultureInfo.InvariantCulture, out TimeOnly parsed) => parsed,
				null => defaultValue,
				_ => value.ToString().ToTimeOnly(defaultValue)
			};
		}

	}

}
