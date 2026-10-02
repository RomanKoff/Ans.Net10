// rev 2026-10-02

using Ans.Net10.Common;

namespace Ans.Net10.Web.Forms
{

	/// <summary>
	/// Базовый класс для компонентов просмотра, отображающих текстовые данные 
	/// с автоматической генерацией скрытого поля для сохранения состояния модели.
	/// </summary>
	public abstract class _View_Text_Base
		: IFormViewControl
	{
		private readonly InputHiddenTag _hidden;

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_View_Text_Base"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="valueText">Текстовое представление значения для вывода на экран. Допускает значение <see langword="null"/>.</param>
		/// <param name="valueData">Оригинальный объект данных для сериализации в скрытое поле формы. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Ограничение максимальной ширины текстового блока в rem единицах.</param>
		/// <param name="useRaw">Флаг вывода текста в сыром виде (без экранирования HTML-сущностей).</param>
		public _View_Text_Base(
			string name,
			string? valueText,
			object? valueData,
			int maxWidth,
			bool useRaw)
		{
			Name = name;
			Value = valueText ?? string.Empty;
			ValueData = SuppValues.GetStringForWeb(valueData) ?? Value;
			MaxWidth = maxWidth;
			UseRaw = useRaw;
			var safeText1 = useRaw ? Value : (SuppTypograph.GetText2Html(Value) ?? string.Empty);
			Control = new DivTag(safeText1);
			Control.AddCssClass("form-control bg-light text-dark");
			if (MaxWidth > 0)
				Control.ExpandStyleAttribute($"max-width:{MaxWidth}rem;");
			_hidden = new InputHiddenTag(Name, ValueData);
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>
		/// Получает текстовое представление отображаемого значения.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Получает строковое сериализованное значение скрытого поля.
		/// </summary>
		public string ValueData { get; }

		/// <summary>
		/// Получает ограничение максимальной ширины блока.
		/// </summary>
		public int MaxWidth { get; }

		/// <summary>
		/// Получает признак вывода текста без экранирования.
		/// </summary>
		public bool UseRaw { get; }

		/// <summary>
		/// Получает сгенерированный HTML-компонент контейнера текста.
		/// </summary>
		public DivTag Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"{Control}{_hidden}";
	}



	/// <summary>
	/// Базовый класс для компонентов просмотра внешних связей и справочников.
	/// </summary>
	public abstract class _View_Reference_Base
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_View_Reference_Base"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Ключ элемента справочника. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>, содержащий текстовые значения.</param>
		public _View_Reference_Base(
			string name,
			string? value,
			RegistryList registry)
			: base(
				  name,
				  value is null
					? null
					: registry.GetValueOrKey(value),
				  value,
				  registry.GetMaxWidth(),
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра логических состояний в виде локализованной текстовой метки Да/Нет.
	/// </summary>
	public class View_Bool
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Bool"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее логическое значение флага.</param>
		public View_Bool(
			string name,
			bool value)
			: base(
				  name,
				  value.Make(
					  Resources.Common.Html_ViewTrue ?? "Да",
					  Resources.Common.Html_ViewFalse ?? "Нет"),
				  value,
				  SuppForms.MW_Bool,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра календарной даты без временной составляющей.
	/// </summary>
	public class View_DateOnly
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_DateOnly"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее значение календарной даты. Допускает значение <see langword="null"/>.</param>
		public View_DateOnly(
			string name,
			DateOnly? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_DateOnly,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра даты и точного времени в стандартном коротком формате.
	/// </summary>
	public class View_DateTime
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_DateTime"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее значение даты и времени. Допускает значение <see langword="null"/>.</param>
		public View_DateTime(
			string name,
			DateTime? value)
			: base(
				  name,
				  value?.ToString("g"),
				  value,
				  SuppForms.MW_DateTime,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра точного времени суток без привязки к календарной дате.
	/// </summary>
	public class View_TimeOnly
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_TimeOnly"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее значение времени суток. Допускает значение <see langword="null"/>.</param>
		public View_TimeOnly(
			string name,
			TimeOnly? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_TimeOnly,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра высокоточных десятичных финансовых чисел.
	/// </summary>
	public class View_Decimal
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Decimal"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее десятичное число. Допускает значение <see langword="null"/>.</param>
		public View_Decimal(
			string name,
			decimal? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_Decimal,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра числовых значений с плавающей запятой двойной точности.
	/// </summary>
	public class View_Double
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Double"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее числовое значение. Допускает значение <see langword="null"/>.</param>
		public View_Double(
			string name,
			double? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_Double,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра числовых значений с плавающей запятой одинарной точности.
	/// </summary>
	public class View_Float
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Float"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее числовое значение. Допускает значение <see langword="null"/>.</param>
		public View_Float(
			string name,
			float? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_Float,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра стандартных 32-битных целых чисел.
	/// </summary>
	public class View_Int
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Int"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее целое число. Допускает значение <see langword="null"/>.</param>
		public View_Int(
			string name,
			int? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_Int,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра больших 64-битных целых чисел.
	/// </summary>
	public class View_Long
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Long"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее большое целое число. Допускает значение <see langword="null"/>.</param>
		public View_Long(
			string name,
			long? value)
			: base(
				  name,
				  value?.ToString(),
				  value,
				  SuppForms.MW_Long,
				  true)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра значения перечисления из справочника по числовому коду.
	/// </summary>
	public class View_Enum
		: _View_Reference_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Enum"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Числовой код выбранного перечисления.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>.</param>
		public View_Enum(
			string name,
			int value,
			RegistryList registry)
			: base(
				  name,
				  value.ToString(),
				  registry)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра значения перечисления из справочника по строковому ключу.
	/// </summary>
	public class View_EnumString
		: _View_Reference_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_EnumString"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Строковый ключ выбранного перечисления. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>.</param>
		public View_EnumString(
			string name,
			string? value,
			RegistryList registry)
			: base(
				  name,
				  value ?? string.Empty,
				  registry)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра текстового названия внешней связанной сущности (внешнего ключа).
	/// </summary>
	public class View_Reference
		: _View_Reference_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Reference"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Идентификатор связи. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник связанных сущностей <see cref="RegistryList"/>.</param>
		public View_Reference(
			string name,
			int? value,
			RegistryList registry)
			: base(
				  name,
				  value?.ToString(),
				  registry)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра набора выбранных флагов, генерирующий визуальные значки 
	/// элементов и группу скрытых инпутов для передачи коллекции.
	/// </summary>
	public class View_Set
		: IFormViewControl
	{
		private readonly HiddenInputsHtml _hidden;

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Set"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Массив выбранных числовых идентификаторов. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник доступных флагов и групп <see cref="RegistryList"/>.</param>
		public View_Set(
			string name,
			int[]? value,
			RegistryList registry)
		{
			Name = name;
			Value = value != null && value.Length > 0
				? value.MakeFromCollection(
					x => registry.GetValue(x) ?? x.ToString(),
					"<div class=\"d-flex flex-wrap gap-1 lh-sm\">{0}</div>",
					"<div class=\"px-2 py-1 text-dark bg-dark-subtle rounded\">{0}</div>",
					null)
				: $"<div class=\"opacity-50\">{Common.Resources.Common.Text_EmptyItem ?? "—"}</div>";
			ValueData = value != null
				? [.. value.Select(x => x.ToString())]
				: [];
			Control = new DivTag(Value);
			_hidden = new HiddenInputsHtml(Name, ValueData);
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>Получает HTML-разметку отображаемого списка значков.</summary>
		public string Value { get; }

		/// <summary>Получает массив строковых ключей для скрытых полей.</summary>
		public string[] ValueData { get; }

		/// <summary>Получает сгенерированный HTML-компонент контейнера значков.</summary>
		public DivTag Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"{Control}{_hidden}";
	}



	/// <summary>
	/// Компонент просмотра адресов электронной почты.
	/// </summary>
	public class View_Email
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Email"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Значение адреса электронной почты. Допускает значение <see langword="null"/>.</param>
		public View_Email(
			string name,
			string? value)
			: base(
				  name,
				  value,
				  null,
				  SuppForms.MW_Email,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра многострочных блоков текста (Memo) без принудительного ограничения ширины.
	/// </summary>
	public class View_Memo
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Memo"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Значение многострочного текста. Допускает значение <see langword="null"/>.</param>
		public View_Memo(
			string name,
			string? value)
			: base(
				  name,
				  value,
				  null,
				  0,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра стандартных имен и заголовков объектов.
	/// </summary>
	public class View_Name
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Name"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Значение имени или названия. Допускает значение <see langword="null"/>.</param>
		public View_Name(
			string name,
			string? value)
			: base(
				  name,
				  value,
				  null,
				  SuppForms.MW_Name,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра системных имен переменных в C#-стиле.
	/// </summary>
	public class View_Varname
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Varname"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Значение имени переменной. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Кастомное ограничение максимальной ширины блока. Если <see langword="null"/>, применяется дефолтный лимит.</param>
		public View_Varname(
			string name,
			string? value,
			int? maxWidth = null)
			: base(
				  name,
				  value,
				  null,
				  maxWidth ?? SuppForms.MW_Varname,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра ультракоротких строк (до 50 символов).
	/// </summary>
	public class View_Text50
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Text50"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строковое текстовое значение. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Кастомное ограничение максимальной ширины блока. Если <see langword="null"/>, применяется дефолтный лимит.</param>
		public View_Text50(
			string name,
			string? value,
			int? maxWidth = null)
			: base(
				  name,
				  value,
				  null,
				  maxWidth ?? SuppForms.MW_Text50,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра коротких строк (до 100 символов).
	/// </summary>
	public class View_Text100
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Text100"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строковое текстовое значение. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Кастомное ограничение максимальной ширины блока. Если <see langword="null"/>, применяется дефолтный лимит.</param>
		public View_Text100(
			string name,
			string? value,
			int? maxWidth = null)
			: base(
				  name,
				  value,
				  null,
				  maxWidth ?? SuppForms.MW_Text100,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра строк средней длины (до 250 символов).
	/// </summary>
	public class View_Text250
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Text250"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строковое текстовое значение. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Кастомное ограничение максимальной ширины блока. Если <see langword="null"/>, применяется дефолтный лимит.</param>
		public View_Text250(
			string name,
			string? value,
			int? maxWidth = null)
			: base(
				  name,
				  value,
				  null,
				  maxWidth ?? SuppForms.MW_Text250,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент просмотра расширенных длинных строк (до 400 символов).
	/// </summary>
	public class View_Text400
		: _View_Text_Base,
		IFormViewControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="View_Text400"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строковое текстовое значение. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Кастомное ограничение максимальной ширины блока. Если <see langword="null"/>, применяется дефолтный лимит.</param>
		public View_Text400(
			string name,
			string? value,
			int? maxWidth = null)
			: base(
				  name,
				  value,
				  null,
				  maxWidth ?? SuppForms.MW_Text400,
				  false)
		{
		}
	}

}
