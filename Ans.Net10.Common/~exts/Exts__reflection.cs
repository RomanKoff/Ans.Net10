// rev 2026-09-26

using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для продвинутой работы с механизмами рефлексии (Reflection), извлечения метаданных, свойств, полей и генерации имен типов.
	/// </summary>
	public static partial class Exts__reflection
	{

		/* functions */


		/// <summary>
		/// Преобразует публичные свойства объекта в динамический объект <see cref="ExpandoObject"/> на основе дескрипторов типов.
		/// </summary>
		/// <param name="value">Исходный экземпляр объекта для преобразования.</param>
		/// <returns>Динамический объект <see cref="ExpandoObject"/>, содержащий свойства исходного объекта.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="value"/> равен <see langword="null"/>.</exception>
		public static dynamic ToDynamic(
			this object value)
		{
			ArgumentNullException.ThrowIfNull(value);
			IDictionary<string, object?> expando1 = new ExpandoObject();
			foreach (PropertyDescriptor property1 in TypeDescriptor.GetProperties(value.GetType()))
				expando1.Add(property1.Name, property1.GetValue(value));
			return expando1;
		}


		/// <summary>
		/// Возвращает кастомный атрибут указанного типа, примененный к целевому типу данных.
		/// </summary>
		/// <typeparam name="T">Тип искомого атрибута, унаследованный от <see cref="Attribute"/>.</typeparam>
		/// <param name="type">Исследуемый тип данных.</param>
		/// <returns>Экземпляр найденного атрибута типа <typeparamref name="T"/> или <see langword="null"/>, если атрибут отсутствует.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? GetAttribute<T>(
			this Type type)
			where T : Attribute
		{
			return (T?)Attribute.GetCustomAttribute(type, typeof(T));
		}


		/// <summary>
		/// Универсально извлекает значение поля или свойства объекта через метаданные члена типа <see cref="MemberInfo"/>.
		/// </summary>
		/// <param name="info">Информация о члене типа (поддерживаются исключительно <see cref="FieldInfo"/> или <see cref="PropertyInfo"/>).</param>
		/// <param name="forObject">Экземпляр объекта, из которого извлекается значение члена.</param>
		/// <returns>Объект, представляющий значение указанного члена.</returns>
		/// <exception cref="NotImplementedException">Выбрасывается, если тип члена не относится к полям или свойствам.</exception>
		public static object? GetValue(
			this MemberInfo info,
			object forObject)
		{
			return info.MemberType switch
			{
				MemberTypes.Field => ((FieldInfo)info).GetValue(forObject),
				MemberTypes.Property => ((PropertyInfo)info).GetValue(forObject),
				_ => throw new NotImplementedException(
					$"[Ans.Net10.Common] Extracting a value for the member type {info.MemberType} is not supported.")
			};
		}


		/// <summary>
		/// Возвращает удобочитаемое имя типа в синтаксисе языка C#, поддерживая массивы, Nullable и обобщения (Generics).
		/// </summary>
		/// <param name="type">Исследуемый тип данных.</param>
		/// <param name="isDropNullable">Признак принудительного удаления знака <c>'?'</c> для типов, допускающих значение <see langword="null"/>.</param>
		/// <returns>Строковое представление корректного имени типа в синтаксисе C#.</returns>
		/// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="type"/> равен <see langword="null"/>.</exception>
		public static string GetCSharpTypeName(
			this Type type,
			bool isDropNullable = false)
		{
			ArgumentNullException.ThrowIfNull(type);
			if (type.IsArray)
			{
				var type1 = type.GetElementType();
				return $"{type1?.GetCSharpTypeName(isDropNullable)}[]";
			}
			var name1 = type.Name;
			if (name1.StartsWith("Nullable`"))
			{
				var type1 = Nullable.GetUnderlyingType(type);
				return $"{type1?.GetCSharpTypeName(isDropNullable)}{isDropNullable.Make(string.Empty, "?")}";
			}
			int i1 = name1.IndexOf('`');
			if (i1 < 0)
				return SuppReflection.FixCSharpName(name1);
			var a1 = type.GenericTypeArguments.GetCSharpTypeNames(isDropNullable);
			return $"{name1[..i1]}<{a1.MakeFromCollection(null, null, ", ")}>";
		}


		/// <summary>
		/// Возвращает коллекцию удобочитаемых имён C# для переданного массива типов.
		/// </summary>
		/// <param name="types">Массив исследуемых типов данных.</param>
		/// <param name="isDropNullable">Признак принудительного удаления знака <c>'?'</c> для Nullable типов.</param>
		/// <returns>Перечисление <see cref="IEnumerable{String}"/> строковых имён типов.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<string> GetCSharpTypeNames(
			this Type[] types,
			bool isDropNullable = false)
		{
			return types.Select(x => x.GetCSharpTypeName(isDropNullable));
		}


		/// <summary>
		/// Возвращает строковую запись обобщенных параметров (Generics) метода в стиле разметки C#.
		/// </summary>
		/// <param name="info">Метаданные исследуемого метода.</param>
		/// <returns>Строка вида <c>&lt;T1, T2&gt;</c> или <see langword="null"/>, если метод не является обобщенным (Generic).</returns>
		public static string? GetCSharpGenerics(
			this MethodBase info)
		{
			if (!info.IsGenericMethod)
				return null;
			var a1 = GetCSharpTypeNames(info.GetGenericArguments());
			return $"<{a1.MakeFromCollection(null, null, ", ")}>";
		}


		/// <summary>
		/// Возвращает коллекцию строковых представлений параметров метода в формате синтаксиса объявления C# (включая ключевое слово params и дефолтные значения).
		/// </summary>
		/// <param name="info">Метаданные исследуемого метода.</param>
		/// <returns>Перечисление строк с описанием параметров метода.</returns>
		public static IEnumerable<string> GetCSharpParams(
			this MethodBase info)
		{
			var a1 = new List<string>();
			foreach (var item1 in info.GetParameters())
			{
				string prefix1 = item1.GetCustomAttribute<ParamArrayAttribute>() != null ? "params " : string.Empty;
				string name1 = item1.ParameterType.GetCSharpTypeName();
				string default1 = item1.HasDefaultValue
					? $" = {SuppReflection.GetCSharpValue(item1.DefaultValue)}"
					: string.Empty;
				a1.Add($"{prefix1}{name1} {item1.Name}{default1}");
			}
			return a1;
		}


		/// <summary>
		/// Очищает имя метода доступа к свойству от служебных системных префиксов <c>"get_"</c> или <c>"set_"</c>, возвращая чистое имя свойства.
		/// </summary>
		/// <param name="info">Метаданные метода доступа (аксессора).</param>
		/// <returns>Очищенное имя целевого свойства.</returns>
		public static string GetPropertyName(
			this MethodInfo info)
		{
			string name1 = info.Name;
			if (name1.StartsWith("get_") || name1.StartsWith("set_"))
				return name1[4..];
			return name1;
		}


		/// <summary>
		/// Возвращает значение свойства объекта по его имени на основе указанного базового типа метаданных.
		/// </summary>
		/// <param name="obj">Экземпляр исследуемого объекта.</param>
		/// <param name="name">Строковое имя свойства.</param>
		/// <param name="type">Тип данных, в рамках которого производится поиск метаданных свойства.</param>
		/// <returns>Объект, содержащий значение свойства.</returns>
		/// <exception cref="InvalidOperationException">Выбрасывается, если свойство с указанным именем не найдено в типе <paramref name="type"/>.</exception>
		public static object? GetPropertyValue(
			this object obj,
			string name,
			Type type)
		{
			var prop1 = type.GetProperty(name)
				?? throw new InvalidOperationException(
					$"The property '{name}' was not found in the type '{type.Name}'.");
			return prop1.GetValue(obj, null);
		}


		/// <summary>
		/// Возвращает значение свойства объекта по его строковому имени на основе фактического рантайм-типа объекта.
		/// </summary>
		/// <param name="obj">Экземпляр исследуемого объекта.</param>
		/// <param name="name">Строковое имя свойства.</param>
		/// <returns>Объект, содержащий значение указанного свойства.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object? GetPropertyValue(
			this object obj,
			string name)
		{
			return obj.GetPropertyValue(name, obj.GetType());
		}


		/// <summary>
		/// Возвращает типизированное значение свойства объекта по его имени на основе указанного базового типа метаданных.
		/// </summary>
		/// <typeparam name="T">Целевой тип, к которому приводится значение свойства.</typeparam>
		/// <param name="obj">Экземпляр исследуемого объекта.</param>
		/// <param name="name">Строковое имя свойства.</param>
		/// <param name="type">Тип данных для сканирования метаданных свойства.</param>
		/// <returns>Значение свойства, приведенное к типу <typeparamref name="T"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetPropertyValue<T>(
			this object obj,
			string name,
			Type type)
		{
			return (T)obj.GetPropertyValue(name, type)!;
		}


		/// <summary>
		/// Возвращает типизированное значение свойства объекта по его строковому имени на основе рантайм-типа объекта.
		/// </summary>
		/// <typeparam name="T">Целевой тип, к которому приводится значение свойства.</typeparam>
		/// <param name="obj">Экземпляр исследуемого объекта.</param>
		/// <param name="name">Строковое имя свойства.</param>
		/// <returns>Значение свойства, приведенное к типу <typeparamref name="T"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetPropertyValue<T>(
			this object obj,
			string name)
		{
			return (T)obj.GetPropertyValue(name)!;
		}


		/// <summary>
		/// Возвращает исходный объект, приведенный к типу <typeparamref name="T"/>, либо кастомное значение по умолчанию, если проверяемый объект равен <see langword="null"/>.
		/// </summary>
		/// <typeparam name="T">Целевой тип результирующего объекта.</typeparam>
		/// <param name="value">Проверяемый сырой объект. Допускает значение <see langword="null"/>.</param>
		/// <param name="defaultValue">Альтернативное возвращаемое значение по умолчанию.</param>
		/// <returns>Объект, приведенный к типу <typeparamref name="T"/>, или значение <paramref name="defaultValue"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T DefaultObject<T>(
			this object? value,
			T defaultValue)
		{
			if (value == null)
				return defaultValue;
			return (T)value;
		}


		/// <summary>
		/// Возвращает исходный объект, приведенный к типу <typeparamref name="T"/>, либо базовое системное значение по умолчанию типа <c>default(T)</c>, если исходный объект равен <see langword="null"/>.
		/// </summary>
		/// <typeparam name="T">Целевой тип результирующего объекта.</typeparam>
		/// <param name="value">Проверяемый сырой объект. Допускает значение <see langword="null"/>.</param>
		/// <returns>Объект типа <typeparamref name="T"/> или системное дефолтное значение.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T? DefaultObject<T>(
			this object? value)
		{
			return value.DefaultObject(default(T)!);
		}

	}

}
