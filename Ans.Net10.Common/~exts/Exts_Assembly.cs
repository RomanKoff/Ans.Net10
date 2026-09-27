// rev 2026-09-26

using System.Reflection;

namespace Ans.Net10.Common
{

	/// <summary>
	/// Методы расширения для извлечения метаданных, версий и описаний из экземпляров <see cref="Assembly"/>.
	/// </summary>
	public static partial class Exts_Assembly
	{

		extension(Assembly? instance)
		{

			/* readonly properties */


			/// <summary>
			/// Возвращает имя сборки.
			/// </summary>
			/// <value>
			/// Строка с именем сборки. Если объект сборки равен <see langword="null"/>, 
			/// возвращает дефолтное значение <c>"UnknownAssembly"</c>.
			/// </value>
			public string Name
				=> instance switch
				{
					null => "UnknownAssembly",
					_ => instance.GetName().Name ?? instance.ToString()
				};


			/// <summary>
			/// Возвращает версию сборки без учета метаданных сборки (например, хэша коммита Git).
			/// </summary>
			/// <value>
			/// Чистая строка версии (например, <c>"1.0.0"</c>). Если объект сборки равен <see langword="null"/> 
			/// или версию определить не удалось, возвращает <c>"1.0.0"</c>.
			/// </value>
			public string Version
			{
				get
				{
					if (instance is null)
						return "1.0.0";
					if (instance.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
						is { InformationalVersion: { } info1 })
					{
						var span1 = info1.AsSpan();
						var i1 = span1.IndexOf('+');
						return i1 == -1
							? info1 : span1[..i1].ToString();
					}
					return instance.GetName().Version?.ToString() ?? "1.0.0";
				}
			}


			/// <summary>
			/// Возвращает полную информационную версию сборки, включая хэш коммита Git (при его наличии).
			/// </summary>
			/// <value>
			/// Полная строка информационной версии сборки. Если объект сборки равен <see langword="null"/>, 
			/// возвращает <c>"1.0.0"</c>.
			/// </value>
			public string FullVersion
			{
				get
				{
					if (instance is null)
						return "1.0.0";
					return instance.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
						?? instance.GetName().Version?.ToString()
						?? "1.0.0";
				}
			}


			/// <summary>
			/// Возвращает текстовое описание сборки из примененных метаданных.
			/// </summary>
			/// <value>
			/// Текст описания сборки из атрибута <see cref="AssemblyDescriptionAttribute"/> 
			/// или <see langword="null"/>, если описание отсутствует или объект сборки равен <see langword="null"/>.
			/// </value>
			public string? Description
				=> instance?.GetCustomAttribute<AssemblyDescriptionAttribute>()
					is { Description: { Length: > 0 } desc1 }
					? desc1 : null;

		}

	}

}
