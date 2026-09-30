// rev 2026-09-30

using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace Ans.Net10.Web
{

	/// <summary>
	/// Методы расширения для интерфейса <see cref="IConfiguration"/>, обеспечивающие удобное 
	/// извлечение параметров и метаданных конфигурации приложения.
	/// </summary>
	public static partial class Exts_IConfiguration
	{

		private static readonly string[] _dbKeySynonyms
			= ["Database", "Initial Catalog", "InitialCatalog", "Data Source"];


		/// <summary>
		/// Извлекает имя базы данных из указанной строки подключения по её системному имени.
		/// </summary>
		/// <remarks>
		/// Метод поддерживает различные форматы и синонимы параметров строк подключения 
		/// основных СУБД (<c>Database=...</c>, <c>Initial Catalog=...</c>, <c>Data Source=...</c>).
		/// </remarks>
		/// <param name="configuration">Текущая конфигурация приложения.</param>
		/// <param name="connectionStringName">Системное имя строки подключения в секции <c>ConnectionStrings</c>.</param>
		/// <returns>
		/// Имя базы данных, извлеченное из параметров строки подключения, или <see langword="null"/>, 
		/// если строка подключения не найдена, пуста или не содержит имя базы данных.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Вызывается, если параметр <paramref name="configuration"/> равен <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Вызывается, если параметр <paramref name="connectionStringName"/> является пустой строкой.
		/// </exception>
		public static string? GetDatabaseName(
			this IConfiguration configuration,
			string connectionStringName)
		{
			ArgumentNullException.ThrowIfNull(configuration);
			ArgumentException.ThrowIfNullOrEmpty(connectionStringName);
			var connectionString1 = configuration.GetConnectionString(connectionStringName);
			if (string.IsNullOrEmpty(connectionString1))
				return null;
			try
			{
				var builder1 = new DbConnectionStringBuilder
				{
					ConnectionString = connectionString1
				};
				foreach (var key1 in _dbKeySynonyms)
					if (builder1.TryGetValue(key1, out var value1)
						&& value1 is string dbName1)
					{
						var trimmedName1 = dbName1.Trim();
						if (!string.IsNullOrEmpty(trimmedName1))
							return trimmedName1;
					}
			}
			catch
			{
				// Игнорируем исключения парсинга невалидной строки, возвращая null согласно контракту метода
			}
			return null;
		}

	}

}
