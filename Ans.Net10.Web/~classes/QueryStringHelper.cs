// rev 2026-09-28

using Ans.Net10.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Вспомогательный класс для удобного парсинга, фильтрации, типизированного извлечения 
	/// и динамического построения строк запроса (Query String) в веб-приложениях.
	/// </summary>
	public class QueryStringHelper
	{

		/* ctors */


		/// <summary>
		/// Инициализирует экземпляр хелпера на основе готового словаря параметров с возможностью фильтрации игнорируемых ключей.
		/// </summary>
		/// <param name="queryParams">Исходный словарь параметров запроса.</param>
		/// <param name="ignoreParams">Массив имен параметров, которые необходимо исключить из обработки.</param>
		public QueryStringHelper(
			Dictionary<string, StringValues>? queryParams,
			params string[] ignoreParams)
		{
			var ignores1 = (ignoreParams ?? [])
				.Concat(["descending"])
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (queryParams != null)
				foreach (var key1 in queryParams.Keys)
					if (!ignores1.Contains(key1))
						Params.Add(key1, queryParams[key1]);
		}


		/// <summary>
		/// Инициализирует экземпляр хелпера на основе коллекции параметров <see cref="IQueryCollection"/> текущего HTTP-запроса.
		/// </summary>
		public QueryStringHelper(
			IQueryCollection queryParams,
			params string[] ignoreParams)
			: this(queryParams?.ToDictionary(x => x.Key, x => x.Value), ignoreParams)
		{
		}


		/// <summary>
		/// Инициализирует экземпляр хелпера путем парсинга сырой строки запроса (URL Query String).
		/// </summary>
		public QueryStringHelper(
			string queryString,
			params string[] ignoreParams)
			: this(QueryHelpers.ParseQuery(queryString), ignoreParams)
		{
		}


		/// <summary>
		/// Инициализирует новый пустой экземпляр хелпера параметров строки запроса.
		/// </summary>
		public QueryStringHelper()
			: this(string.Empty)
		{
		}


		/* readonly properties */


		/// <summary>
		/// Возвращает внутренний рабочий словарь отфильтрованных параметров строки запроса.
		/// </summary>
		public Dictionary<string, StringValues> Params { get; } = new(StringComparer.OrdinalIgnoreCase);


		/* functions */


		/// <summary>
		/// Формирует итоговую строку запроса, объединяя указанный базовый URL с отфильтрованными параметрами.
		/// </summary>
		/// <param name="baseUrl">Базовый путь или URL веб-страницы.</param>
		/// <param name="allowedParams">Опциональный белый список параметров. Если передан, в URL попадут только эти параметры.</param>
		/// <returns>Полный URL-адрес со сформированной строкой параметров запроса.</returns>
		public string MakeQueryString(
			string baseUrl,
			params string[] allowedParams)
		{
			var filteredParams1 = allowedParams != null && allowedParams.Length > 0
				? Params.Where(x => allowedParams.Contains(x.Key))
				: Params;
			var targetDict1 = filteredParams1
				.GroupBy(x => x.Key)
				.ToDictionary(x => x.Key, x => x.Last().Value);
			return QueryHelpers.AddQueryString(baseUrl, targetDict1);
		}


		/// <summary>
		/// Проверяет наличие указанного ключа в коллекции параметров.
		/// </summary>
		public bool TestKey(
			string key)
		{
			return !string.IsNullOrEmpty(key) && Params.ContainsKey(key);
		}


		/// <summary>
		/// Проверяет, совпадает ли строковое значение параметра с указанным.
		/// </summary>
		public bool TestValue(
			string key,
			string value)
		{
			return TestKey(key) && string.Equals(Params[key].ToString(), value, StringComparison.Ordinal);
		}


		/*--- string ---*/


		/// <summary>
		/// Извлекает строковое значение параметра. При отсутствии возвращает значение по умолчанию.
		/// </summary>
		public string? GetString(
			string key,
			string? defaultValue = null)
		{
			return TestKey(key)
				? Params[key].ToString()
				: defaultValue;
		}


		/*--- int ---*/


		/// <summary>
		/// Извлекает целочисленное значение параметра со сбросом в nullable.
		/// </summary>
		public int? GetInt(
			string key)
		{
			return TestKey(key)
				? Params[key].ToString().ToInt()
				: null;
		}


		/// <summary>
		/// Извлекает целочисленное значение параметра с поддержкой fallback-значения.
		/// </summary>
		public int GetInt(
			string key,
			int defaultValue)
		{
			return GetInt(key) ?? defaultValue;
		}


		/*--- bool ---*/


		/// <summary>
		/// Извлекает логическое значение флага. При отсутствии или ошибке возвращает <see langword="false"/>.
		/// </summary>
		public bool GetBool(
			string key)
		{
			return TestKey(key) && Params[key].ToString().ToBool();
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Извлекает дату и время из параметров запроса.
		/// </summary>
		public DateTime? GetDateTime(
			string key)
		{
			return TestKey(key)
				? Params[key].ToString().ToDateTime()
				: null;
		}


		/// <summary>
		/// Извлекает дату и время из параметров с поддержкой fallback-значения.
		/// </summary>
		public DateTime GetDateTime(
			string key,
			DateTime defaultValue)
		{
			return GetDateTime(key) ?? defaultValue;
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Извлекает календарную дату из параметров запроса.
		/// </summary>
		public DateOnly? GetDateOnly(
			string key)
		{
			return TestKey(key)
				? Params[key].ToString().ToDateOnly()
				: null;
		}


		/// <summary>
		/// Извлекает календарную дату с поддержкой fallback-значения.
		/// </summary>
		public DateOnly GetDateOnly(
			string key,
			DateOnly defaultValue)
		{
			return GetDateOnly(key) ?? defaultValue;
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Извлекает время суток из параметров запроса.
		/// </summary>
		public TimeOnly? GetTimeOnly(
			string key)
		{
			return TestKey(key)
				? Params[key].ToString().ToTimeOnly()
				: null;
		}


		/// <summary>
		/// Извлекает время суток с поддержкой fallback-значения.
		/// </summary>
		public TimeOnly GetTimeOnly(
			string key,
			TimeOnly defaultValue)
		{
			return GetTimeOnly(key) ?? defaultValue;
		}


		/* methods */


		/// <summary>
		/// Удаляет указанный параметр из коллекции строки запроса.
		/// </summary>
		public void Remove(
			string key)
		{
			if (!string.IsNullOrEmpty(key))
				Params.Remove(key);
		}


		/*--- string ---*/


		/// <summary>
		/// Записывает или перезаписывает строковое значение параметра в коллекции.
		/// </summary>
		public void AppendString(
			string key,
			string? value)
		{
			if (!string.IsNullOrEmpty(key) && value != null)
				Params[key] = value;
		}


		/// <summary>
		/// Переносит строковое значение из словаря ViewData в текущую строку запроса.
		/// </summary>
		public void AppendString(
			ViewDataDictionary viewData,
			string key,
			string? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			AppendString(key, viewData.GetString(key, defaultValue));
		}


		/// <summary>
		/// Копирует строковое значение из другого хелпера параметров запроса.
		/// </summary>
		public void AppendString(
			QueryStringHelper helper,
			string key,
			string? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(helper);
			AppendString(key, helper.GetString(key, defaultValue));
		}


		/*--- int ---*/


		/// <summary>
		/// Записывает целочисленное значение параметра (значения равные 0 игнорируются).
		/// </summary>
		public void AppendInt(
			string key,
			int? value)
		{
			if (value.HasValue && value.Value != 0)
				AppendString(key, value.Value.ToString());
		}


		/// <summary>
		/// Переносит целочисленное значение из словаря ViewData в параметры запроса.
		/// </summary>
		public void AppendInt(
			ViewDataDictionary viewData,
			string key,
			int defaultValue = 0)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			AppendInt(key, viewData.GetInt(key, defaultValue));
		}


		/// <summary>
		/// Копирует целочисленное значение из другого хелпера параметров запроса.
		/// </summary>
		public void AppendInt(
			QueryStringHelper helper,
			string key,
			int defaultValue = 0)
		{
			ArgumentNullException.ThrowIfNull(helper);
			AppendInt(key, helper.GetInt(key, defaultValue));
		}


		/*--- bool ---*/


		/// <summary>
		/// Записывает логическое значение флага (записывается только состояние true).
		/// </summary>
		public void AppendBool(
			string key,
			bool value)
		{
			if (value)
				AppendString(key, "true");
		}


		/// <summary>
		/// Переносит логическое значение из словаря ViewData в параметры запроса.
		/// </summary>
		public void AppendBool(
			ViewDataDictionary viewData,
			string key)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			AppendBool(key, viewData.GetBool(key));
		}


		/// <summary>
		/// Копирует логическое значение из другого хелпера параметров запроса.
		/// </summary>
		public void AppendBool(
			QueryStringHelper helper,
			string key)
		{
			ArgumentNullException.ThrowIfNull(helper);
			AppendBool(key, helper.GetBool(key));
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Записывает дату и время в каноническом формате ISO 8601 (s).
		/// </summary>
		public void AppendDateTime(
			string key,
			DateTime? value)
		{
			if (value.HasValue)
				AppendString(key, value.Value.ToString("s"));
		}


		/// <summary>
		/// Переносит дату и время из словаря ViewData в параметры запроса.
		/// </summary>
		public void AppendDateTime(
			ViewDataDictionary viewData,
			string key,
			DateTime? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			var value = defaultValue.HasValue
				? viewData.GetDateTime(key, defaultValue.Value)
				: viewData.GetDateTime(key);
			AppendDateTime(key, value);
		}


		/// <summary>
		/// Копирует дату и время из другого хелпера параметров запроса.
		/// </summary>
		public void AppendDateTime(
			QueryStringHelper helper,
			string key,
			DateTime? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(helper);
			var value = defaultValue.HasValue
				? helper.GetDateTime(key, defaultValue.Value)
				: helper.GetDateTime(key);
			AppendDateTime(key, value);
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Записывает календарную дату в параметры запроса.
		/// </summary>
		public void AppendDateOnly(
			string key,
			DateOnly? value)
		{
			if (value.HasValue)
				AppendString(key, value.Value.ToString());
		}


		/// <summary>
		/// Переносит календарную дату из словаря ViewData в параметры запроса.
		/// </summary>
		public void AppendDateOnly(
			ViewDataDictionary viewData,
			string key,
			DateOnly? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			var value = defaultValue.HasValue
				? viewData.GetDateOnly(key, defaultValue.Value)
				: viewData.GetDateOnly(key);
			AppendDateOnly(key, value);
		}


		/// <summary>
		/// Копирует календарную дату из другого хелпера параметров запроса.
		/// </summary>
		public void AppendDateOnly(
			QueryStringHelper helper,
			string key,
			DateOnly? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(helper);
			var value = defaultValue.HasValue
				? helper.GetDateOnly(key, defaultValue.Value)
				: helper.GetDateOnly(key);
			AppendDateOnly(key, value);
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Записывает время суток в параметры запроса.
		/// </summary>
		public void AppendTimeOnly(
			string key,
			TimeOnly? value)
		{
			if (value.HasValue)
				AppendString(key, value.Value.ToString());
		}


		/// <summary>
		/// Переносит время суток из словаря ViewData в параметры запроса.
		/// </summary>
		public void AppendTimeOnly(
			ViewDataDictionary viewData,
			string key,
			TimeOnly? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(viewData);
			var value = defaultValue.HasValue
				? viewData.GetTimeOnly(key, defaultValue.Value)
				: viewData.GetTimeOnly(key);
			AppendTimeOnly(key, value);
		}


		/// <summary>
		/// Копирует время суток из другого хелпера параметров запроса.
		/// </summary>
		public void AppendTimeOnly(
			QueryStringHelper helper,
			string key,
			TimeOnly? defaultValue = null)
		{
			ArgumentNullException.ThrowIfNull(helper);
			var value = defaultValue.HasValue
				? helper.GetTimeOnly(key, defaultValue.Value)
				: helper.GetTimeOnly(key);
			AppendTimeOnly(key, value);
		}

	}

}
