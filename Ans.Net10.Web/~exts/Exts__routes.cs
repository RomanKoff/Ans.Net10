// rev 2026-09-28

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для интерфейса <see cref="IEndpointRouteBuilder"/>, обеспечивающие 
	/// декларативную регистрацию контроллерных маршрутов (Routes) из строковых конфигураций.
	/// </summary>
	public static partial class Exts__routes
	{

		/// <summary>
		/// Добавляет и регистрирует маршрут контроллера из одной форматированной строки-определения.
		/// </summary>
		/// <param name="endpoints">Текущий строитель конечных точек приложения.</param>
		/// <param name="routeDef">Строка определения маршрута в формате: <c>name|template|controller|action</c>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="endpoints"/> равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Вызывается, если строка <paramref name="routeDef"/> пуста или равна <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Вызывается, если строка <paramref name="routeDef"/> не содержит ровно 4 сегмента, разделенных символом пайпа '|'.</exception>
		public static void AddRoute(
			this IEndpointRouteBuilder endpoints,
			string routeDef)
		{
			ArgumentNullException.ThrowIfNull(endpoints);
			ArgumentException.ThrowIfNullOrEmpty(routeDef);
			var parts1 = routeDef.Split('|');
			if (parts1.Length != 4)
				throw new ArgumentOutOfRangeException(
					nameof(routeDef),
					routeDef,
					"[Ans.Net10.Web] Маршрут должен строго соответствовать формату: 'name|template|controller|action'");
			endpoints.MapControllerRoute(
				name: parts1[0],
				pattern: parts1[1],
				defaults: new { controller = parts1[2], action = parts1[3] });
		}


		/// <summary>
		/// Добавляет и регистрирует коллекцию маршрутов из массива строк.
		/// </summary>
		/// <param name="endpoints">Текущий строитель конечных точек приложения.</param>
		/// <param name="routeDefs">Массив строк-определений маршрутов.</param>
		/// <exception cref="ArgumentNullException">Вызывается, если <paramref name="endpoints"/> равен <see langword="null"/>, либо если массив <paramref name="routeDefs"/> пуст.</exception>
		public static void AddRoutes(
			this IEndpointRouteBuilder endpoints,
			params string[] routeDefs)
		{
			ArgumentNullException.ThrowIfNull(endpoints);
			if (routeDefs == null || routeDefs.Length == 0)
				throw new ArgumentNullException(
					nameof(routeDefs),
					"[Ans.Net10.Web] Список определений маршрутов не может быть пустым.");
			foreach (var routeDef1 in routeDefs)
				if (!string.IsNullOrWhiteSpace(routeDef1))
					endpoints.AddRoute(routeDef1);
		}


		/// <summary>
		/// Добавляет и регистрирует коллекцию маршрутов из одной строки, где маршруты разделены точкой с запятой.
		/// </summary>
		/// <param name="endpoints">Текущий строитель конечных точек приложения.</param>
		/// <param name="routeDefs">Строка с маршрутами, разделенными символом ';'. Например: <c>"r1|t1|c1|a1;r2|t2|c2|a2"</c>.</param>
		/// <exception cref="ArgumentNullException">Вызывается, if <paramref name="endpoints"/> равен <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Вызывается, если строка <paramref name="routeDefs"/> пуста или равна <see langword="null"/>.</exception>
		public static void AddRoutes(
			this IEndpointRouteBuilder endpoints,
			string routeDefs)
		{
			ArgumentNullException.ThrowIfNull(endpoints);
			ArgumentException.ThrowIfNullOrEmpty(routeDefs);
			var routeArray1 = routeDefs.Split(';', StringSplitOptions.RemoveEmptyEntries);
			endpoints.AddRoutes(routeArray1);
		}

	}

}
