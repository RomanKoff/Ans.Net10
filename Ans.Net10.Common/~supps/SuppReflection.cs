// rev 2026-09-26

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Вспомогательный класс для работы с рефлексией и метаданными типов.
	/// </summary>
	public static class SuppReflection
	{

		/* functions */


		/// <summary>
		/// Преобразует системное имя встроенного типа CTS в его стандартный C# эквивалент (алиас).
		/// </summary>
		/// <param name="name">Исходное системное имя типа (например, <c>"Int32"</c> или <c>"Boolean"</c>).</param>
		/// <returns>Строковый алиас типа, принятый в синтаксисе языка C# (например, <c>"int"</c>, <c>"bool"</c>).</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FixCSharpName(
			string name)
		{
			return name switch
			{
				"Boolean" => "bool",
				"Char" => "char",
				"Decimal" => "decimal",
				"Double" => "double",
				"Int32" => "int",
				"Int64" => "long",
				"Single" => "float",
				"String" => "string",
				"Void" => "void",
				_ => name
			};
		}


		/// <summary>
		/// Возвращает текстовое представление значения любого объекта, адаптированное под синтаксис языка C# или детальный отладочный вывод.
		/// </summary>
		/// <remarks>
		/// Метод интеллектуально обрабатывает пустые строки, типы <c>HtmlString</c>, логические флаги, даты, перечисления (с получением лежащего в основе числового значения), а также вычисляет размеры массивов и коллекций.
		/// </remarks>
		/// <param name="value">Исходный объект для текстового анализа. Допускает значение <see langword="null"/>.</param>
		/// <param name="useFullView">Признак генерации развернутого (полного) текстового описания для сложных объектов, словарей и обобщений. По умолчанию равен <see langword="false"/>.</param>
		/// <returns>Строка с форматированным значением объекта; если передан <see langword="null"/>, возвращает строковый маркер <c>"null"</c>.</returns>
		public static string GetCSharpValue(
			object? value,
			bool useFullView = false)
		{
			if (value == null)
				return "null";
			var type1 = value.GetType();
			var name1 = type1.Name;
			return name1 switch
			{
				"String" => (string)value == string.Empty
					? "Empty" : $"\"{value}\"",
				"HtmlString" => value.ToString() == string.Empty
					? "HtmlEmpty" : $"~{value}~",
				"Int32" or
				"Int64" or
				"Single" or
				"Double" or
				"Decimal" => value.ToString()!,
				"Boolean" => (bool)value
					? "true" : "false",
				_ => useFullView
					? _getCSharpValueFull(value, name1, type1)
					: _getCSharpValueShort(value, name1, type1)
			};
		}


		/// <summary>
		/// Возвращает упорядоченный по алфавиту массив типов, принадлежащих исключительно указанному пространству имен в заданной сборке.
		/// </summary>
		/// <param name="assembly">Исследуемая сборка объектов <see cref="Assembly"/>.</param>
		/// <param name="ns">Целевое строгое пространство имен (Namespace) для фильтрации типов.</param>
		/// <returns>Массив отфильтрованных и отсортированных объектов <see cref="Type"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetNamespaceTypes(
			Assembly assembly,
			string ns)
		{
			return [.. assembly.GetTypes()
				.Where(x => string.Equals(x.Namespace, ns, StringComparison.Ordinal))
				.OrderBy(x => x.Name)];
		}


		/* privates */


		private static string _getCSharpValueFull(
			object value,
			string name,
			Type type)
		{
			if (type.IsEnum)
				return $"{Convert.ChangeType(value, Enum.GetUnderlyingType(type))}.{value}";
			if (value is DateTime dateTime1)
				return dateTime1.ToString("G");
			if (value is DateOnly dateOnly)
				return dateOnly.ToString();
			if (value is TimeOnly timeOnly)
				return timeOnly.ToString("HH:mm:ss");
			if (type.IsArray
				|| value is ICollection
				|| value is IDictionary)
			{
				if (value is ICollection collection)
					return $"[{collection.Count}]";
				if (value is IDictionary dictionary)
					return $"[{dictionary.Count}]";
				if (value is IEnumerable enumerable)
				{
					int count1 = 0;
					var enumerator1 = enumerable.GetEnumerator();
					while (enumerator1.MoveNext())
						count1++;
					return $"[{count1}]";
				}
			}
			if (!name.Contains('`'))
				return $"[{value}]";
			if (name.Contains("Dictionary"))
				return $"[{name}]";
			if (value is IEnumerable enumFallback1)
			{
				int count1 = 0;
				var enumerator1 = enumFallback1.GetEnumerator();
				while (enumerator1.MoveNext())
					count1++;
				return $"[{count1}]";
			}
			return $"[{name}]";
		}


		private static string _getCSharpValueShort(
			object value,
			string name,
			Type type)
		{
			if (type.IsEnum)
				return $"{Convert.ChangeType(value, Enum.GetUnderlyingType(type))}.{value}";
			if (value is DateTime dateTime1)
				return dateTime1.ToString("G");
			if (value is DateOnly dateOnly)
				return dateOnly.ToString();
			if (value is TimeOnly timeOnly)
				return timeOnly.ToString("HH:mm:ss");
			if (type.IsArray
				|| value is ICollection
				|| value is IDictionary)
			{
				if (value is ICollection collection)
					return $"[{collection.Count}]";
				if (value is IDictionary dictionary)
					return $"[{dictionary.Count}]";
				if (value is IEnumerable enumerable)
				{
					int count1 = 0;
					var enumerator1 = enumerable.GetEnumerator();
					while (enumerator1.MoveNext())
						count1++;
					return $"[{count1}]";
				}
			}
			if (!name.Contains('`'))
				return "[object]";
			if (name.Contains("Dictionary"))
				return "[dict]";
			if (value is IEnumerable enumFallback)
			{
				int count1 = 0;
				var enumerator1 = enumFallback.GetEnumerator();
				while (enumerator1.MoveNext())
					count1++;
				return $"[{count1}]";
			}
			return "[object]";
		}

	}

}
