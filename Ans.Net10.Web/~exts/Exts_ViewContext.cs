// rev 2026-09-30

using Ans.Net10.Common;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для <see cref="ViewContext"/>, упрощающие изоляцию параметров маршрутизации 
	/// и обеспечивающие высокопроизводительный рендеринг Razor-представлений.
	/// </summary>
	public static partial class Exts_ViewContext
	{

		/// <summary>
		/// Асинхронно рендерит Razor-представление в текстовую строку с использованием внедренной службы <see cref="IViewRenderService"/>.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="viewName">Имя или полный виртуальный путь к представлению Razor.</param>
		/// <param name="model">Объект доменной модели данных, передаваемый внутрь Razor-шаблона.</param>
		/// <returns>Поток-задача, возвращающая отрендеренный HTML-код в виде строки.</returns>
		public async static Task<string> RenderViewAsync(
			this ViewContext context,
			string viewName,
			object model)
		{
			var engine1 = context.HttpContext.RequestServices
				.GetRequiredService<IViewRenderService>();
			return await engine1.RenderViewToStringAsync(viewName, model);
		}


		/*--- string ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и возвращает его строковое представление.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Строковое значение параметра маршрута или <see langword="null"/>, если параметр не найден.</returns>
		public static string? GetRouteValueAsString(
			this ViewContext context,
			string key)
		{
			return context.HttpContext.GetRouteValue(key)?.ToString();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия параметра.</param>
		/// <returns>Строковое значение параметра маршрута либо <paramref name="defaultValue"/>.</returns>
		public static string GetRouteValueAsString(
			this ViewContext context,
			string key,
			string defaultValue)
		{
			return context.GetRouteValueAsString(key) ?? defaultValue;
		}


		/*--- int ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в 32-битное целое число со знаком.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Преобразованное целое число, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static int? GetRouteValueAsInt(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToInt();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде целого числа или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Целочисленное значение параметра маршрута либо <paramref name="defaultValue"/>.</returns>
		public static int GetRouteValueAsInt(
			this ViewContext context,
			string key,
			int defaultValue)
		{
			return context.GetRouteValueAsInt(key) ?? defaultValue;
		}


		/*--- uint ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в 32-битное целое число без знака.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Преобразованное число без знака, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static uint? GetRouteValueAsUInt(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToUInt();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде числа без знака или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="uint"/> либо <paramref name="defaultValue"/>.</returns>
		public static uint GetRouteValueAsUInt(
			this ViewContext context,
			string key,
			uint defaultValue)
		{
			return context.GetRouteValueAsUInt(key) ?? defaultValue;
		}


		/*--- long ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в 64-битное целое число со знаком.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Преобразованное число типа <see cref="long"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static long? GetRouteValueAsLong(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToLong();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде 64-битного целого числа или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="long"/> либо <paramref name="defaultValue"/>.</returns>
		public static long GetRouteValueAsLong(
			this ViewContext context,
			string key,
			long defaultValue)
		{
			return context.GetRouteValueAsLong(key) ?? defaultValue;
		}


		/*--- double ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в число с плавающей запятой двойной точности.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Преобразованное число типа <see cref="double"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static double? GetRouteValueAsDouble(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToDouble();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде числа двойной точности или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="double"/> либо <paramref name="defaultValue"/>.</returns>
		public static double GetRouteValueAsDouble(
			this ViewContext context,
			string key,
			double defaultValue)
		{
			return context.GetRouteValueAsDouble(key) ?? defaultValue;
		}


		/*--- float ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в число с плавающей запятой одинарной точности.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Преобразованное число типа <see cref="float"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static float? GetRouteValueAsFloat(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToFloat();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде числа одинарной точности или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="float"/> либо <paramref name="defaultValue"/>.</returns>
		public static float GetRouteValueAsFloat(
			this ViewContext context,
			string key,
			float defaultValue)
		{
			return context.GetRouteValueAsFloat(key) ?? defaultValue;
		}


		/*--- decimal ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в высокоточное десятичное число.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Преобразованное число типа <see cref="decimal"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static decimal? GetRouteValueAsDecimal(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToDecimal();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде высокоточного десятичного числа или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="decimal"/> либо <paramref name="defaultValue"/>.</returns>
		public static decimal GetRouteValueAsDecimal(
			this ViewContext context,
			string key,
			decimal defaultValue)
		{
			return context.GetRouteValueAsDecimal(key) ?? defaultValue;
		}


		/*--- DateTime ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в объект даты и времени.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Объект структуры <see cref="DateTime"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static DateTime? GetRouteValueAsDateTime(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToDateTime();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде объекта даты и времени или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="DateTime"/> либо <paramref name="defaultValue"/>.</returns>
		public static DateTime GetRouteValueAsDateTime(
			this ViewContext context,
			string key,
			DateTime defaultValue)
		{
			return context.GetRouteValueAsDateTime(key) ?? defaultValue;
		}


		/*--- DateOnly ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в календарную дату без времени.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Объект структуры <see cref="DateOnly"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static DateOnly? GetRouteValueAsDateOnly(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToDateOnly();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде календарной даты без времени или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="DateOnly"/> либо <paramref name="defaultValue"/>.</returns>
		public static DateOnly GetRouteValueAsDateOnly(
			this ViewContext context,
			string key,
			DateOnly defaultValue)
		{
			return context.GetRouteValueAsDateOnly(key) ?? defaultValue;
		}


		/*--- TimeOnly ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в время суток без привязки к дате.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns>Объект структуры <see cref="TimeOnly"/>, либо <see langword="null"/>, если параметр не найден или имеет некорректный формат.</returns>
		public static TimeOnly? GetRouteValueAsTimeOnly(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key)?.ToTimeOnly();
		}


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу в виде времени суток или возвращает заданное значение по умолчанию.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <param name="defaultValue">Значение, возвращаемое в случае отсутствия или некорректного формата параметра.</param>
		/// <returns>Значение параметра маршрута типа <see cref="TimeOnly"/> либо <paramref name="defaultValue"/>.</returns>
		public static TimeOnly GetRouteValueAsTimeOnly(
			this ViewContext context,
			string key,
			TimeOnly defaultValue)
		{
			return context.GetRouteValueAsTimeOnly(key) ?? defaultValue;
		}


		/*--- bool ---*/


		/// <summary>
		/// Извлекает значение параметра маршрутизации по указанному ключу и преобразует его в логическое значение.
		/// </summary>
		/// <param name="context">Текущий контекст представления <see cref="ViewContext"/>.</param>
		/// <param name="key">Системный ключ параметра маршрута.</param>
		/// <returns><see langword="true"/>, если значение эквивалентно маркерам "1", "+" или "true"; иначе — <see langword="false"/>.</returns>
		public static bool GetRouteValueAsBool(
			this ViewContext context,
			string key)
		{
			return context.GetRouteValueAsString(key) is string item1 && item1.ToBool();
		}

	}

}
