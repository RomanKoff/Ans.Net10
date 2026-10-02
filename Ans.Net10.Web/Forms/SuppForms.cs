// rev 2026-10-02

namespace Ans.Net10.Web.Forms
{

	/// <summary>
	/// Вспомогательный класс, содержащий базовые конфигурационные константы, 
	/// лимиты ширины (Max Width) и параметры разметки для полей ввода CRUD-форм.
	/// </summary>
	public static class SuppForms
	{

		/// <summary>
		/// Максимальная ширина для ультракороткого текстового поля ввода (Text50).
		/// </summary>
		public const int MW_Text50 = 15;


		/// <summary>
		/// Максимальная ширина для короткого текстового поля ввода (Text100).
		/// </summary>
		public const int MW_Text100 = 25;


		/// <summary>
		/// Максимальная ширина для текстового поля ввода средней длины (Text250).
		/// </summary>
		public const int MW_Text250 = 40;


		/// <summary>
		/// Максимальная ширина для расширенного текстового поля ввода (Text400).
		/// </summary>
		public const int MW_Text400 = 80;


		/// <summary>
		/// Стандартное количество колонок (ширина) для многострочного текстового поля (Memo/TextArea).
		/// </summary>
		public const int COLS_Memo = 100;


		/// <summary>
		/// Максимальная ширина для поля ввода стандартных имен или названий (Name).
		/// </summary>
		public const int MW_Name = 12;


		/// <summary>
		/// Максимальная ширина для поля ввода системных имен переменных в C#-стиле (Varname).
		/// </summary>
		public const int MW_Varname = MW_Name;


		/// <summary>
		/// Максимальная ширина для поля ввода адреса электронной почты (Email).
		/// </summary>
		public const int MW_Email = 16;


		/// <summary>
		/// Максимальная ширина для поля ввода стандартных 32-битных целых чисел (Int).
		/// </summary>
		public const int MW_Int = 8;


		/// <summary>
		/// Максимальная ширина для поля ввода больших 64-битных целых чисел (Long).
		/// </summary>
		public const int MW_Long = 10;


		/// <summary>
		/// Максимальная ширина для поля ввода вещественных чисел одинарной точности (Float).
		/// </summary>
		public const int MW_Float = MW_Long;


		/// <summary>
		/// Максимальная ширина для поля ввода вещественных чисел двойной точности (Double).
		/// </summary>
		public const int MW_Double = MW_Long;


		/// <summary>
		/// Максимальная ширина для поля ввода высокоточных десятичных финансовых чисел (Decimal).
		/// </summary>
		public const int MW_Decimal = MW_Long;


		/// <summary>
		/// Максимальная ширина для поля ввода даты и времени (DateTime).
		/// </summary>
		public const int MW_DateTime = 11;


		/// <summary>
		/// Максимальная ширина для поля ввода календарной даты без времени (DateOnly).
		/// </summary>
		public const int MW_DateOnly = 9;


		/// <summary>
		/// Максимальная ширина для поля ввода времени суток без даты (TimeOnly).
		/// </summary>
		public const int MW_TimeOnly = 6;


		/// <summary>
		/// Максимальная ширина для поля ввода логического флажка (Bool).
		/// </summary>
		public const int MW_Bool = 5;


		/// <summary>
		/// Стандартный набор символов-разделителей элементов коллекции.
		/// </summary>
		public static readonly char[] SEP_ITEMS = [',', ';'];

	}

}
