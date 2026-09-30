// rev 2026-09-30

namespace Ans.Net10.Web
{

	/// <summary>
	/// Помощник для парсинга и генерации разрешений (Claims) на основе компактного строкового описания 
	/// структуры каталогов, контроллеров и действий MVC.
	/// </summary>
	public class MvcAccessHelper
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="MvcAccessHelper"/> и выполняет разбор переданной строки.
		/// </summary>
		/// <param name="def">Строка определений каталогов, разделенная точкой с запятой (<c>;</c>). Пример: <c>"Catalog1;Catalog2"</c>.</param>
		public MvcAccessHelper(
			string def)
		{
			if (string.IsNullOrEmpty(def))
				return;
			var tokens1 = def.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			Catalogs = Array.ConvertAll(tokens1, x => new MvcAccessCatalog(x));
		}


		/* readonly properties */


		/// <summary>
		/// Получает неизменяемый список распарсенных каталогов доступа.
		/// </summary>
		public IReadOnlyList<MvcAccessCatalog> Catalogs { get; } = [];


		/* functions */


		/// <summary>
		/// Генерирует плоский массив строковых значений разрешений (Claims) в формате <c>"Каталог.Контроллер.Действие"</c>.
		/// </summary>
		/// <returns>Массив строк с уникальными именами разрешений. Если структура пуста, возвращает пустой массив.</returns>
		public string[] GetResultClaimsValue()
		{
			if (Catalogs.Count == 0)
				return [];
			var result1 = new List<string>(Catalogs.Count * 2);
			foreach (var catalog1 in Catalogs)
			{
				var controllers1 = catalog1.Controllers;
				if (controllers1 is { Count: > 0 })
				{
					foreach (var controller1 in controllers1)
					{
						var catalogAndController1 = $"{catalog1.Name}.{controller1.Name}";
						var actions1 = controller1.Actions;
						if (actions1 is { Count: > 0 })
							foreach (var action1 in actions1)
								result1.Add($"{catalogAndController1}.{action1}");
						else
							result1.Add(catalogAndController1);
					}
				}
				else
					result1.Add(catalog1.Name);
			}
			return [.. result1];
		}

	}



	/// <summary>
	/// Описывает каталог (модуль) доступа, содержащий вложенную структуру MVC-контроллеров.
	/// </summary>
	public class MvcAccessCatalog
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="MvcAccessCatalog"/> на основе строкового DSL.
		/// </summary>
		/// <param name="def">Строка определения каталога и контроллеров. Пример: <c>"Admin>Home,Users,Settings"</c>.</param>
		public MvcAccessCatalog(
			string def)
		{
			ArgumentException.ThrowIfNullOrEmpty(def);
			var parts1 = def.Split('>', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			Name = parts1[0];
			if (parts1.Length > 1)
			{
				var tokens1 = parts1[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				Controllers = Array.ConvertAll(tokens1, x => new MvcAccessController(x));
			}
		}


		/* readonly properties */


		/// <summary>
		/// Получает системное имя каталога доступа.
		/// </summary>
		public string Name { get; } = string.Empty;


		/// <summary>
		/// Получает список распарсенных контроллеров, принадлежащих данному каталогу.
		/// </summary>
		public IReadOnlyList<MvcAccessController> Controllers { get; } = [];

	}



	/// <summary>
	/// Описывает MVC-контроллер доступа, содержащий атомарные списки разрешенных действий (Actions).
	/// </summary>
	public class MvcAccessController
	{

		/* ctor */


		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="MvcAccessController"/> на основе строкового DSL.
		/// </summary>
		/// <param name="def">Строка определения контроллера и его экшенов. Пример: <c>"Users=Create+Update+Delete"</c>.</param>
		public MvcAccessController(
			string def)
		{
			ArgumentException.ThrowIfNullOrEmpty(def);
			var parts1 = def.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			Name = parts1[0];
			if (parts1.Length > 1)
				Actions = parts1[1].Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}


		/* readonly properties */


		/// <summary>
		/// Получает системное имя MVC-контроллера.
		/// </summary>
		public string Name { get; } = string.Empty;


		/// <summary>
		/// Получает список имен действий (Actions) текущего контроллера.
		/// </summary>
		public IReadOnlyList<string> Actions { get; } = [];

	}

}
