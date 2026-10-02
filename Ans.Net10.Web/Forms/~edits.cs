// rev 2026-10-02

using Ans.Net10.Common;

namespace Ans.Net10.Web.Forms
{

	/// <summary>
	/// Базовый класс для компонентов редактирования, работающих со справочниками и реестрами данных.
	/// </summary>
	/// <remarks>
	/// Автоматически определяет оптимальный режим отображения (выпадающий список или группа переключателей) 
	/// на основе плотности данных справочника.
	/// </remarks>
	public abstract class _Edit_Registry_Base
		: IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_Edit_Registry_Base"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее сохраненное строковое значение (или список ключей через запятую). Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>, содержащий доступные варианты выбора.</param>
		/// <param name="mode">Режим визуального рендеринга компонента (выпадающий список, чекбоксы/радиокнопки или автовыбор).</param>
		/// <param name="cssClasses">Дополнительные CSS-классы для кастомизации внешнего вида элемента. Допускает значение <see langword="null"/>.</param>
		/// <param name="isMultiple">Флаг, указывающий на поддержку множественного выбора элементов (чекбоксы вместо радиокнопок).</param>
		public _Edit_Registry_Base(
			string name,
			string? value,
			RegistryList registry,
			RegistryModeEnum mode,
			string? cssClasses,
			bool isMultiple)
		{
			Name = name;
			Value = value;
			Registry = registry;
			Mode = mode == RegistryModeEnum.Auto
				? registry.GetProposeMode()
				: mode;
			CssClasses = cssClasses ?? string.Empty;
			IsMultiple = isMultiple;
			Control = GetControl();
			if (cssClasses != null)
				Control.AddCssClass(cssClasses);
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>
		/// Получает текущее строковое значение поля.
		/// </summary>
		public string? Value { get; }

		/// <summary>
		/// Получает привязанный справочник элементов.
		/// </summary>
		public RegistryList Registry { get; }

		/// <summary>
		/// Получает рассчитанный или явно заданный режим визуального рендеринга.
		/// </summary>
		public RegistryModeEnum Mode { get; }

		/// <summary>
		/// Получает строку кастомных CSS-классов
		/// .</summary>
		public string CssClasses { get; }

		/// <summary>
		/// Получает сгенерированный расширенный тег компонента ввода.
		/// </summary>
		public TagBuilderExt Control { get; }

		/// <summary>
		/// Получает флаг поддержки множественного выбора.
		/// </summary>
		public bool IsMultiple { get; }

		/// <inheritdoc />
		public override string ToString()
		{
			return Mode == RegistryModeEnum.Select
				? $"<div class=\"p-0\" style=\"max-width:{Registry.GetMaxWidth()}rem;\">{Control}</div>"
				: $"{Control}";
		}

		/// <summary>
		/// Конструирует и настраивает объект управления <see cref="TagBuilderExt"/> в зависимости от выбранного режима.
		/// </summary>
		/// <returns>Инициализированный объект тега компонента.</returns>
		public TagBuilderExt GetControl()
		{
			TagBuilderExt ctrl1;
			var parts1 = Value?.Split(SuppForms.SEP_ITEMS, StringSplitOptions.RemoveEmptyEntries);
			switch (Mode)
			{
				case RegistryModeEnum.Inputs:
					ctrl1 = new SelectInputsHtml(Name, parts1, Registry, IsMultiple);
					break;
				default:
					ctrl1 = new SelectTag(Name, parts1, Registry, IsMultiple);
					ctrl1.AddCssClass("form-select tom-select");
					break;
			}
			if (Value != null)
				ctrl1.MergeAttribute("data-value", Value);
			return ctrl1;
		}
	}



	/// <summary>
	/// Базовый класс для стандартных текстовых полей ввода формы.
	/// </summary>
	public abstract class _Edit_Text_Base
		: IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_Edit_Text_Base"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы, соответствующее свойству модели.</param>
		/// <param name="value">Текущее строковое текстовое значение инпута. Допускает значение <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы для стилизации HTML-тега. Допускает значение <see langword="null"/>.</param>
		/// <param name="maxWidth">Ограничение максимальной ширины блока в rem единицах (на основе констант геометрии форм).</param>
		public _Edit_Text_Base(
			string name,
			string? value,
			string? cssClasses,
			int maxWidth)
		{
			Name = name;
			Value = value;
			MaxWidth = maxWidth;
			Control = new(Name, Value);
			Control.AddCssClass("form-control");
			if (cssClasses != null)
				Control.AddCssClass(cssClasses);
			if (MaxWidth > 0)
				Control.ExpandStyleAttribute($"max-width:{MaxWidth}rem;");
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>Получает текущее текстовое значение поля.</summary>
		public string? Value { get; }

		/// <summary>Получает ограничение максимальной ширины блока.</summary>
		public int MaxWidth { get; }

		/// <summary>Получает объект стандартного текстового тега ввода.</summary>
		public InputTextTag Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"{Control}";
	}



	/// <summary>
	/// Компонент формы, позволяющий внедрить произвольную кастомную HTML-разметку в качестве поля ввода.
	/// </summary>
	public class Edit__Custom
		: IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit__Custom"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="control">Сырая HTML-строка кастомного элемента управления со всей необходимой разметкой.</param>
		public Edit__Custom(
			string name,
			string control)
		{
			Name = name;
			Control = control;
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>Получает сырую HTML-разметку кастомного элемента.</summary>
		public string Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"{Control}";
	}



	/// <summary>
	/// Компонент формы, принудительно использующий классический выпадающий список (Dropdown/Select) для выбора элемента справочника.
	/// </summary>
	public class Edit__Select
		: _Edit_Registry_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit__Select"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Числовой идентификатор выбранного значения. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit__Select(
			string name,
			int? value,
			RegistryList registry,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  registry,
				  RegistryModeEnum.Select,
				  cssClasses,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент формы для редактирования логических состояний в виде одиночного Bootstrap-чебокса.
	/// </summary>
	public class Edit_Bool
		: IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Bool"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее логическое состояние переключателя (активен/деактивирован).</param>
		/// <param name="title">Отображаемый текст подписи рядом с флажком. Если не задан, берется стандартный текст "Да". Допускает значение <see langword="null"/>.</param>
		public Edit_Bool(
			string name,
			bool value,
			string? title = null)
		{
			Name = name;
			Value = value;
			Title = title ?? Resources.Common.Html_EditChecked;
			Control = new(Name, Name, true.ToString(), Title, true, Value);
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>Получает логическое значение переключателя.</summary>
		public bool Value { get; }

		/// <summary>Получает текст подписи флажка.</summary>
		public string Title { get; }

		/// <summary>Получает сгенерированный Bootstrap-компонент флажка.</summary>
		public CheckboxHtml Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"<div>{Control}</div>";
	}



	/// <summary>
	/// Компонент формы для организации поля загрузки локальных файлов с диска пользователя.
	/// </summary>
	public class Edit_FileUpload
		: IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_FileUpload"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		public Edit_FileUpload(
			string name)
		{
			Name = name;
			Control = new(Name);
			Control.AddCssClass("form-control");
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>Получает объект HTML-тега выбора файла.</summary>
		public InputFileTag Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"{Control}";
	}



	/// <summary>
	/// Компонент формы для редактирования перечислений на основе числовых кодов справочника.
	/// </summary>
	public class Edit_Enum
		: _Edit_Registry_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Enum"/> с указанием явного режима рендеринга.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее числовое значение перечисления.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>.</param>
		/// <param name="mode">Явно заданный режим визуального отображения.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit_Enum(
			string name,
			int value,
			RegistryList registry,
			RegistryModeEnum mode,
			string? cssClasses)
			: base(
				  name,
				  value.ToString(),
				  registry,
				  mode,
				  cssClasses,
				  false)
		{
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Enum"/> с автоматическим расчетом режима рендеринга.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее числовое значение перечисления.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit_Enum(
			string name,
			int value,
			RegistryList registry,
			string? cssClasses = null)
			: this(
				  name,
				  value,
				  registry,
				  RegistryModeEnum.Auto,
				  cssClasses)
		{
		}
	}



	/// <summary>
	/// Компонент формы для редактирования перечислений на основе строковых ключей справочника.
	/// </summary>
	public class Edit_EnumString
		: _Edit_Registry_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_EnumString"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее строковое значение перечисления. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Справочник элементов <see cref="RegistryList"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit_EnumString(
			string name,
			string? value,
			RegistryList registry,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  registry,
				  RegistryModeEnum.Auto,
				  cssClasses,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент формы для управления ссылками внешних ключей (взаимосвязей объектов) на основе реестра сущностей.
	/// </summary>
	public class Edit_Reference
		: _Edit_Registry_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Reference"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Идентификатор связи (внешний ключ). Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Реестр связанных сущностей <see cref="RegistryList"/>.</param>
		/// <param name="registryMode">Режим визуального отображения компонента.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit_Reference(
			string name,
			int? value,
			RegistryList registry,
			RegistryModeEnum registryMode = RegistryModeEnum.Auto,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  registry,
				  registryMode,
				  cssClasses,
				  false)
		{
		}
	}



	/// <summary>
	/// Компонент формы для редактирования массивов и наборов флагов (связей многие-ко-многим).
	/// </summary>
	public class Edit_Set
		: _Edit_Registry_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Set"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Массив выбранных целочисленных ключей флагов. Допускает значение <see langword="null"/>.</param>
		/// <param name="registry">Полный справочник доступных флагов.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit_Set(
			string name,
			int[]? value,
			RegistryList registry,
			string? cssClasses = null)
			: base(
				  name,
				  value.MakeFromCollection(x => x.ToString(), null, null, ","),
				  registry,
				  RegistryModeEnum.Auto,
				  cssClasses, true)
		{
		}
	}



	/// <summary>
	/// Компонент формы для редактирования расширенных многострочных текстовых блоков (Memo/TextArea).
	/// </summary>
	public class Edit_Memo
		: IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Memo"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее значение многострочного текста. Допускает значение <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает значение <see langword="null"/>.</param>
		public Edit_Memo(
			string name,
			string? value,
			string? cssClasses = null)
		{
			Name = name;
			Value = value;
			Control = new(Name, Value);
			Control.AddCssClass("form-control");
			Control.ExpandStyleAttribute("border:none;");
			if (cssClasses != null)
				Control.AddCssClass(cssClasses);
			Control.MergeAttribute("rows", "6");
		}

		/// <inheritdoc />
		public string Name { get; }

		/// <summary>Получает текущее значение многострочного текста.</summary>
		public string? Value { get; }

		/// <summary>Получает объект HTML-тега текстовой области.</summary>
		public TextareaTag Control { get; }

		/// <inheritdoc />
		public override string ToString()
			=> $"<div class=\"form-control p-0\">{Control}</div>";
	}



	/// <summary>
	/// Компонент формы для ввода календарной даты без временной составляющей.
	/// </summary>
	public class Edit_DateOnly
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_DateOnly"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее календарное значение даты. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_DateOnly(
			string name,
			DateOnly? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_DateOnly)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода даты и точного времени.
	/// </summary>
	public class Edit_DateTime
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_DateTime"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее значение даты и времени. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_DateTime(
			string name,
			DateTime? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString("g"),
				  cssClasses,
				  SuppForms.MW_DateTime)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода точного времени суток без привязки к дате.
	/// </summary>
	public class Edit_TimeOnly
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_TimeOnly"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее значение времени суток. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_TimeOnly(
			string name,
			TimeOnly? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_TimeOnly)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода фиксированных десятичных финансовых чисел.
	/// </summary>
	public class Edit_Decimal
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Decimal"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее числовое значение. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Decimal(
			string name,
			decimal? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_Decimal)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода чисел с плавающей запятой двойной точности.
	/// </summary>
	public class Edit_Double
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Double"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее числовое значение. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Double(
			string name,
			double? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_Double)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода чисел с плавающей запятой одинарной точности.
	/// </summary>
	public class Edit_Float
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Float"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее числовое значение. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Float(
			string name,
			float? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_Float)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода стандартных 32-битных целых чисел.
	/// </summary>
	public class Edit_Int
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Int"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее целое число. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Int(
			string name,
			int? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_Int)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода больших 64-битных целых чисел.
	/// </summary>
	public class Edit_Long
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Long"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текущее целое число. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Long(
			string name,
			long? value,
			string? cssClasses = null)
			: base(
				  name,
				  value?.ToString(),
				  cssClasses,
				  SuppForms.MW_Long)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода адресов электронной почты.
	/// </summary>
	public class Edit_Email
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Email"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текстовая строка email адреса. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Email(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Email)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода стандартных имен и заголовков объектов.
	/// </summary>
	public class Edit_Name
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Name"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Текстовое значение названия. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Name(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Name)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода системных наименований переменных.
	/// </summary>
	public class Edit_Varname
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Varname"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строка с именем переменной. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Varname(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Varname)
		{
		}
	}



	/// <summary>Компонент формы для ввода ультракоротких текстовых строк (до 50 символов).</summary>
	public class Edit_Text50
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Text50"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строка текста. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Text50(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Text50)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода коротких текстовых строк (до 100 символов).
	/// </summary>
	public class Edit_Text100
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Text100"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строка текста. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Text100(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Text100)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода текстовых строк средней длины (до 250 символов).
	/// </summary>
	public class Edit_Text250
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Text250"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строка текста. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Text250(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Text250)
		{
		}
	}



	/// <summary>
	/// Компонент формы для ввода длинных текстовых строк (до 400 символов).
	/// </summary>
	public class Edit_Text400
		: _Edit_Text_Base,
		IFormEditControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Edit_Text400"/>.
		/// </summary>
		/// <param name="name">Системное программное имя поля формы.</param>
		/// <param name="value">Строка текста. Допускает <see langword="null"/>.</param>
		/// <param name="cssClasses">Дополнительные CSS-классы. Допускает <see langword="null"/>.</param>
		public Edit_Text400(
			string name,
			string? value,
			string? cssClasses = null)
			: base(
				  name,
				  value,
				  cssClasses,
				  SuppForms.MW_Text400)
		{
		}
	}

}
