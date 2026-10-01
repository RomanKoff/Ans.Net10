using Ans.Net10.Common;
using System.Globalization;

namespace Ans.Net10.Psql
{

	/// <summary>
	/// Вспомогательный класс, предоставляющий методы для генерации сырых SQL-запросов, 
	/// форматирования и безопасного приведения типов к строковым литералам PostgreSQL.
	/// </summary>
	public static class SuppSql
	{

		/// <summary>
		/// Формирует SQL-скрипт для синхронизации и сброса счетчика автоинкрементной последовательности (SERIAL/BIGSERIAL) 
		/// таблицы PostgreSQL на основе текущего максимального значения её первичного ключа.
		/// </summary>
		/// <param name="table">Имя целевой таблицы базы данных.</param>
		/// <param name="key">Имя столбца первичного ключа (идентификатора).</param>
		/// <returns>Строка SQL-запроса с вызовом функции <c>setval</c>.</returns>
		public static string GetPsqlSerialSequence(
			string table,
			string key)
		{
			return $@"
SELECT setval(
	pg_get_serial_sequence('public.""{table}""', '{key}'),
	SELECT max(""{key}"") from public.""{table}""
);";
		}


		/*--- string ---*/


		/// <summary>
		/// Преобразует строковое значение в безопасный строковый литерал PostgreSQL с экранированием одинарных кавычек. 
		/// Возвращает <c>NULL</c>, если строка пуста или равна значению <see langword="null"/>.
		/// </summary>
		/// <param name="value">Исходное строковое значение.</param>
		/// <returns>Строковый литерал в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			string value)
		{
			if (string.IsNullOrEmpty(value))
				return "NULL";
			var s1 = value.Replace("'", "''");
			return $"'{s1}'";
		}


		/// <summary>
		/// Безопасно преобразует произвольный объект в строковый литерал PostgreSQL, вызывая метод <see cref="object.ToString"/>.
		/// </summary>
		/// <param name="value">Преобразуемый объект.</param>
		/// <returns>Результат обработки значения методом <see cref="GetValue(string)"/>.</returns>
		public static string GetValueAsString(
			object value)
		{
			return GetValue(value?.ToString() ?? string.Empty);
		}


		/*--- int ---*/


		/// <summary>
		/// Возвращает строковое представление 32-битного целого числа со знаком для подстановки в SQL-запрос.
		/// </summary>
		/// <param name="value">Числовое значение типа <see cref="int"/>.</param>
		/// <returns>Строка, содержащая числовое значение.</returns>
		public static string GetValue(
			int value)
		{
			return value.ToString();
		}


		/// <summary>
		/// Преобразует nullable 32-битное целое число в строковое представление. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение целого числа, допускающее <see langword="null"/>.</param>
		/// <returns>Строковое представление числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			int? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в 32-битное целое число со знаком. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление полученного числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsIntOrNULL(
			object value)
		{
			return GetValue(value?.ToString().ToInt());
		}


		/// <summary>
		/// Безопасно парсит произвольный объект в 32-битное целое число со знаком. 
		/// В случае ошибки парсинга или равенства объекта <see langword="null"/> возвращает строковое представление нуля (<c>"0"</c>).
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление числа либо строка <c>"0"</c>.</returns>
		public static string GetValueAsIntOr0(
			object value)
		{
			return GetValue(value?.ToString().ToInt() ?? 0);
		}


		/*--- long ---*/


		/// <summary>
		/// Возвращает строковое представление 64-битного целого числа со знаком (long) для подстановки в SQL-запрос.
		/// </summary>
		/// <param name="value">Числовое значение типа <see cref="long"/>.</param>
		/// <returns>Строка, содержащая числовое значение.</returns>
		public static string GetValue(
			long value)
		{
			return value.ToString();
		}


		/// <summary>
		/// Преобразует nullable 64-битное целое число в строковое представление. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение длинного целого числа, допускающее <see langword="null"/>.</param>
		/// <returns>Строковое представление числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			long? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в 64-битное целое число со знаком. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление полученного числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsLongOrNULL(
			object value)
		{
			return GetValue(value?.ToString().ToLong());
		}


		/// <summary>
		/// Безопасно парсит произвольный объект в 64-битное целое число со знаком. 
		/// В случае ошибки парсинга или равенства объекта <see langword="null"/> возвращает строковое представление нуля (<c>"0"</c>).
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление числа либо строка <c>"0"</c>.</returns>
		public static string GetValueAsLongOr0(
			object value)
		{
			return GetValue(value?.ToString().ToLong() ?? 0);
		}


		/*--- float ---*/


		/// <summary>
		/// Возвращает строковое представление числа с плавающей запятой одинарной точности (float), 
		/// отформатированное по стандартам инвариантной культуры для безопасной подстановки в SQL PostgreSQL.
		/// </summary>
		/// <param name="value">Числовое значение типа <see cref="float"/>.</param>
		/// <returns>Строка с числовым значением, где разделителем является точка.</returns>
		public static string GetValue(
			float value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}


		/// <summary>
		/// Преобразует nullable число с плавающей запятой одинарной точности в строковое представление. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение числа с плавающей запятой, допускающее <see langword="null"/>.</param>
		/// <returns>Строковое представление числа с точкой в качестве разделителя либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			float? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в число с плавающей запятой одинарной точности. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление полученного числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsFloatOrNULL(
			object value)
		{
			return GetValue(value?.ToString().ToFloat());
		}


		/// <summary>
		/// Безопасно парсит произвольный объект в число с плавающей запятой одинарной точности. 
		/// В случае ошибки парсинга или равенства объекта <see langword="null"/> возвращает строковое представление нуля (<c>"0"</c>).
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление числа либо строка <c>"0"</c>.</returns>
		public static string GetValueAsFloatOr0(
			object value)
		{
			return GetValue(value?.ToString().ToFloat() ?? 0f);
		}


		/*--- double ---*/


		/// <summary>
		/// Возвращает строковое представление числа с плавающей запятой двойной точности (double), 
		/// отформатированное по стандартам инвариантной культуры для безопасной подстановки в SQL PostgreSQL.
		/// </summary>
		/// <param name="value">Числовое значение типа <see cref="double"/>.</param>
		/// <returns>Строка с числовым значением, где разделителем является точка.</returns>
		public static string GetValue(
			double value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}


		/// <summary>
		/// Преобразует nullable число с плавающей запятой двойной точности в строковое представление. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение числа с плавающей запятой двойной точности, допускающее <see langword="null"/>.</param>
		/// <returns>Строковое представление числа с точкой в качестве разделителя либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			double? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в число с плавающей запятой двойной точности. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление полученного числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsDoubleOrNULL(
			object value)
		{
			return GetValue(value?.ToString().ToDouble());
		}


		/// <summary>
		/// Безопасно парсит произвольный объект в число с плавающей запятой двойной точности. 
		/// В случае ошибки парсинга или равенства объекта <see langword="null"/> возвращает строковое представление нуля (<c>"0"</c>).
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление числа либо строка <c>"0"</c>.</returns>
		public static string GetValueAsDoubleOr0(
			object value)
		{
			return GetValue(value?.ToString().ToDouble(0) ?? 0.0d);
		}


		/*--- decimal ---*/


		/// <summary>
		/// Возвращает строковое представление фиксированного десятичного числа (decimal), 
		/// отформатированное по стандартам инвариантной культуры для безопасной подстановки в SQL PostgreSQL.
		/// </summary>
		/// <param name="value">Числовое значение типа <see cref="decimal"/>.</param>
		/// <returns>Строка с числовым значением, где разделителем является точка.</returns>
		public static string GetValue(
			decimal value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}


		/// <summary>
		/// Преобразует nullable фиксированное десятичное число в строковое представление. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Десятичное числовое значение, допускающее <see langword="null"/>.</param>
		/// <returns>Строковое представление числа с точкой в качестве разделителя либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			decimal? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в фиксированное десятичное число (decimal). 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление полученного числа либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsDecimalOrNULL(
			object value)
		{
			return GetValue(value?.ToString().ToDecimal());
		}


		/// <summary>
		/// Безопасно парсит произвольный объект в фиксированное десятичное число (decimal). 
		/// В случае ошибки парсинга или равенства объекта <see langword="null"/> возвращает строковое представление нуля (<c>"0"</c>).
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковое представление числа либо строка <c>"0"</c>.</returns>
		public static string GetValueAsDecimalOr0(
			object value)
		{
			return GetValue(value?.ToString().ToDecimal(0) ?? 0m);
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Преобразует значение даты и времени в строковый литерал стандарта ISO 8601 в одинарных кавычках, 
		/// пригодный для прямой подстановки в запросы к PostgreSQL.
		/// </summary>
		/// <param name="value">Значение даты и времени типа <see cref="DateTime"/>.</param>
		/// <returns>Строковый литерал даты и времени с миллисекундами, заключенный в одинарные кавычки.</returns>
		public static string GetValue(
			DateTime value)
		{
			return $"'{value:yyyy-MM-dd HH:mm:ss.000}'";
		}


		/// <summary>
		/// Преобразует nullable значение даты и времени в строковый литерал. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение даты и времени, допускающее <see langword="null"/>.</param>
		/// <returns>Строковый литерал даты и времени в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			DateTime? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в значение даты и времени. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковый литерал даты и времени в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsDateTimeOrNULL(
			object value)
		{
			if (value == null)
				return "NULL";
			var v1 = value.ToString().ToDateTime();
			return v1 == null
				? "NULL" : GetValue(v1.Value);
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Преобразует значение даты (без времени) в строковый литерал стандарта ISO 8601 в одинарных кавычках, 
		/// пригодный для прямой подстановки в запросы к PostgreSQL.
		/// </summary>
		/// <param name="value">Значение даты типа <see cref="DateOnly"/>.</param>
		/// <returns>Строковый литерал даты, заключенный в одинарные кавычки.</returns>
		public static string GetValue(
			DateOnly value)
		{
			return $"'{value:yyyy-MM-dd}'";
		}


		/// <summary>
		/// Преобразует nullable значение даты в строковый литерал. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение даты, допускающее <see langword="null"/>.</param>
		/// <returns>Строковый литерал даты в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			DateOnly? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в значение календарной даты. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковый литерал даты в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsDateOnlyOrNULL(
			object value)
		{
			if (value == null)
				return "NULL";
			var v1 = value.ToString().ToDateOnly();
			return v1 == null
				? "NULL" : GetValue(v1.Value);
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Преобразует значение времени суток в строковый литерал в одинарных кавычках, 
		/// пригодный для прямой подстановки в запросы к PostgreSQL.
		/// </summary>
		/// <param name="value">Значение времени суток типа <see cref="TimeOnly"/>.</param>
		/// <returns>Строковый литерал времени суток с миллисекундами, заключенный в одинарные кавычки.</returns>
		public static string GetValue(
			TimeOnly value)
		{
			return $"'{value:HH:mm:ss.000}'";
		}


		/// <summary>
		/// Преобразует nullable значение времени суток в строковый литерал. 
		/// Возвращает <c>"NULL"</c>, если значение отсутствует.
		/// </summary>
		/// <param name="value">Значение времени суток, допускающее <see langword="null"/>.</param>
		/// <returns>Строковый литерал времени суток в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValue(
			TimeOnly? value)
		{
			return value == null
				? "NULL" : GetValue(value.Value);
		}


		/// <summary>
		/// Безопасно парсит объект в значение времени суток. 
		/// Возвращает <c>"NULL"</c> в случае неудачи или равенства исходного объекта <see langword="null"/>.
		/// </summary>
		/// <param name="value">Объект для парсинга.</param>
		/// <returns>Строковый литерал времени суток в одинарных кавычках либо строка <c>"NULL"</c>.</returns>
		public static string GetValueAsTimeOnlyOrNULL(
			object value)
		{
			if (value == null)
				return "NULL";
			var v1 = value.ToString().ToTimeOnly();
			return v1 == null
				? "NULL" : GetValue(v1.Value);
		}


		/*--- bool ---*/


		/// <summary>
		/// Преобразует логическое значение в текстовый литерал PostgreSQL (<c>'t'</c> или <c>'f'</c>).
		/// </summary>
		/// <param name="value">Логическое значение.</param>
		/// <returns>Строка <c>"'t'"</c> для истинного значения либо <c>"'f'"</c> для ложного.</returns>
		public static string GetValue(
			bool value)
		{
			return value ? "'t'" : "'f'";
		}


		/// <summary>
		/// Безопасно приводит произвольный объект к логическому значению и возвращает его литерал для PostgreSQL.
		/// </summary>
		/// <param name="value">Объект для анализа.</param>
		/// <returns>Результат обработки значения методом <see cref="GetValue(bool)"/>.</returns>
		public static string GetValueAsBool(
			object value)
		{
			return GetValue(value?.ToString().ToBool() ?? false);
		}

	}

}