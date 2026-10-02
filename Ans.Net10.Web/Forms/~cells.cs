// rev 2026-10-02

using Ans.Net10.Common;

namespace Ans.Net10.Web.Forms
{

	/// <summary>
	/// Базовый класс для компонентов ячеек форм, выполняющий 
	/// вывод текстовых данных с поддержкой автотипографики и экранирования.
	/// </summary>
	public abstract class _Cell_Text_Base
		: IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="_Cell_Text_Base"/>.
		/// </summary>
		/// <param name="value">Текстовое значение для вывода в ячейку. Допускает значение <see langword="null"/>.</param>
		/// <param name="useRaw">Признак вывода строки в сыром виде (без экранирования HTML-сущностей и автотипографики).</param>
		public _Cell_Text_Base(
			string? value,
			bool useRaw)
		{
			Value = value;
			UseRaw = useRaw;
		}

		/// <summary>
		/// Возвращает исходное текстовое значение ячейки.
		/// </summary>
		public string? Value { get; }

		/// <summary>
		/// Возвращает признак вывода строки без предварительного экранирования HTML-сущностей.
		/// </summary>
		public bool UseRaw { get; }

		/// <summary>
		/// Формирует и возвращает финальное строковое HTML-представление текстовой ячейки.
		/// </summary>
		/// <remarks>
		/// Если значение свойства <see cref="Value"/> пусто или равно <see langword="null"/>, 
		/// метод возвращает неразрывный пробел <c>&amp;nbsp;</c> для предотвращения визуального схлопывания ячейки.
		/// </remarks>
		/// <returns>Строка, содержащая HTML-код для вывода в представлении.</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Value))
				return "&nbsp;";
			return UseRaw
				? Value
				: SuppTypograph.GetText2Html(Value) ?? string.Empty;
		}
	}



	/// <summary>
	/// Компонент ячейки формы, выполняющий интеллектуальное усечение текста 
	/// до 50 символов с автоматическим добавлением маркера обрезки и HTML-экранированием.
	/// </summary>
	public class Cell__Crop50
		: IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell__Crop50"/>.
		/// </summary>
		/// <param name="value">Исходное текстовое значение для усечения и вывода. Допускает значение <see langword="null"/>.</param>
		public Cell__Crop50(
			string? value)
		{
			Value = value;
		}

		/// <summary>
		/// Возвращает исходное текстовое значение ячейки.
		/// </summary>
		public string? Value { get; }

		/// <summary>
		/// Формирует и возвращает усеченное до 50 символов строковое HTML-представление ячейки.
		/// </summary>
		/// <remarks>
		/// Если исходный текст пуст или равен <see langword="null"/>, возвращается неразрывный пробел 
		/// <c>&amp;nbsp;</c> для предотвращения визуального схлопывания ячейки в браузере.
		/// </remarks>
		/// <returns>Строка, содержащая экранированный и усеченный HTML-код.</returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(Value))
				return "&nbsp;";
			var croppedText1 = Value.GetCrop(0, 50, null, null);
			return SuppTypograph.GetText2Html(croppedText1) ?? string.Empty;
		}
	}



	/// <summary>
	/// Компонент ячейки формы, отображающий внешнюю веб-ссылку 
	/// с интеллектуальным усечением отображаемого URL до 50 символов.
	/// </summary>
	public class Cell__Link
		: IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell__Link"/>.
		/// </summary>
		/// <param name="value">Целевой URL-адрес для ссылки. Допускает значение <see langword="null"/>.</param>
		public Cell__Link(
			string? value)
		{
			Value = value;
		}

		/// <summary>
		/// Возвращает исходный URL-адрес ссылки.
		/// </summary>
		public string? Value { get; }

		/// <summary>
		/// Формирует и возвращает HTML-разметку тега ссылки <c>&lt;a&gt;</c> с атрибутом открытия в новом окне.
		/// </summary>
		public override string ToString()
		{
			return string.IsNullOrEmpty(Value)
				? "&nbsp;"
				: $"<a class=\"text-break\" target=\"_blank\" href=\"{Value}\">{Value.GetCrop(0, 50, null, null)}</a>";
		}
	}



	/// <summary>
	/// Компонент ячейки формы, отображающий логическое значение (флаг) 
	/// в виде локализованной HTML-метки Да/Нет.
	/// </summary>
	public class Cell_Bool
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Bool"/>.
		/// </summary>
		/// <param name="value">Логическое значение.</param>
		public Cell_Bool(
			bool value)
			: base(value.Make(Resources.Common.Html_CellTrue ?? "да", Resources.Common.Html_CellFalse ?? "&nbsp;"), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения календарной даты без времени.
	/// </summary>
	public class Cell_DateOnly
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_DateOnly"/>.
		/// </summary>
		/// <param name="value">Календарная дата.</param>
		public Cell_DateOnly(
			DateOnly? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения даты и времени в стандартном коротком формате.
	/// </summary>
	public class Cell_DateTime
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_DateTime"/>.
		/// </summary>
		/// <param name="value">Объект даты и времени.</param>
		public Cell_DateTime(
			DateTime? value)
			: base(value?.ToString("g"), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения высокоточных десятичных финансовых чисел.
	/// </summary>
	public class Cell_Decimal
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Decimal"/>.
		/// </summary>
		/// <param name="value">Десятичное число.</param>
		public Cell_Decimal(
			decimal? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения чисел с плавающей запятой двойной точности.
	/// </summary>
	public class Cell_Double
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Double"/>.
		/// </summary>
		/// <param name="value">Число двойной точности.</param>
		public Cell_Double(
			double? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения адресов электронной почты.
	/// </summary>
	public class Cell_Email
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Email"/>.
		/// </summary>
		/// <param name="value">Адрес электронной почты.</param>
		public Cell_Email(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения чисел с плавающей запятой одинарной точности.
	/// </summary>
	public class Cell_Float
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Float"/>.
		/// </summary>
		/// <param name="value">Число одинарной точности.</param>
		public Cell_Float(
			float? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения стандартных 32-битных целых чисел.
	/// </summary>
	public class Cell_Int
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Int"/>.
		/// </summary>
		/// <param name="value">Целое число.</param>
		public Cell_Int(
			int? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения больших 64-битных целых чисел.
	/// </summary>
	public class Cell_Long
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Long"/>.
		/// </summary>
		/// <param name="value">Большое целое число.</param>
		public Cell_Long(
			long? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения значения из фиксированного реестра по числовому коду.
	/// </summary>
	public class Cell_Enum
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Enum"/>.
		/// </summary>
		/// <param name="value">Числовой идентификатор перечисления.</param>
		/// <param name="registry">Реестр элементов справочника.</param>
		public Cell_Enum(
			int value,
			RegistryList registry)
			: base(registry.GetValue(value), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения значения из фиксированного реестра по строковому ключу.
	/// </summary>
	public class Cell_EnumString
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_EnumString"/>.
		/// </summary>
		/// <param name="value">Строковый ключ перечисления.</param>
		/// <param name="registry">Реестр элементов справочника.</param>
		public Cell_EnumString(
			string? value,
			RegistryList registry)
			: base(value is null ? null : registry.GetValueOrKey(value), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения текстового названия связанной внешней сущности (внешнего ключа).
	/// </summary>
	public class Cell_Reference
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Reference"/>.
		/// </summary>
		/// <param name="value">Идентификатор связи.</param>
		/// <param name="registry">Реестр связанных сущностей.</param>
		public Cell_Reference(
			int? value,
			RegistryList registry)
			: base(value.HasValue ? registry.GetValue(value.Value) : null, true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения набора выбранных флагов (множественного выбора), 
	/// разделенных знаками пунктуации, в виде сгруппированных HTML-тегов.
	/// </summary>
	public class Cell_Set
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Set"/>.
		/// </summary>
		/// <param name="value">Сериализованная строка выбранных ключей флагов.</param>
		/// <param name="registry">Реестр элементов справочника флагов.</param>
		public Cell_Set(
			string? value,
			RegistryList registry)
			: base(string.IsNullOrEmpty(value)
				? null
				: value.Split(SuppForms.SEP_ITEMS, StringSplitOptions.RemoveEmptyEntries)
				  .MakeFromCollection(x => registry.GetValue(x) ?? x, null, "<span>{0}</span>", null), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения точного времени суток без привязки к календарной дате.
	/// </summary>
	public class Cell_TimeOnly
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_TimeOnly"/>.
		/// </summary>
		/// <param name="value">Время суток.</param>
		public Cell_TimeOnly(
			TimeOnly? value)
			: base(value?.ToString(), true)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения расширенных многострочных текстовых блоков (Memo).
	/// </summary>
	public class Cell_Memo
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Memo"/>.
		/// </summary>
		/// <param name="value">Многострочный текст.</param>
		public Cell_Memo(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения стандартных имен, названий и строковых идентификаторов.
	/// </summary>
	public class Cell_Name
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Name"/>.
		/// </summary>
		/// <param name="value">Имя или название.</param>
		public Cell_Name(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения системных имен переменных в C#-стиле.
	/// </summary>
	public class Cell_Varname
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Varname"/>.
		/// </summary>
		/// <param name="value">Имя переменной.</param>
		public Cell_Varname(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения ультракоротких текстовых строк (до 50 символов).
	/// </summary>
	public class Cell_Text50
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Text50"/>.
		/// </summary>
		/// <param name="value">Текст короткой строки.</param>
		public Cell_Text50(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения коротких текстовых строк (до 100 символов).
	/// </summary>
	public class Cell_Text100
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Text100"/>.
		/// </summary>
		/// <param name="value">Текст строки.</param>
		public Cell_Text100(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения строк средней длины (до 250 символов).
	/// </summary>
	public class Cell_Text250
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Text250"/>.
		/// </summary>
		/// <param name="value">Текст строки.</param>
		public Cell_Text250(
			string? value)
			: base(value, false)
		{
		}
	}



	/// <summary>
	/// Компонент ячейки формы для отображения расширенных текстовых строк (до 400 символов).
	/// </summary>
	public class Cell_Text400
		: _Cell_Text_Base,
		IFormCellControl
	{
		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="Cell_Text400"/>.
		/// </summary>
		/// <param name="value">Текст расширенной строки.</param>
		public Cell_Text400(
			string? value)
			: base(value, false)
		{
		}
	}

}
