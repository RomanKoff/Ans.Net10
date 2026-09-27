// rev 2026-09-25

using System.Reflection;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Базовый класс для описания общих элементов структурированных метаданных объекта.
	/// </summary>
	public class _ObjInfo_Base
	{
		/// <summary>
		/// Получает или задает системное программное имя элемента (свойства, метода, конструктора).
		/// </summary>
		/// <value>Строковое значение имени члена типа.</value>
		public string Name { get; set; } = string.Empty;
	}



	/// <summary>
	/// Описание структурированных метаданных свойства объекта.
	/// </summary>
	public class ObjInfoProperty
		: _ObjInfo_Base
	{
		/// <summary>
		/// Получает или задает признак наличия открытого метода чтения (getter) у свойства.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если свойство поддерживает чтение; в противном случае — <see langword="false"/>.</value>
		public bool HasGetter { get; set; }

		/// <summary>
		/// Получает или задает признак наличия открытого метода записи (setter) у свойства.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если свойство поддерживает запись; в противном случае — <see langword="false"/>.</value>
		public bool HasSetter { get; set; }

		/// <summary>
		/// Получает или задает удобочитаемое имя типа данных свойства, отформатированное в соответствии с синтаксисом языка C#.
		/// </summary>
		/// <value>Строка с типом данных (например, <c>"int?"</c>, <c>"string"</c>, <c>"List&lt;DateTime&gt;"</c>).</value>
		public string TypeName { get; set; } = string.Empty;

		/// <summary>
		/// Получает или задает строковое представление текущего фактического значения свойства живого объекта.
		/// </summary>
		/// <value>Текстовое значение свойства или <see langword="null"/>, если исследуемый объект не был передан или значение равно null.</value>
		public string? Value { get; set; }
	}



	/// <summary>
	/// Описание структурированных метаданных метода или конструктора исследуемого объекта.
	/// </summary>
	public class ObjInfoMethod
		: _ObjInfo_Base
	{
		/// <summary>
		/// Получает или задает строковое представление объявлений обобщенных параметров типов (Generics) метода.
		/// </summary>
		/// <value>Строка параметров типов, заключенная в угловые скобки (например, <c>"&lt;T, TResult&gt;"</c>), или <see langword="null"/>, если метод не является универсальным.</value>
		public string? Generics { get; set; }

		/// <summary>
		/// Получает или задает коллекцию строковых объявлений входных параметров метода с указанием их типов и имен в синтаксисе C#.
		/// </summary>
		/// <value>Перечисление <see cref="IEnumerable{String}"/>, содержащее сигнатуры параметров.</value>
		public IEnumerable<string> Parameters { get; set; } = [];
	}



	/// <summary>
	/// Описание структурированных метаданных метода-функции, возвращающей значение.
	/// </summary>
	public class ObjInfoFunction
		: ObjInfoMethod
	{
		/// <summary>
		/// Получает или задает удобочитаемое имя возвращаемого типа данных функции в каноническом синтаксисе языка C#.
		/// </summary>
		/// <value>Строка с типом возвращаемого значения (например, <c>"Task&lt;bool&gt;"</c>).</value>
		public string Return { get; set; } = string.Empty;
	}



	/// <summary>
	/// Строитель и высокоуровневый контейнер структурированных метаданных типов и живых объектов, 
	/// автоматизирующий извлечение сигнатур в синтаксические конструкции C#.
	/// </summary>
	public class ObjInfoBuilder
	{

		/* ctor */

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ObjInfoBuilder"/> для указанного типа данных 
		/// и опционально извлекает строковые значения свойств из переданного живого экземпляра.
		/// </summary>
		/// <remarks>
		/// Внутренний парсер автоматически исключает из анализа базовые методы, унаследованные от системного типа <see cref="object"/>. 
		/// Для получения C#-имен типов и значений привлекаются расширения рефлексии <c>GetPropertyValue</c> и <c>GetCSharpValue</c>.
		/// </remarks>
		/// <param name="type">Исследуемый тип данных метаданных <see cref="Type"/>.</param>
		/// <param name="obj">Опциональный экземпляр живого объекта исследуемого типа для считывания текущих значений свойств. Если равен <see langword="null"/>, значения свойств останутся пустыми.</param>
		public ObjInfoBuilder(
			Type type,
			object? obj = null)
		{
			InfoType = type;
			_parse();
			if (obj != null)
			{
				foreach (var prop1 in ReadWriteProperties)
					prop1.Value = SuppReflection.GetCSharpValue(obj.GetPropertyValue(prop1.Name));
				foreach (var prop1 in ReadOnlyProperties)
					prop1.Value = SuppReflection.GetCSharpValue(obj.GetPropertyValue(prop1.Name));
			}
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает ссылку на исследуемый тип данных.
		/// </summary>
		/// <value>Экземпляр описания типа <see cref="Type"/>.</value>
		public Type InfoType { get; }


		/// <summary>
		/// Возвращает коллекцию метаданных публичных конструкторов исследуемого типа.
		/// </summary>
		/// <value>Неизменяемая коллекция <see cref="IReadOnlyCollection{ObjInfoMethod}"/>.</value>
		public IReadOnlyCollection<ObjInfoMethod> Ctors { get; private set; } = [];


		/// <summary>
		/// Возвращает коллекцию метаданных свойств, одновременно доступных и на чтение, и на запись (ReadWrite).
		/// </summary>
		/// <value>Неизменяемая коллекция <see cref="IReadOnlyCollection{ObjInfoProperty}"/>.</value>
		public IReadOnlyCollection<ObjInfoProperty> ReadWriteProperties { get; private set; } = [];


		/// <summary>
		/// Возвращает коллекцию метаданных свойств, доступных исключительно для чтения (ReadOnly).
		/// </summary>
		/// <value>Неизменяемая коллекция <see cref="IReadOnlyCollection{ObjInfoProperty}"/>.</value>
		public IReadOnlyCollection<ObjInfoProperty> ReadOnlyProperties { get; private set; } = [];


		/// <summary>
		/// Возвращает коллекцию метаданных свойств, доступных исключительно для записи (WriteOnly).
		/// </summary>
		/// <value>Неизменяемая коллекция <see cref="IReadOnlyCollection{ObjInfoProperty}"/>.</value>
		public IReadOnlyCollection<ObjInfoProperty> WriteOnlyProperties { get; private set; } = [];


		/// <summary>
		/// Возвращает коллекцию метаданных методов-функций, возвращающих конкретные типы данных (не void).
		/// </summary>
		/// <value>Неизменяемая коллекция <see cref="IReadOnlyCollection{ObjInfoFunction}"/>.</value>
		public IReadOnlyCollection<ObjInfoFunction> Functions { get; private set; } = [];


		/// <summary>
		/// Возвращает коллекцию метаданных стандартных процедурных методов, не возвращающих значение (<c>void</c>).
		/// </summary>
		/// <value>Неизменяемая коллекция <see cref="IReadOnlyCollection{ObjInfoMethod}"/>.</value>
		public IReadOnlyCollection<ObjInfoMethod> Methods { get; private set; } = [];


		/// <summary>
		/// Возвращает признак наличия у исследуемого типа свойств, доступных и на чтение, и на запись.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если коллекция <see cref="ReadWriteProperties"/> содержит элементы; в противном случае — <see langword="false"/>.</value>
		public bool HasReadWriteProperties
			=> ReadWriteProperties.Count > 0;


		/// <summary>
		/// Возвращает признак наличия у исследуемого типа свойств, доступных только для чтения.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если коллекция <see cref="ReadOnlyProperties"/> содержит элементы; в противном случае — <see langword="false"/>.</value>
		public bool HasReadonlyProperties
			=> ReadOnlyProperties.Count > 0;


		/// <summary>
		/// Возвращает признак наличия у исследуемого типа свойств, доступных только для записи.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если коллекция <see cref="WriteOnlyProperties"/> содержит элементы; в противном случае — <see langword="false"/>.</value>
		public bool HasWriteonlyProperties
			=> WriteOnlyProperties.Count > 0;


		/// <summary>
		/// Возвращает интегральный признак наличия абсолютно любых публичных свойств у исследуемого типа данных.
		/// </summary>
		/// <value>Значение <see langword="true"/>, если тип содержит хотя бы одно свойство в любой из категорий доступа; иначе — <see langword="false"/>.</value>
		public bool HasProperties
			=> HasReadWriteProperties
				|| HasReadonlyProperties
				|| HasWriteonlyProperties;


		/* privates */


		private void _parse()
		{
			var objectType1 = typeof(object);

			// Парсинг конструкторов
			Ctors = [.. InfoType.GetConstructors()
				.Select(x => new ObjInfoMethod
				{
					Name = x.Name,
					Parameters = x.GetCSharpParams()
				})];

			// Получение всех методов, кроме унаследованных от System.Object
			var methods = InfoType.GetMethods()
				.Where(x => x.DeclaringType != objectType1);

			var getters1 = new List<MethodInfo>();
			var setters1 = new List<MethodInfo>();
			var funcs1 = new List<MethodInfo>();
			var meths1 = new List<MethodInfo>();

			foreach (var item1 in methods)
			{
				if (item1.Name.StartsWith("get_"))
					getters1.Add(item1);
				else if (item1.Name.StartsWith("set_"))
					setters1.Add(item1);
				else if (item1.ReturnType == typeof(void))
					meths1.Add(item1);
				else
					funcs1.Add(item1);
			}

			var propsDict1 = new Dictionary<string, ObjInfoProperty>();

			foreach (var item1 in getters1)
			{
				var prop1 = _makeProp(item1, isGetter: true);
				prop1.HasGetter = true;
				propsDict1[prop1.Name] = prop1;
			}

			foreach (var item1 in setters1)
			{
				var name1 = item1.GetPropertyName();
				if (propsDict1.TryGetValue(name1, out var existingProp1))
					existingProp1.HasSetter = true;
				else
				{
					var prop1 = _makeProp(item1, isGetter: false, name1);
					prop1.HasSetter = true;
					propsDict1[name1] = prop1;
				}
			}

			var allProperties1 = propsDict1.Values;
			ReadWriteProperties = [.. allProperties1.Where(x => x.HasGetter && x.HasSetter)];
			ReadOnlyProperties = [.. allProperties1.Where(x => !x.HasSetter)];
			WriteOnlyProperties = [.. allProperties1.Where(x => !x.HasGetter)];

			// Парсинг функций
			Functions = [.. funcs1.Select(x => new ObjInfoFunction
			{
				Return = x.ReturnType.GetCSharpTypeName(),
				Name = x.Name,
				Generics = x.GetCSharpGenerics(),
				Parameters = x.GetCSharpParams()
			})];

			// Парсинг void-методов
			Methods = [.. meths1.Select(x => new ObjInfoMethod
			{
				Name = x.Name,
				Generics = x.GetCSharpGenerics(),
				Parameters = x.GetCSharpParams()
			})];
		}


		private static ObjInfoProperty _makeProp(
			MethodInfo info,
			bool isGetter,
			string? name = null)
		{
			var propertyType1 = isGetter
				? info.ReturnType
				: info.GetParameters()[0].ParameterType;
			return new ObjInfoProperty
			{
				Name = name ?? info.GetPropertyName(),
				TypeName = propertyType1.GetCSharpTypeName(),
				Value = null
			};
		}

	}

}
