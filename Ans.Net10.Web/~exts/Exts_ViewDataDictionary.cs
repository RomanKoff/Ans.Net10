// rev 2026-09-30

using Ans.Net10.Common;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="ViewDataDictionary"/>, обеспечивающие строго типизированную 
	/// запись и чтение динамических данных в представлениях и шаблонах Razor.
	/// </summary>
	public static partial class Exts_ViewDataDictionary
	{

		/* consts */


		/// <summary>
		/// Системный ключ в словаре ViewData для хранения и извлечения метаданных пагинации.
		/// </summary>
		public const string KEY_PAGINATED_DATA = "Paginated_Data";


		/// <summary>
		/// Системный ключ в словаре ViewData для хранения и извлечения метаданных CRUD-поля.
		/// </summary>
		public const string KEY_FIELD_INFO = "Field_Info";


		/* methods */


		/// <summary>
		/// Записывает объект списка реестра <see cref="RegistryList"/> в словарь ViewData со специализированным префиксом.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Системное имя (ключ) реестра.</param>
		/// <param name="registry">Экземпляр записываемого реестра.</param>
		public static void SetRegistryList(
			this ViewDataDictionary viewData,
			string name,
			RegistryList registry)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			viewData[$"Reg_{name}"] = registry;
		}


		/// <summary>
		/// Преобразует коллекцию элементов в объект <see cref="RegistryList"/> с помощью функции-селектора и записывает его в словарь ViewData.
		/// </summary>
		/// <typeparam name="T">Тип исходных объектов коллекции.</typeparam>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Системное имя (ключ) реестра.</param>
		/// <param name="items">Исходная коллекция элементов.</param>
		/// <param name="funcItem">Делегат функции преобразования объекта в элемент реестра <see cref="RegistryItem"/>.</param>
		public static void SetRegistryList<T>(
			this ViewDataDictionary viewData,
			string name,
			IEnumerable<T> items,
			Func<T, RegistryItem> funcItem)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			ArgumentNullException.ThrowIfNull(items);
			ArgumentNullException.ThrowIfNull(funcItem);
			var reg1 = new RegistryList(items.Select(funcItem));
			viewData.SetRegistryList(name, reg1);
		}


		/// <summary>
		/// Преобразует коллекцию элементов в плоский объект <see cref="RegistryList"/> на основе функций извлечения ключа и значения, и записывает его в словарь ViewData.
		/// </summary>
		/// <typeparam name="T">Тип исходных объектов коллекции.</typeparam>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Системное имя (ключ) реестра.</param>
		/// <param name="items">Исходная коллекция элементов.</param>
		/// <param name="funcKey">Делегат функции извлечения уникального ключа элемента.</param>
		/// <param name="funcValue">Делегат функции извлечения текстового контента элемента.</param>
		public static void SetRegistryList<T>(
			this ViewDataDictionary viewData,
			string name,
			IEnumerable<T> items,
			Func<T, string> funcKey,
			Func<T, string> funcValue)
		{
			ArgumentNullException.ThrowIfNull(funcKey);
			ArgumentNullException.ThrowIfNull(funcValue);
			viewData.SetRegistryList(
				name, items, x => new RegistryItem(
					funcKey(x), funcValue(x), 0, false));
		}


		/// <summary>
		/// Записывает модель параметров пагинации <see cref="PaginatedDataModel"/> в словарь ViewData.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="pagination">Экземпляр модели пагинации.</param>
		public static void SetPaginationData(
			this ViewDataDictionary viewData,
			PaginatedDataModel pagination)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			viewData[KEY_PAGINATED_DATA] = pagination;
		}


		/* functions */


		/// <summary>
		/// Извлекает объект списка реестра <see cref="RegistryList"/> из словаря ViewData по его имени.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Системное имя (ключ) реестра.</param>
		/// <returns>Найденный объект реестра или <see langword="null"/>, если ключ отсутствует или тип не совпадает.</returns>
		public static RegistryList? GetRegistryList(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			return viewData[$"Reg_{name}"] as RegistryList;
		}


		/// <summary>
		/// Извлекает модель параметров пагинации <see cref="PaginatedDataModel"/> из словаря ViewData.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <returns>Экземпляр модели пагинации или <see langword="null"/>, если данные отсутствуют.</returns>
		public static PaginatedDataModel? GetPaginationData(
			this ViewDataDictionary viewData)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			return viewData[KEY_PAGINATED_DATA] as PaginatedDataModel;
		}


		/// <summary>
		/// Извлекает данные пагинации и формирует на их основе вычислительный хелпер <see cref="PaginationHelper"/>.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <returns>Новый инициализированный экземпляр хелпера постраничной навигации.</returns>
		public static PaginationHelper GetPaginationHelper(
			this ViewDataDictionary viewData)
		{
			var data1 = viewData.GetPaginationData();
			return new PaginationHelper(
				data1?.ItemsOnPage ?? 0,
				data1?.TotalItems ?? 0,
				data1?.Page ?? 0);
		}


		/// <summary>
		/// Извлекает метаданные отображения CRUD-поля <see cref="CrudFace"/> из словаря ViewData.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <returns>Объект метаданных поля или <see langword="null"/>, если данные отсутствуют.</returns>
		public static CrudFace? GetFieldInfo(
			this ViewDataDictionary viewData)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			return viewData[KEY_FIELD_INFO] as CrudFace;
		}


		/// <summary>
		/// Извлекает объект произвольного типа из словаря ViewData с помощью стандартного метода вычисления выражений Eval.
		/// </summary>
		/// <typeparam name="T">Целевой тип извлекаемого объекта.</typeparam>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Значение, приведенное к типу <typeparamref name="T"/>, или <see langword="null"/>.</returns>
		public static T? Get<T>(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is T value2
				? value2 : default;
		}


		/*--- string ---*/


		/// <summary>
		/// Извлекает строковое значение из словаря ViewData. Если значение отсутствует, возвращает указанный fallback-вариант.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение, возвращаемое по умолчанию при отсутствии ключа.</param>
		/// <returns>Строковое значение или значение по умолчанию.</returns>
		public static string? GetString(
			this ViewDataDictionary viewData,
			string name,
			string? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			return viewData.Eval(name) as string ?? defaultValue;
		}


		/*--- int ---*/


		/// <summary>
		/// Извлекает целочисленное значение из словаря ViewData со сбросом в nullable-состояние.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Целое число со знаком или <see langword="null"/>.</returns>
		public static int? GetInt(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is int value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает целочисленное значение из словаря ViewData с поддержкой кастомного значения по умолчанию.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Целое число.</returns>
		public static int GetInt(
			this ViewDataDictionary viewData,
			string name,
			int defaultValue)
		{
			return viewData.GetInt(name) ?? defaultValue;
		}


		/*--- uint ---*/


		/// <summary>
		/// Извлекает из словаря ViewData значение по его имени со сбросом в nullable-состояние 32-битного целого числа без знака.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Целое число без знака или <see langword="null"/>.</returns>
		public static uint? GetUInt(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is uint value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает из словаря ViewData значение по его имени с поддержкой кастомного значения по умолчанию типа uint.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Целое число без знака.</returns>
		public static uint GetUInt(
			this ViewDataDictionary viewData,
			string name,
			uint defaultValue)
		{
			return viewData.GetUInt(name) ?? defaultValue;
		}


		/*--- long ---*/


		/// <summary>
		/// Извлекает из словаря ViewData значение по его имени со сбросом в nullable-состояние 64-битного целого числа со знаком.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Большое целое число со знаком или <see langword="null"/>.</returns>
		public static long? GetLong(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is long value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает из словаря ViewData значение по его имени с поддержкой кастомного значения по умолчанию типа long.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Большое целое число со знаком.</returns>
		public static long GetLong(
			this ViewDataDictionary viewData,
			string name,
			long defaultValue)
		{
			return viewData.GetLong(name) ?? defaultValue;
		}


		/*--- double ---*/


		/// <summary>
		/// Извлекает из словаря ViewData структуру числа с плавающей запятой двойной точности.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Число с плавающей запятой двойной точности или <see langword="null"/>.</returns>
		public static double? GetDouble(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is double value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает из словаря ViewData структуру числа двойной точности с поддержкой fallback-значения.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Число с плавающей запятой двойной точности.</returns>
		public static double GetDouble(
			this ViewDataDictionary viewData,
			string name,
			double defaultValue)
		{
			return viewData.GetDouble(name) ?? defaultValue;
		}


		/*--- float ---*/


		/// <summary>
		/// Извлекает из словаря ViewData структуру числа с плавающей запятой одинарной точности.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Число с плавающей запятой одинарной точности или <see langword="null"/>.</returns>
		public static float? GetFloat(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is float value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает из словаря ViewData структуру числа одинарной точности с поддержкой fallback-значения.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Число с плавающей запятой одинарной точности.</returns>
		public static float GetFloat(
			this ViewDataDictionary viewData,
			string name,
			float defaultValue)
		{
			return viewData.GetFloat(name) ?? defaultValue;
		}


		/*--- decimal ---*/


		/// <summary>
		/// Извлекает из словаря ViewData высокоточное десятичное число.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Высокоточное десятичное число или <see langword="null"/>.</returns>
		public static decimal? GetDecimal(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is decimal value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает из словаря ViewData высокоточное десятичное число с поддержкой fallback-значения.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Высокоточное десятичное число.</returns>
		public static decimal GetDecimal(
			this ViewDataDictionary viewData,
			string name,
			decimal defaultValue)
		{
			return viewData.GetDecimal(name) ?? defaultValue;
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Извлекает структуру даты и времени из словаря ViewData.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Объект даты и времени или <see langword="null"/>.</returns>
		public static DateTime? GetDateTime(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is DateTime value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает структуру даты и времени из словаря ViewData с поддержкой fallback-значения.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Объект даты и времени.</returns>
		public static DateTime GetDateTime(
			this ViewDataDictionary viewData,
			string name,
			DateTime defaultValue)
		{
			return viewData.GetDateTime(name) ?? defaultValue;
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Извлекает календарную дату из словаря ViewData.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Календарная дата или <see langword="null"/>.</returns>
		public static DateOnly? GetDateOnly(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is DateOnly value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает календарную дату из словаря ViewData с поддержкой fallback-значения.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Календарная дата.</returns>
		public static DateOnly GetDateOnly(
			this ViewDataDictionary viewData,
			string name,
			DateOnly defaultValue)
		{
			return viewData.GetDateOnly(name) ?? defaultValue;
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Извлекает значение времени суток из словаря ViewData.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Время суток или <see langword="null"/>.</returns>
		public static TimeOnly? GetTimeOnly(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is TimeOnly value2
				? value2 : null;
		}


		/// <summary>
		/// Извлекает значение времени суток из словаря ViewData с поддержкой fallback-значения.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <param name="defaultValue">Значение по умолчанию.</param>
		/// <returns>Время суток.</returns>
		public static TimeOnly GetTimeOnly(
			this ViewDataDictionary viewData,
			string name,
			TimeOnly defaultValue)
		{
			return viewData.GetTimeOnly(name) ?? defaultValue;
		}


		/*--- bool ---*/


		/// <summary>
		/// Извлекает логическое значение из словаря ViewData. При отсутствии ключа возвращает <see langword="false"/>.
		/// </summary>
		/// <param name="viewData">Текущий словарь ViewData.</param>
		/// <param name="name">Имя выражения или ключа словаря.</param>
		/// <returns>Логическое значение флажка.</returns>
		public static bool GetBool(
			this ViewDataDictionary viewData,
			string name)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			ArgumentException.ThrowIfNullOrEmpty(name);
			var value1 = viewData.Eval(name);
			return value1 is bool value2 && value2;
		}

	}

}
