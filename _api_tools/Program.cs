using System.Reflection;
using System.Xml.Linq;

namespace _api_tools
{

	class Program
	{

		static void Main(
			string[] args)
		{
			if (args?.Length != 1)
			{
				Console.WriteLine("Ошибка: Требуется имя файла документации.");
				return;
			}
			var path1 = Directory.GetCurrentDirectory();
			var libDocName1 = args[0];
			GetLibApi(path1, libDocName1);

			//GetApi(
			//	@"D:\REPOS\NET10\_Libs\Ans.Net10\Ans.Net10.Common\bin\Debug\net10.0\Ans.Net10.Common.dll",
			//	@"D:\REPOS\NET10\_Libs\Ans.Net10\Ans.Net10.Common\LibCommonDocs.xml",
			//	@"D:\REPOS\NET10\_Libs\Ans.Net10\Ans.Net10.Common\LibCommonDocs.txt");
		}



		static void GetLibApi(
			string libPath,
			string libDocName)
		{
			var libName1 = libPath.Split('\\').Last();
			GetApi(
				$"{libPath}/bin/Debug/net10.0/{libName1}.dll",
				$"{libPath}/{libDocName}.xml",
				$"{libPath}/{libDocName}.txt");
		}



		static void GetApi(
			string dllPath,
			string xmlPath,
			string outputPath)
		{
			if (!File.Exists(dllPath) || !File.Exists(xmlPath))
			{
				Console.WriteLine("Ошибка: Убедитесь, что оба файла (.dll и .xml) существуют.");
				return;
			}

			Console.WriteLine($"DLL: {dllPath}");
			Console.WriteLine($"DOC: {xmlPath}");
			Console.WriteLine($"API: {outputPath}");

			try
			{
				var xmlDoc1 = XDocument.Load(xmlPath);
				var assembly1 = Assembly.LoadFrom(dllPath);

				using (var writer1 = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8))
				{
					writer1.WriteLine($"=== ПУБЛИЧНОЕ API СБОРКИ: {assembly1.GetName().Name} ===");
					writer1.WriteLine($"Дата генерации: {DateTime.Now}\n");

					var publicTypes1 = assembly1.GetExportedTypes();

					foreach (var type1 in publicTypes1)
					{
						// Документация типа (класса/интерфейса)
						string typeXmlKey1 = $"T:{type1.FullName}";
						string typeSummary1 = GetXmlSummary(xmlDoc1, typeXmlKey1);

						writer1.WriteLine($"\nТип: {GetReadableTypeName(type1)}");
						if (!string.IsNullOrEmpty(typeSummary1))
							writer1.WriteLine($"   Описание: {typeSummary1}");

						// 1. Конструкторы
						var ctors1 = type1.GetConstructors();
						if (ctors1.Length != 0)
						{
							writer1.WriteLine("   Конструкторы:");
							foreach (var ctor1 in ctors1)
							{
								string ctorKey1 = $"M:{type1.FullName}.#ctor{GetXmlParametersString(ctor1.GetParameters())}";
								writer1.WriteLine($"      - {ctor1}");
								WriteMemberDetails(writer1, xmlDoc1, ctorKey1, ctor1.GetParameters());
							}
						}

						// 2. Свойства
						var props1 = type1.GetProperties();
						if (props1.Length != 0)
						{
							writer1.WriteLine("   Свойства:");
							foreach (var prop1 in props1)
							{
								string propKey1 = $"P:{type1.FullName}.{prop1.Name}";
								string propSummary1 = GetXmlSummary(xmlDoc1, propKey1);
								writer1.WriteLine($"      - {prop1.PropertyType.Name} {prop1.Name}");
								if (!string.IsNullOrEmpty(propSummary1))
									writer1.WriteLine($"        Описание: {propSummary1}");
							}
						}

						// 3. Методы
						var methods1 = type1.GetMethods(
							BindingFlags.Public
							| BindingFlags.Instance
							| BindingFlags.Static
							| BindingFlags.DeclaredOnly)
							.Where(m => !m.IsSpecialName); // Исключаем геттеры/сеттеры
						if (methods1.Any())
						{
							writer1.WriteLine("   Методы:");
							foreach (var method1 in methods1)
							{
								string methodKey1 = $"M:{type1.FullName}.{method1.Name}{GetXmlParametersString(method1.GetParameters())}";
								writer1.WriteLine(
									$"      - {method1.ReturnType.Name} {method1.Name}({string.Join(", ", method1.GetParameters().Select(p => p.ParameterType.Name))})");
								WriteMemberDetails(writer1, xmlDoc1, methodKey1, method1.GetParameters());
							}
						}

						writer1.WriteLine(new string('-', 60));
					}
				}

				Console.WriteLine(
					$"Файл успешно сохранен по пути: {outputPath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"Произошла ошибка: {ex.Message}");
			}
		}



		static string GetXmlSummary(
			XDocument doc,
			string key)
		{
			var member1 = doc.Descendants("member")
				.FirstOrDefault(x => x.Attribute("name")?.Value == key);
			return member1?.Element("summary")?.Value.Trim() ?? string.Empty;
		}



		static void WriteMemberDetails(
			StreamWriter writer,
			XDocument doc,
			string key,
			ParameterInfo[] parameters)
		{
			var member1 = doc.Descendants("member")
				.FirstOrDefault(x => x.Attribute("name")?.Value == key);
			if (member1 == null)
				return;

			string? summary1 = member1.Element("summary")?.Value.Trim();
			if (!string.IsNullOrEmpty(summary1))
				writer.WriteLine(
					$"        Описание: {summary1}");

			foreach (var param1 in parameters)
			{
				string? paramDoc1 = member1.Elements("param")
					.FirstOrDefault(x => x.Attribute("name")?.Value == param1.Name)?.Value.Trim();
				if (!string.IsNullOrEmpty(paramDoc1))
					writer.WriteLine(
						$"        * Параметр {param1.Name}: {paramDoc1}");
			}

			string? returns = member1.Element("returns")?.Value.Trim();
			if (!string.IsNullOrEmpty(returns))
				writer.WriteLine(
					$"        * Возвращает: {returns}");
		}



		static string GetXmlParametersString(
			ParameterInfo[] parameters)
		{
			if (parameters.Length == 0)
				return string.Empty;
			return $"({string.Join(",", parameters.Select(x => x.ParameterType.FullName))})";
		}



		static string GetReadableTypeName(
			Type type)
		{
			if (type.IsInterface)
				return $"Interface {type.FullName}";
			if (type.IsValueType && !type.IsEnum)
				return $"Struct {type.FullName}";
			if (type.IsEnum)
				return $"Enum {type.FullName}";
			return $"Class {type.FullName}";
		}

	}

}
