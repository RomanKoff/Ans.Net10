using Ans.Net10.Common;
using Ans.Net10.Common.Crud;
using Ans.Net10.Common.Json;
using Ans.Net10.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace _test_common
{

	internal class Program
	{

		static readonly IServiceCollection _Services = new ServiceCollection();
		static readonly string _ApiUrl = "https://api.guap.ru/rasp-sem/v1/get-info";


		static void Main()
		{
			SuppCulture.AddCodePagesSupport();
			Console.InputEncoding = Encoding.UTF8;
			Console.OutputEncoding = Encoding.UTF8;

			SuppConsole.AppStart();

			SuppConsole.WriteLineParam("App.Name", SuppApp.EntryAssembly.Name);
			SuppConsole.WriteLineParam("App.Version", SuppApp.EntryAssembly.Version);
			SuppConsole.WriteLineParam("App.FullVersion", SuppApp.EntryAssembly.FullVersion);
			SuppConsole.WriteLineParam("App.Description", SuppApp.EntryAssembly.Description);
			Console.WriteLine();

			SuppConsole.WriteLineParam("Ans.Net10.Common.Name", LibCommonInfo.Name);
			SuppConsole.WriteLineParam("Ans.Net10.Common.Version", LibCommonInfo.Version);
			SuppConsole.WriteLineParam("Ans.Net10.Common.FullVersion", LibCommonInfo.FullVersion);
			SuppConsole.WriteLineParam("Ans.Net10.Common.Description", LibCommonInfo.Description);
			Console.WriteLine();

			SuppConsole.WriteLineParam("SuppApp.CurrentDirectory", SuppApp.CurrentDirectory);
			SuppConsole.WriteLineParam("SuppApp.BaseDirectory", SuppApp.BaseDirectory);
			SuppConsole.WriteLineParam("SuppApp.VSProjectName", SuppApp.VSProjectName);
			SuppConsole.WriteLineParam("SuppApp.VSProjectPath", SuppApp.VSProjectPath);
			SuppConsole.WriteLineParam("SuppApp.VSSolutionName", SuppApp.VSSolutionName);
			SuppConsole.WriteLineParam("SuppApp.VSSolutionPath", SuppApp.VSSolutionPath);
			Console.WriteLine();


			_outConsts();
			_outExtsAsync().GetAwaiter().GetResult();
			_outSuppsAsync().GetAwaiter().GetResult();
			_outClassesAsync().GetAwaiter().GetResult();
			_outServices();


			SuppConsole.AppEnd();

		}







		/* privates */


		static void _outConsts()
		{
			SuppConsole.SectionStart("CONSTS");

			SuppConsole.WriteLineParam("_Consts.GetRandomSampleRu()", _Consts.GetRandomSampleRu());
			SuppConsole.WriteLineParam("_Consts.GetRandomSampleSmallRu()", _Consts.GetRandomSampleSmallRu());
			SuppConsole.WriteLineParam("_Consts.GetRandomSampleSmallerRu()", _Consts.GetRandomSampleSmallerRu());
			SuppConsole.WriteLineParam("_Consts.FORBIDDEN_FILE_NAMES.Length", _Consts.FORBIDDEN_FILE_NAMES.Length);
			SuppConsole.WriteLineParam("_Consts.FORBIDDEN_FILE_NAMES[0]", _Consts.FORBIDDEN_FILE_NAMES[0]);
			SuppConsole.WriteLineParam("_Consts.CONTENTINFO_BIN", _Consts.CONTENTINFO_BIN);
			SuppConsole.WriteLineParam("_Consts.CONTENTINFOS[\".JPG\"]", _Consts.CONTENTINFOS[".JPG"]);
			SuppConsole.WriteLineParam("_Consts.CONTENTINFOS.Count", _Consts.CONTENTINFOS.Count);
			SuppConsole.WriteLineParam("_Consts.ENCODING_UTF8.WebName", _Consts.ENCODING_UTF8.WebName);
			SuppConsole.WriteLineParam("_Consts.ENCODING_WINDOWS1251.WebName", _Consts.ENCODING_WINDOWS1251.WebName);
			SuppConsole.WriteLineParam("_Consts.ENCODING_KOI8R.WebName", _Consts.ENCODING_KOI8R.WebName);
			SuppConsole.WriteLineParam("_Consts.ENCODING_CP866.WebName", _Consts.ENCODING_CP866.WebName);
			SuppConsole.WriteLineParam("_Consts.ENCODING_ISO88591.WebName", _Consts.ENCODING_ISO88591.WebName);

			bool isEmailValid = _Consts.G_REGEX_EMAIL().IsMatch("test.user@example.com");
			SuppConsole.WriteLineParam(
				"Валидация корректного Email (test.user@example.com)",
				isEmailValid);
			bool isUpperEmailValid = _Consts.G_REGEX_EMAIL().IsMatch("ADMIN@DOMAIN.RU");
			SuppConsole.WriteLineParam(
				"Валидация корректного Email в ВЕРХНЕМ регистре",
				isUpperEmailValid);
			string textWithNumbers = "Код 12, весит 4.5 кг, индекс -1";
			var matches = _Consts.G_REGEX_SMALLNUMBER().Matches(textWithNumbers);
			string[] foundValues = new string[matches.Count];
			for (int i = 0; i < matches.Count; i++)
				foundValues[i] = matches[i].Value;
			SuppConsole.WriteLineParam(
				"Поиск коротких чисел в строке",
				SuppConsole.ArrToText(foundValues));
			string dirtyText = "Текст    с    множеством      пробелов.";
			string cleanText = _Consts.G_REGEX_MULTISPACE().Replace(dirtyText, " ");
			SuppConsole.WriteLineParam("Очистка текста от лишних пробелов", cleanText);

		}


		static async Task _outExtsAsync()
		{
			SuppConsole.SectionStart("EXTS");



			SuppConsole.PartStart("Exts__collections");

			var a1 = new[] { 1, 2 };
			var a2 = new[] { 2, 3 };
			var a3 = new[] { 1, 3 };
			var a4 = new[] { 1, 2 };
			var a5 = new[] { 1, 2, 3 };
			var a6 = new[] { "A", "B" };
			var a7 = new[] { 10, 20, 30 };
			var a8 = new[] { 1, 2 };
			var a9 = new[] { 9 };
			var a10 = new[] { "A", "B", "C" };
			Console.WriteLine($"(Добавление в массив) [ 1, 2 ].GetArrayAdd(3) : {SuppConsole.ArrToText(a1.GetArrayAdd(3))}");
			Console.WriteLine($"(Добавление в null) ((int[]?)null).GetArrayAdd(9) : {SuppConsole.ArrToText(((int[]?)null).GetArrayAdd(9))}");
			Console.WriteLine($"(Вставка в начало) [ 2, 3 ].GetArrayInsert(0, 1) : {SuppConsole.ArrToText(a2.GetArrayInsert(0, 1))}");
			Console.WriteLine($"(Вставка в середину) [ 1, 3 ].GetArrayInsert(1, 2) : {SuppConsole.ArrToText(a3.GetArrayInsert(1, 2))}");
			Console.WriteLine($"(Вставка в конец) [ 1, 2 ].GetArrayInsert(2, 3) : {SuppConsole.ArrToText(a4.GetArrayInsert(2, 3))}");
			Console.WriteLine($"(Удаление первого) [ 1, 2, 3 ].GetArrayRemoveAt(0) : {SuppConsole.ArrToText(a5.GetArrayRemoveAt(0))}");
			Console.WriteLine($"(Удаление единственного) [ 9 ].GetArrayRemoveAt(0).Length : {a9.GetArrayRemoveAt(0).Length}");
			Console.WriteLine($"(Удаление существующего) [ \"A\", \"B\", \"C\" ].GetArrayRemove(\"B\")) : {SuppConsole.ArrToText(a10.GetArrayRemove("B"))}");
			Console.WriteLine($"(Элемент не найден) [ \"A\", \"B\" ].GetArrayRemove(\"X\")) : {SuppConsole.ArrToText(a6.GetArrayRemove("X"))}");
			Console.WriteLine($"(Удаление из середины) [ 10, 20, 30 ].GetArrayRemoveAt(1)) : {SuppConsole.ArrToText(a7.GetArrayRemoveAt(1))}");
			try
			{
				Console.WriteLine($"(Проверка выхода за границы) new[] [ 1, 2 ].GetArrayRemoveAt(5) : {a8.GetArrayRemoveAt(5)}");
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Console.WriteLine($"(Перехвачено исключение) Ошибка: {ex.Message.Split('\r')[0]}");
			}

			int[]? nullArray = null;
			int[] emptyArray = [];
			int[] workingArray = [1, 2, 3, 4, 5];
			Console.WriteLine($"Массив: {SuppConsole.ArrToText(workingArray)}");
			Console.WriteLine($"Добавление в null: {SuppConsole.ArrToText(nullArray.GetArrayAdd(99))}");
			Console.WriteLine($"Добавление в пустой: {SuppConsole.ArrToText(emptyArray.GetArrayAdd(99))}");
			Console.WriteLine($"Добавление в конец: {SuppConsole.ArrToText(workingArray.GetArrayAdd(40))}");
			Console.WriteLine($"Вставка в null на индекс 0: {SuppConsole.ArrToText(nullArray.GetArrayInsert(0, 99))}");
			Console.WriteLine($"Вставка в начало: {SuppConsole.ArrToText(workingArray.GetArrayInsert(0, 5))}");
			Console.WriteLine($"Вставка в середину: {SuppConsole.ArrToText(workingArray.GetArrayInsert(2, 15))}");
			Console.WriteLine($"Вставка в конец: {SuppConsole.ArrToText(workingArray.GetArrayInsert(5, 40))}");
			Console.WriteLine($"Удаление первого элемента: {SuppConsole.ArrToText(workingArray.GetArrayRemoveAt(0))}");
			Console.WriteLine($"Удаление из середины: {SuppConsole.ArrToText(workingArray.GetArrayRemoveAt(2))}");
			Console.WriteLine($"Удаление из конца: {SuppConsole.ArrToText(workingArray.GetArrayRemoveAt(4))}");
			Console.WriteLine($"Удаление существующего элемента: {SuppConsole.ArrToText(workingArray.GetArrayRemove(3))}");
			Console.WriteLine($"Удаление отсутствующего элемента: {SuppConsole.ArrToText(workingArray.GetArrayRemove(99))}");
			Console.WriteLine($"Удаление из null-массива: {SuppConsole.ArrToText(nullArray.GetArrayRemove(10))}");

			var currentData = new[] { (Id: 1, Name: "Ivan"), (Id: 2, Name: "Petr"), (Id: 3, Name: "Anna") };
			var newestData = new[] { (Id: 2, Name: "Petr_Updated"), (Id: 3, Name: "Anna"), (Id: 4, Name: "Sidor") };
#pragma warning disable IDE0042 // Deconstruct variable declaration
			var diffLazy = currentData.GetDiffLazy(newestData, x => x.Id);
			var diffList = currentData.GetDiffList(newestData, x => x.Id);
#pragma warning restore IDE0042 // Deconstruct variable declaration
			Console.WriteLine($"Current: {SuppConsole.ArrToText(currentData)}");
			Console.WriteLine($"Newest: {SuppConsole.ArrToText(newestData)}");
			Console.WriteLine($"GetDiff (Добавленные) : {SuppConsole.ArrToText(diffLazy.Added.Select(x => x.Name))}");
			Console.WriteLine($"GetDiff (Удаленные) : {SuppConsole.ArrToText(diffLazy.Removed.Select(x => x.Name))}");
			Console.WriteLine($"GetDiff (Обновленные) : {SuppConsole.ArrToText(diffLazy.Updated.Select(x => x.Name))}");
			Console.WriteLine($"GetDiffList (Добавленные) : {SuppConsole.ArrToText(diffList.Added.Select(x => x.Name))}");
			Console.WriteLine($"GetDiffList (Удаленные) : {SuppConsole.ArrToText(diffList.Removed.Select(x => x.Name))}");
			Console.WriteLine($"GetDiffList (Обновленные) : {SuppConsole.ArrToText(diffList.Updated.Select(x => x.Name))}");
			Console.WriteLine($"GetActual (Старые версии оставшихся) : {SuppConsole.ArrToText(currentData.GetActual(newestData, x => x.Id).Select(x => x.Name))}");
			Console.WriteLine($"GetUpdated (Новые версии измененных) : {SuppConsole.ArrToText(currentData.GetUpdated(newestData, x => x.Id).Select(x => x.Name))}");
			Console.WriteLine($"GetAdded (Только новые элементы) : {SuppConsole.ArrToText(currentData.GetAdded(newestData, x => x.Id).Select(x => x.Name))}");
			Console.WriteLine($"GetRemoved (Только удаленные элементы) : {SuppConsole.ArrToText(currentData.GetRemoved(newestData, x => x.Id).Select(x => x.Name))}");
			var rawStrings = new string?[] { "  Привет  ", "", null, "Мир", "  Привет  ", "   " };
			Console.WriteLine($"GetItemsClean (Удаление пробелов и пустот) : {SuppConsole.ArrToText(rawStrings.GetItemsClean())}");
			Console.WriteLine($"GetItemsTrim (Только обрезка пробелов с null) : {SuppConsole.ArrToText(rawStrings.GetItemsTrim())}");
			Console.WriteLine($"GetItemsWithoutEmpty (Удаление null и пустых строк) : {SuppConsole.ArrToText(rawStrings.GetItemsWithoutEmpty())}");
			Console.WriteLine($"GetItemsUnique (Поиск дубликатов по умолчанию) : {SuppConsole.ArrToText(rawStrings.GetItemsUnique())}");
			Console.WriteLine($"GetItemsCleanUnique (Комплексная очистка + уникальность) : {SuppConsole.ArrToText(rawStrings.GetItemsCleanUnique())}");



			SuppConsole.PartStart("Exts__datetime");

			var testDate = new DateTime(2026, 9, 14, 15, 30, 0);
			Console.WriteLine($"Проверка наличия времени суток (HasTimeOfDay) : {testDate.HasTimeOfDay()}");
			Console.WriteLine($"Специализированный строковый формат AnsDate : {testDate.GetAnsDate()}");
			Console.WriteLine($"Строковый формат AnsDateTime : {testDate.GetAnsDateTime()}");
			Console.WriteLine($"Формат даты, адаптированный для имени файла : {testDate.GetAnsDateTimeForFile()}");



			SuppConsole.PartStart("Exts__di");

			_Services.AddTransient(_ => "Тест");
			SuppConsole.WriteLineParam("_Services.HasService(typeof(string))", _Services.HasService(typeof(string)));
			SuppConsole.WriteLineParam("_Services.HasService(typeof(int))", _Services.HasService(typeof(int)));



			SuppConsole.PartStart("Exts__dictionary");

			var sourceDict = new Dictionary<string, object>
			{
				{ "Str", "Тестовая строка" },
				{ "Int", 42 },
				{ "IntFromLong", 100L },
				{ "Long", 9223372036854775807L },
				{ "Double", 123.456 },
				{ "Float", 3.14f },
				{ "Decimal", 999.99m },
				{ "BoolTrue", true },
				{ "BoolStr", "true" },
				{ "BoolOne", 1 },
				{ "DateTime", new DateTime(2026, 9, 10, 22, 0, 0) },
				{ "DateTimeStr", "2026-09-10T22:00:00" },
				{ "BadValue", "не число" }
			};
			var readOnlyDict = (IReadOnlyDictionary<string, object>)sourceDict;
			var intStrDict = new Dictionary<int, string> { { 10, "Десять" } };
			var pair = new KeyValuePair<string, string>("User", "Roman");
			Console.WriteLine($"Сериализация пары: {pair.GetSerialization("[{0}]")}");
			Console.WriteLine($"Прямое получение T: {sourceDict.GetFromDict("Int")}");
			Console.WriteLine($"Прямое получение из ReadOnly: {readOnlyDict.Get("Str")}");
			Console.WriteLine($"Строка или ключ: найдено : {sourceDict.GetValueStringOrKey("Str", v => v.ToString()?.ToUpper())}");
			Console.WriteLine($"Строка или ключ: шаблон : {sourceDict.GetValueStringOrKey("Missing", v => v.ToString(), "[{0}]")}");
			Console.WriteLine($"Строка из объекта: \"{sourceDict.GetStringFromDict("Str", "дефолт")}\"");
			Console.WriteLine($"Строка по числу из IDictionary: \"{intStrDict.GetStringFromDict(10, "-")}\"");
			Console.WriteLine($"Число int напрямую: {sourceDict.GetIntFromDict("Int", 0)}");
			Console.WriteLine($"Число int из long-объекта: {sourceDict.GetIntFromDict("IntFromLong", 0)}");
			Console.WriteLine($"Число int: ошибка парсинга: {readOnlyDict.GetInt("BadValue", -1)}");
			Console.WriteLine($"Число long напрямую: {sourceDict.GetLongFromDict("Long", 0L)}");
			Console.WriteLine($"Число double напрямую: {readOnlyDict.GetDouble("Double", 0.0)}");
			Console.WriteLine($"Число float напрямую: {sourceDict.GetFloatFromDict("Float", 0.0f)}");
			Console.WriteLine($"Число decimal напрямую: {readOnlyDict.GetDecimal("Decimal", 0.0m)}");
			Console.WriteLine($"Число decimal из double: {sourceDict.GetDecimalFromDict("Double", 0.0m)}");
			Console.WriteLine($"Булево напрямую: {sourceDict.GetBoolFromDict("BoolTrue", false)}");
			Console.WriteLine($"Булево из строки: {readOnlyDict.GetBool("BoolStr", false)}");
			Console.WriteLine($"Булево из единицы: {sourceDict.GetBoolFromDict("BoolOne", false)}");
			Console.WriteLine($"Дата-время напрямую: {readOnlyDict.GetDateTime("DateTime", DateTime.MinValue):yyyy-MM-dd HH:mm:ss}");
			Console.WriteLine($"Дата-время из строки: {sourceDict.GetDateTimeFromDict("DateTimeStr", DateTime.MinValue):yyyy-MM-dd HH:mm:ss}");
			Console.WriteLine($"Только дата из DateTime: {sourceDict.GetDateOnlyFromDict("DateTime", DateOnly.MinValue)}");
			Console.WriteLine($"Только время из DateTime: {readOnlyDict.GetTimeOnly("DateTime", TimeOnly.MinValue)}");
			Console.WriteLine($"Отсутствующий ключ: {sourceDict.GetIntFromDict("Missing", -999)}");
			Console.WriteLine($"Null вместо словаря: {((IReadOnlyDictionary<string, object>?)null).Get("AnyKey") ?? "null"}");



			SuppConsole.PartStart("Exts__io");

			var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
			SuppConsole.WriteLineParam("SuppConsole.ArrToText(dir.GetFilesByExtensions(\".json\", \".dll\"))",
				SuppConsole.ArrToText(dir.GetFilesByExtensions(".json", ".dll")));
			SuppConsole.WriteLineParam("SuppConsole.ArrToText(dir.GetFilesByExtensions(\".exe;.pdb\"))",
				SuppConsole.ArrToText(dir.GetFilesByExtensions(".exe;.pdb")));
			SuppConsole.WriteLineParam("SuppConsole.ArrToText(dir.GetFilesByExtensions(\".XML\"))",
				SuppConsole.ArrToText(dir.GetFilesByExtensions(".XML")));



			SuppConsole.PartStart("Exts__linq");

			IQueryable<Client> dataset = new List<Client> {
				new() { Name = "Иван", Age = 20 },
				new() { Name = "Сергей", Age = 45 },
				new() { Name = "Антон", Age = 20 }
			}.AsQueryable();
			var sorting = new OrderBuilder("Age,-Name");
			Expression<Func<Client, bool>> condition1 = c => c.Age >= 20;
			Expression<Func<Client, bool>> condition2 = c => c.Name.StartsWith('А');
			SuppConsole.WriteLineParam("List<Client>", dataset);
			SuppConsole.WriteLineParam("ApplyOrderBy(\"Name\")", SuppConsole.ArrToText(dataset.ApplyOrderBy("Name").Select(x => x.Name)));
			SuppConsole.WriteLineParam("ApplyOrder(new OrderBuilder(\"Age,-Name\"))", SuppConsole.ArrToText(dataset.ApplyOrder(sorting).Select(x => $"{x.Age}:{x.Name}")));
			SuppConsole.WriteLineParam("dataset.Where(condition1.And(condition2)).Count()", dataset.Where(condition1.And(condition2)).Count());
			SuppConsole.WriteLineParam("dataset.Where(condition1.Or(condition2)).Count()", dataset.Where(condition1.Or(condition2)).Count());



			SuppConsole.PartStart("Exts__make");

			string? name = "Роман";
			Console.WriteLine(name.Make("Привет, {0}!"));
			Console.WriteLine(name.Make("Пользователь: {0}", "Аноним"));

			string? anon = "Аноним";
			Console.WriteLine(anon.Make("Пользователь: {0}", "Аноним"));

			var date = new DateTime(2026, 9, 2);
			Console.WriteLine(date.Make("Дата события: {0}", "d MMMM yyyy"));

			DateTime emptyDate = DateTime.MinValue;
			Console.WriteLine($"Пустая дата: '{emptyDate.Make("Дата: {0}")}'");

			UserRole role1 = UserRole.Admin;
			UserRole role2 = UserRole.Guest;
			Console.WriteLine(role1.Make("Вы вошли как: {0}"));
			Console.WriteLine(role2.Make("Роль без описания: {0}"));
			Console.WriteLine($"Исключение роли: '{role2.Make("Роль: {0}", UserRole.Guest)}'");

			var list = new List<string?> { "Первый", "", "Второй", null, "Третий" };
			Console.WriteLine(list.MakeFromCollection("Элементы: [{0}]", "*{0}*", ", "));

			string site = "github.com";
			Console.WriteLine(site.Make_UrlWrapper("Перейти по ссылке: {0}"));



			SuppConsole.PartStart("Exts__reflection");

			var item = new SampleItem();
			var myMethodInfo = typeof(SampleItem).GetMethod("MyGenericMethod");
			SuppConsole.WriteLineParam("item.ToDynamic().Title", item.ToDynamic().Title);
			SuppConsole.WriteLineParam("typeof(int?).GetCSharpTypeName()", typeof(int?).GetCSharpTypeName());
			SuppConsole.WriteLineParam("typeof(int?).GetCSharpTypeName(true)", typeof(int?).GetCSharpTypeName(true));
			SuppConsole.WriteLineParam("myMethodInfo?.GetCSharpGenerics()", myMethodInfo?.GetCSharpGenerics());
			SuppConsole.WriteLineParam("SuppConsole.ArrToText(myMethodInfo?.GetCSharpParams())",
				SuppConsole.ArrToText(myMethodInfo?.GetCSharpParams()));
			SuppConsole.WriteLineParam("typeof(SampleItem).GetMethod(\"get_Title\")?.GetPropertyName()",
				typeof(SampleItem).GetMethod("get_Title")?.GetPropertyName());
			SuppConsole.WriteLineParam("item.GetPropertyValue(\"Title\")", item.GetPropertyValue("Title"));
			SuppConsole.WriteLineParam("((object?)null).DefaultObject(999)", ((object?)null).DefaultObject(999));



			SuppConsole.PartStart("Exts__to");

			Console.WriteLine($"(Валидный) \" 123  \".ToInt() : {" 123  ".ToInt()}");
			Console.WriteLine($"(Невалидный) \"123x\".ToInt() : {"123x".ToInt()}");
			Console.WriteLine($"(Дефолт при ошибке) \"123x\".ToInt(999) : {"123x".ToInt(999)}");
			Console.WriteLine($"(Дефолт при null) ((string?)null).ToInt(555) : {((string?)null).ToInt(555)}");
			Console.WriteLine($"(Валидный double) \"123.45\".ToDouble() : {"123.45".ToDouble()}");
			Console.WriteLine($"(Дефолт при пустой строке) \"   \".ToDecimal(10.5m) : {"   ".ToDecimal(10.5m)}");
			Console.WriteLine($"(Валидная дата) \"2026-12-31\".ToDateTime() : {"2026-12-31".ToDateTime()}");
			Console.WriteLine($"(Валидный DateOnly) \"2026-12-31\".ToDateOnly() : {"2026-12-31".ToDateOnly()}");
			Console.WriteLine($"(Валидный TimeOnly) \"23:59:59\".ToTimeOnly() : {"23:59:59".ToTimeOnly()}");
			Console.WriteLine($"(Дефолт при ошибке) \"error\".ToDateOnly(new DateOnly(2020, 1, 1)) : {"error".ToDateOnly(new DateOnly(2020, 1, 1))}");
			Console.WriteLine($"(Логическая единица) \"1\".ToBool() : {"1".ToBool()}");
			Console.WriteLine($"(Логическая строка) \" True \".ToBool() : {" True ".ToBool()}");
			Console.WriteLine($"(Логический ноль) \"0\".ToBool() : {"0".ToBool()}");
			Console.WriteLine($"(Невалидный текст) \"abc\".ToBool() : {"abc".ToBool()}");
			Console.WriteLine($"(При значении null) ((string?)null).ToBool() : {((string?)null).ToBool()}");

			var a10_1 = new int[] { 1, 2, 3, 4, 5 };
			var a10_2 = new string[] { "10", "abc", "30" };
			var s10_1 = "10;abc|20,,30";
			var s10_2 = " 10 ;  | 20 ,, 30 ";
			Console.WriteLine($"Разбиение на блоки по 2 {SuppConsole.ArrToText(a10_1)}.ToGrid(2) : {SuppConsole.ArrToText(a10_1.ToGrid(2).Select(x => "[" + SuppConsole.ArrToText(x) + "]"))}");
			Console.WriteLine($"Массив строк в числа {SuppConsole.ArrToText(a10_2)}.ToIntArray() : {SuppConsole.ArrToText(a10_2.ToIntArray())}");
			Console.WriteLine($"Парсинг строки с ошибками \"{s10_1}\".ToIntArray() : {SuppConsole.ArrToText(s10_1.ToIntArray())}");
			Console.WriteLine($"Парсинг пустой строки (string?)null.ToIntArray() : {SuppConsole.ArrToText(((string?)null).ToIntArray())}");
			Console.WriteLine($"Парсинг с пробелами и пустыми \"{s10_2}\".ToIntArrayTrim() : {SuppConsole.ArrToText(s10_2.ToIntArrayTrim())}");



			SuppConsole.PartStart("Exts_Assembly");

			Console.WriteLine($"Имя текущей сборки: {Assembly.GetExecutingAssembly().Name}");
			Console.WriteLine($"Имя неопределенной сборки: {((Assembly?)null).Name}");
			Console.WriteLine($"Версия текущей сборки: {Assembly.GetExecutingAssembly().Version}");
			Console.WriteLine($"Версия при null: {((Assembly?)null).Version}");
			Console.WriteLine($"Полная версия текущей сборки: {Assembly.GetExecutingAssembly().FullVersion}");
			Console.WriteLine($"Полная версия при null: {((Assembly?)null).FullVersion}");
			Console.WriteLine($"Описание текущей сборки: {Assembly.GetExecutingAssembly().Description ?? "null"}");
			Console.WriteLine($"Описание при null: {((Assembly?)null).Description ?? "null"}");



			SuppConsole.PartStart("Exts_Claims");

			var identity = new ClaimsIdentity("TestAuth");
			identity.AddClaim(ClaimTypes.NameIdentifier, "user_9952");
			identity.AddClaim("id_username", "roman_koff");
			identity.AddClaim(ClaimTypes.Email, "koff@example.com");
			var principal = new ClaimsPrincipal(identity);
			principal.AddRolesClaims(["Admin", "Developer", "Moderator"]);
			principal.AddClaims("custom_badge", "Gold", "BetaTester");
			SuppConsole.WriteLineParam("principal.GetNameIdentifierFromClaim()", principal.GetNameIdentifierFromClaim());
			SuppConsole.WriteLineParam("principal.GetIdUsernameFromClaim()", principal.GetIdUsernameFromClaim());
			SuppConsole.WriteLineParam("principal.GetEmailFromClaim()", principal.GetEmailFromClaim());
			SuppConsole.WriteLineParam("SuppConsole.ArrToText(principal.GetRolesFromClaim())", SuppConsole.ArrToText(principal.GetRolesFromClaim()));
			SuppConsole.WriteLineParam("principal.GetNameFromClaim()", principal.GetNameFromClaim() ?? "–нет–");



			SuppConsole.PartStart("Exts_Exception");

			var rootCause = new InvalidOperationException("Индекс базы данных 'IDX_USERS' поврежден или недоступен");
			var dbException = new AggregateException("Ошибка выполнения SQL-запроса на сервере", rootCause);
			var appException = new Exception("Критическая ошибка приложения при авторизации", dbException);
			SuppConsole.WriteLineParam(
				"Сообщение корневой ошибки (GetExceptionMessage)",
				appException.GetExceptionMessage());
			SuppConsole.WriteLineParam(
				"Проверка Contains на верхнем уровне ('авторизации')",
				appException.TestContains("авторизации"));
			SuppConsole.WriteLineParam(
				"Проверка Contains во вложенном уровне ('IDX_USERS')",
				appException.TestContains("idx_users"));
			SuppConsole.WriteLineParam(
				"Проверка StartsWith на верхнем уровне ('Критическая')",
				appException.TestStartsWith("Критическая"));
			SuppConsole.WriteLineParam(
				"Проверка StartsWith во вложенном уровне ('Ошибка выполнения')",
				appException.TestStartsWith("Ошибка выполнения"));
			SuppConsole.WriteLineParam(
				"Поиск отсутствующей строки ('Успешно')",
				appException.TestContains("Успешно"));



			SuppConsole.PartStart("Exts_HttpClient");

			using var client = new HttpClient();
			var nativeCache = new MemoryCache(new MemoryCacheOptions());
			var resNormal = await client.GetJsonResultAsync<RaspModel>(
				_ApiUrl, nativeCache);
			SuppConsole.WriteLineParam(
				"Обычный запрос (сохранится в кэш)", resNormal.Content?.DateRelease);
			var resDirect = await client.GetJsonResultAsync<RaspModel>(
				_ApiUrl, nativeCache, cacheOptions: SuppCache.ZERO_CACHE_OPTIONS);
			SuppConsole.WriteLineParam(
				"Запрос в обход сохранения в кэш", resDirect.Content?.DateRelease);



			SuppConsole.PartStart("Exts_IPAddress");

			var subnetV4 = new IPSubnet("192.168.1.0/24");
			var subnetV6 = new IPSubnet("2001:db8::/32");
			var ipOkV4 = IPAddress.Parse("192.168.1.45");
			var ipBadV4 = IPAddress.Parse("192.168.2.1");
			var ipOkV6 = IPAddress.Parse("2001:db8:aaaa:bbbb::1");
			var ipBadV6 = IPAddress.Parse("2001:def::1");
			SuppConsole.WriteLineParam("192.168.1.45 входит в 192.168.1.0/24", ipOkV4.IsInSubnet(subnetV4));
			SuppConsole.WriteLineParam("192.168.2.1 входит в 192.168.1.0/24", ipBadV4.IsInSubnet(subnetV4));
			SuppConsole.WriteLineParam("2001:db8:aaaa::1 входит в 2001:db8::/32", ipOkV6.IsInSubnet(subnetV6));
			SuppConsole.WriteLineParam("2001:def::1 входит в 2001:db8::/32", ipBadV6.IsInSubnet(subnetV6));
			var whiteList = new IPSubnetsList("10.0.0.0/8; 172.16.0.0/12, 192.168.1.0/24");
			var checkIp = IPAddress.Parse("172.20.5.10");
			bool isAllowed = whiteList.Any(checkIp.IsInSubnet);
			SuppConsole.WriteLineParam("Строка конфигурации списков подсетей", whiteList);
			SuppConsole.WriteLineParam("IP 172.20.5.10 находится в белом списке подсетей", isAllowed);



			SuppConsole.PartStart("Exts_String");

			Console.WriteLine($"Разделение строки с нехваткой элементов : {SuppConsole.ArrToText("раз|два".SplitFix("|", 4))}");
			Console.WriteLine($"Разделение строки с избытком элементов : {SuppConsole.ArrToText("1;2;3;4;5".SplitFix(";", 2))}");
			Console.WriteLine($"Извлечение тега из середины строки : {"контекст [target] текст".GetTag("[", "]")}");
			Console.WriteLine($"Извлечение тега с пустой границей после : {"ключ=значение".GetTag("ключ=", "")}");
			Console.WriteLine($"Рекурсивная очистка двойных пробелов : {"много   пробелов   тут".GetReplaceRecursively("  ", " ")}");

			var dict = new Dictionary<string, string> { { "A", "1" }, { "B", "2" } };
			var hashDict = new Dictionary<string, string> { { "news", "события" }, { "csharp", "программирование" } };
			Console.WriteLine($"Замена по словарю : {"A и B сидели на трубе".ReplaceFromDict(dict)}");
			Console.WriteLine($"Замена если равны : {"тест".ReplaceIfEqual("тест", "новый")}");
			Console.WriteLine($"Маскирование символов по строке : {"abc-123-def".ReplaceByChars("*", "123")}");
			Console.WriteLine($"Маскирование символов по массиву : {"hello world".ReplaceByChars("?", 'h', 'e', 'l')}");
			Console.WriteLine($"Замена хэштегов из словаря : {"Привет всем, читайте #news про #csharp!".ReplaceHashtagsFromDict(hashDict)}");
			Console.WriteLine($"Удаление повторов в начале : {"///path/to/dir".ReplaceStart("/", "")}");
			Console.WriteLine($"Тримминг конца строки с заменой : {"index.html.bak.bak".GetTrimEnd(".bak", ".old")}");

			var case1 = "text STRING Value. тест ЗНАЧЕНИЯ Строки.";
			Console.WriteLine($"Первая заглавная (без изменения остальных) : {case1.GetAsFirstUpper()}");
			Console.WriteLine($"Первая заглавная (остальные строчные) : {case1.GetAsFirstUpper(forcedToLower: true)}");
			Console.WriteLine($"Первая строчная (без изменения остальных) : {case1.GetAsFirstLower()}");
			Console.WriteLine($"Первая строчная (остальные заглавные) : {case1.GetAsFirstLower(forcedToUpper: true)}");
			Console.WriteLine($"Каждое слово с заглавной буквы : {case1.GetAsTitleCase()}");

			var path = "C:\\Projects\\App\\src\\program.cs";
			var text = "Мама мыла раму красивым розовым мылом";
			Console.WriteLine($"Левая часть до 1-го слеша с конца (skip=0) : {path.GetLeftTo('\\', 0)}");
			Console.WriteLine($"Левая часть до 2-го слеша с конца (skip=1) : {path.GetLeftTo('\\', 1)}");
			Console.WriteLine($"Первые 11 символов строки : {path.GetLeft(11)}");
			Console.WriteLine($"Правая часть после расширения (skip=0) : {path.GetRightFrom('.', 0)}");
			Console.WriteLine($"Правая часть от первого слеша слева : {path.GetRightSide('\\')}");
			Console.WriteLine($"Отрезать 3 символа справа : {path.GetLeftSide(3)}");
			Console.WriteLine($"Безопасный умный обрез по словам : {text.GetCropToWords(20, "...")}");
			Console.WriteLine($"Обрезка строки с масками по краям : {text.GetCrop(4, 8, ">>>", "<<<")}");

			Console.WriteLine($"Экранирование непечатных символов : {"Текст\nС\tСимволами".GetSafeText()}");
			Console.WriteLine($"Схлопывание пробелов и Unicode-пробелов : {"   Много    пробелов   тут   ".CollapseSpaces()}");
			Console.WriteLine($"Проверка на наличие системных хак-символов : {"Валидный Текст №1".HasForbiddenSymbols()}");
			Console.WriteLine($"Очистка строки от плохих символов : {"Текст\u0001 с ошибками\u0012".ClearForbiddenSymbols()}");
			Console.WriteLine($"Проверка строки на эмодзи : {"Привет! 👋 Код работает 🚀".HasEmoji()}");
			Console.WriteLine($"Полная очистка текста от эмодзи : {"Привет! 👋 Код работает 🚀".ClearEmoji()}");



			SuppConsole.PartStart("Exts_StringBuilder");

			var sb1 = new StringBuilder("Базовый текст");
			sb1.AppendIfPresent(" -> Добавлено: {0} и {1}", "ЭлементА", "ЭлементБ");
			sb1.InsertIfPresent(0, "[СТАРТ] ");
			sb1.InsertIf(true, sb1.Length, " [КОНЕЦ]");
			SuppConsole.WriteLineParam("Результат работы Append/Insert методов", sb1.ToString());
			var sb2 = new StringBuilder("У Романа естьяблоко, но у романа нет груши. РОМАН доволен.");
			int idx = sb2.IndexOf("романа", 0, ignoreCase: true);
			SuppConsole.WriteLineParam("Индекс первого вхождения 'романа' (регистронезависимо)", idx);
			sb2.ReplaceRecursively("роман", "Алексей", ignoreCase: true);
			SuppConsole.WriteLineParam("Результат ReplaceRecursively ('роман' -> 'Алексей')", sb2.ToString());
			var sb3 = new StringBuilder("   \t Много пробелов по краям \r\n   ");
			sb3.Trim();
			SuppConsole.WriteLineParam("Результат работы Trim()", $"'{sb3}'");
			var sb4 = new StringBuilder("###!!!Текст внутри спецсимволов!!!##");
			sb4.Trim(['#', '!']);
			SuppConsole.WriteLineParam("Результат работы Trim(chars)", $"'{sb4}'");

		}


		static async Task _outSuppsAsync()
		{
			SuppConsole.SectionStart("SUPPS");



			SuppConsole.PartStart("SuppApp");

			Console.WriteLine($"Входная сборка: {SuppApp.EntryAssembly?.GetName().Name ?? "null"}");
			Console.WriteLine($"Вызывающая сборка: {SuppApp.CallingAssembly?.GetName().Name ?? "null"}");
			Console.WriteLine($"Рабочий каталог: {SuppApp.CurrentDirectory}");
			Console.WriteLine($"Базовый каталог: {SuppApp.BaseDirectory}");
			Console.WriteLine($"Путь к проекту VS: {SuppApp.VSProjectPath}");
			Console.WriteLine($"Имя проекта VS: {SuppApp.VSProjectName}");
			Console.WriteLine($"Путь к решению VS: {SuppApp.VSSolutionPath}");
			Console.WriteLine($"Имя решения VS: {SuppApp.VSSolutionName}");



			SuppConsole.PartStart("SuppCache");

			var optDefault = SuppCache.GetOptions(0, 0);
			var optSliding = SuppCache.GetOptions(15, 0);
			var optBoth = SuppCache.GetOptions(10, 60);
			Console.WriteLine($"Дефолтные опции при нулевых параметрах (абсолютное время) : {optDefault.AbsoluteExpirationRelativeToNow?.TotalSeconds} сек");
			Console.WriteLine($"Только скользящее время жизни записи кэша : {optSliding.SlidingExpiration?.TotalSeconds} сек");
			Console.WriteLine($"Комбинированные опции (скользящее время) : {optBoth.SlidingExpiration?.TotalSeconds} сек");
			Console.WriteLine($"Комбинированные опции (абсолютное ограничение сверху) : {optBoth.AbsoluteExpirationRelativeToNow?.TotalSeconds} сек");



			SuppConsole.PartStart("SuppConsole");



			SuppConsole.PartStart("SuppCrypto");

			string pass1 = "MySecretPass123!";
			string pass2 = "WrongPass!";
			string hash1 = SuppCrypto.HashPassword(pass1);
			var bytesFromNull1 = SuppCrypto.ToSecureBytes(null);
			var stringFromNull1 = SuppCrypto.ToSecureString(null);
			Console.WriteLine($"Генерация пароля (16): {SuppCrypto.GenerateSecurePassword(16)}");
			Console.WriteLine($"Генерация токена (32): {SuppCrypto.GenerateApiToken(32)}");
			Console.WriteLine($"Хэширование пароля \"{pass1}\": {hash1}");
			Console.WriteLine($"Проверка верного пароля \"{pass1}\": {SuppCrypto.VerifyPassword(pass1, hash1)}");
			Console.WriteLine($"Проверка неверного пароля \"{pass2}\": {SuppCrypto.VerifyPassword(pass2, hash1)}");
			Console.WriteLine($"Хэш SHA256 строки \"Hello\": {SuppCrypto.ComputeSha256("Hello")}");
			Console.WriteLine($"Хэш SHA256 от null-строки: \"{SuppCrypto.ComputeSha256((string?)null)}\"");
			Console.WriteLine($"HMAC SHA256 строки \"Hello\": {SuppCrypto.ComputeHmacSha256("Hello", [1, 2, 3])}");
			Console.WriteLine($"HMAC SHA256 от null-данных: {SuppConsole.ArrToText(SuppCrypto.ComputeHmacSha256((string?)null, [1, 2, 3]))}");
			Console.WriteLine($"Строка null в байты: {SuppConsole.ArrToText(bytesFromNull1)}");
			Console.WriteLine($"Массив байт null в строку: \"{stringFromNull1}\"");



			SuppConsole.PartStart("SuppCulture");

			var oldCulture = CultureInfo.CurrentCulture.Name;
			SuppCulture.SetCulture("en-US");
			Console.WriteLine($"Установка культуры по строке 'en-US' (текущая дата) : {DateTime.Now:G}");
			SuppCulture.SetCulture(new CultureInfo("fr-FR"));
			Console.WriteLine($"Установка культуры по объекту 'fr-FR' (текущая дата) : {DateTime.Now:G}");
			SuppCulture.SetCulture(oldCulture);
			Console.WriteLine($"Возврат к исходной культуре ({oldCulture}) : {DateTime.Now:G}");



			SuppConsole.PartStart("SuppDateTime");

			Console.WriteLine($"Текущее время библиотеки (Current) : {SuppDateTime.Current}");
			Console.WriteLine($"Начало текущего календарного года : {SuppDateTime.CurrentYearBegin}");
			Console.WriteLine($"Дата вчерашнего дня (Yesterday) : {SuppDateTime.Yesterday}");
			Console.WriteLine($"Дата послезавтрашнего дня (TomorrowAfter) : {SuppDateTime.TomorrowAfter}");

			var dt1 = DateTime.Now;
			var dt2 = dt1.AddDays(-10);
			var dt3 = dt1.AddDays(10);
			var dt4 = dt1.AddDays(-30);
			var dt5 = dt1.AddMonths(-6);
			var dt6 = dt1.AddMonths(-12);
			var dt61 = dt6.AddDays(-10);
			var dt62 = dt6.AddDays(-30);
			_writeDateSpan(dt1, dt1);
			_writeDateSpan(dt1, dt2);
			_writeDateSpan(dt1, dt3);
			_writeDateSpan(dt1, dt4);
			_writeDateSpan(dt1, dt5);
			_writeDateSpan(dt1, dt6);
			_writeDateSpan(dt6, dt61);
			_writeDateSpan(dt6, dt62);
			Console.WriteLine($"Парсинг диапазона из строки с разделителем : {SuppDateTime.GetSpan("2026-09-01|13.2.1977", false, new CultureInfo("ru-RU"))}");

			var startDt = new DateOnly(2026, 9, 1);
			var endDt = new DateOnly(2026, 9, 4);
			Console.WriteLine($"Ленивая коллекция дней по порядку : {SuppConsole.ArrToText(SuppDateTime.GetDays(startDt, endDt, false))}");
			Console.WriteLine($"Разворот дат при useMod=true : {SuppConsole.ArrToText(SuppDateTime.GetDays(endDt, startDt, true))}");
			Console.WriteLine($"Пустая коллекция при неверном порядке и useMod=false : {SuppConsole.ArrToText(SuppDateTime.GetDays(endDt, startDt, false))}");

			var d1 = new DateTime(2026, 9, 14);
			var d2 = new DateTime(2026, 9, 20);
			Console.WriteLine($"Максимальная дата из двух (сравнивая с null) : {SuppDateTime.Max(d1, null)}");
			Console.WriteLine($"Максимальная дата из двух (сравнивая с большей) : {SuppDateTime.Max(d1, d2)}");
			Console.WriteLine($"Минимальная дата из двух (сравнивая с большей) : {SuppDateTime.Min(d1, d2)}");

			Console.WriteLine($"Парсинг Unix таймстампа (1789382400) : {SuppDateTime.GetDateTimeFromUnixTimeStamp(1789382400)}");
			Console.WriteLine($"Парсинг Java таймстампа с null значением : {SuppDateTime.GetDateTimeFromJavaTimeStamp((double?)null)}");
			Console.WriteLine($"Парсинг строки формата UniDate (yyyy-MM-dd) : {SuppDateTime.GetDateFromUniDate("2026-09-14")}");
			Console.WriteLine($"Парсинг некорректной строки UniDate : {SuppDateTime.GetDateFromUniDate("invalid-date")}");



			SuppConsole.PartStart("SuppDynamic");

			dynamic expando = new ExpandoObject();
			var anonymous = new { Id = 10, Name = "Roman" };
			expando.Title = "Тестовый заголовок";
			Console.WriteLine($"Проверка существующего свойства в ExpandoObject : {SuppDynamic.HasProperty(expando, "Title")}");
			Console.WriteLine($"Проверка отсутствующего свойства в ExpandoObject : {SuppDynamic.HasProperty(expando, "Age")}");
			Console.WriteLine($"Проверка существующего свойства в анонимном объекте : {SuppDynamic.HasProperty(anonymous, "Name")}");
			Console.WriteLine($"Проверка отсутствующего свойства в анонимном объекте : {SuppDynamic.HasProperty(anonymous, "Price")}");



			SuppConsole.PartStart("SuppGrid");

			string rawGridString =
				"// Список сотрудников отдела разработки\n" +
				"== ----------------------------------------\n" +
				"1 |  Иван Иванов  | 4.85 | 2023-05-12 | true\n" +
				"-- На очереди запись с пустыми полями (проверка дефолтов)\n" +
				"2 | Петр Петров   |      |            | false\n" +
				"3 | Анна Сидорова | 5.0  | 2025-01-20 | true\n" +
				"== Конец файла ==";
			List<EmployeeMetric> fromString = [.. SuppGrid.GetItemsFromString(
				rawGridString,
				parser => new EmployeeMetric(
					Id: parser.GetInt(0),
					Name: parser.GetText(1),
					Rating: parser.GetReal(2),
					HiredDate: parser.GetDate(3),
					IsActive: parser.GetBool(4)
				))];
			SuppConsole.WriteLineParam(
				"1. Количество элементов, прочитанных из string",
				fromString.Count);
			var displayStringList = fromString
				.Select(e => $"[{e.Id}] {e.Name} (Рейтинг: {e.Rating}, Менеджер: {e.IsActive}, Принят: {e.HiredDate?.ToShortDateString() ?? "нет"})")
				.ToArray();
			SuppConsole.WriteLineParam(
				"Содержимое коллекции из строки",
				SuppConsole.ArrToText(displayStringList));

			byte[] gridBytes = Encoding.UTF8.GetBytes("4 | Сервисный Аккаунт | 4.0 | | true");
			using var memoryStream = new MemoryStream(gridBytes);
			List<EmployeeMetric> fromStream = [.. SuppGrid.GetItemsFromStream(
				memoryStream,
				parser => new EmployeeMetric(
					Id: parser.GetInt(0),
					Name: parser.GetText(1),
					Rating: parser.GetReal(2),
					HiredDate: parser.GetDate(3),
					IsActive: parser.GetBool(4)
				))];
			SuppConsole.WriteLineParam(
				"2. Количество элементов, прочитанных из Stream",
				fromStream.Count);
			if (fromStream.Count > 0)
			{
				SuppConsole.WriteLineParam(
					"Данные из потока",
					fromStream[0].ToString());
			}

			string russianGrid =
				"// Тест локализованной культуры ru-RU\n" +
				"5 | Екатерина | 4,91 | 18.09.2024 | true";
			using var russianReader = new StringReader(russianGrid);
			var russianCulture = new CultureInfo("ru-RU");
			List<EmployeeMetric> fromCulture = [.. SuppGrid.GetItems(
				russianReader,
				parser => new EmployeeMetric(
					Id: parser.GetInt(0),
					Name: parser.GetText(1),
					Rating: parser.GetReal(2),
					HiredDate: parser.GetDate(3),
					IsActive: parser.GetBool(4)
				),
				provider: russianCulture)];
			SuppConsole.WriteLineParam(
				"3. Парсинг с культурой ru-RU (запятая в дроби)",
				fromCulture.Count > 0 ? $"Успешно. Рейтинг: {fromCulture[0].Rating}" : "Ошибка");

			string syncFilePath = Path.Combine(Path.GetTempPath(), "sync_data.grid");
			string syncContent =
				"// Тест синхронного чтения файла\n" +
				"6 | Дмитрий | 3.5 | 2026-02-10 | false";
			File.WriteAllText(syncFilePath, syncContent, new UTF8Encoding(false));
			List<EmployeeMetric> fromSyncFile = [.. SuppGrid.GetItemsFromFile(
				syncFilePath,
				parser => new EmployeeMetric(
					Id: parser.GetInt(0),
					Name: parser.GetText(1),
					Rating: parser.GetReal(2),
					HiredDate: parser.GetDate(3),
					IsActive: parser.GetBool(4)
				))];
			SuppConsole.WriteLineParam(
				"4. Синхронно прочитано записей из файла",
				fromSyncFile.Count);

			string asyncFilePath = Path.Combine(Path.GetTempPath(), "async_data.grid");
			string asyncContent =
				"// Тест асинхронного потокового чтения\n" +
				"7 | Елена Глушкова | 4.99 | 2026-09-22 | true\n" +
				"8 | Антон Смирнов  | 4.20 | 2021-11-05 | false";
			await File.WriteAllTextAsync(asyncFilePath, asyncContent, new UTF8Encoding(false));
			var asyncEmployees = new List<EmployeeMetric>();
			await foreach (EmployeeMetric emp1 in SuppGrid.GetItemsFromFileAsync(
				asyncFilePath,
				parser => new EmployeeMetric(
					Id: parser.GetInt(0),
					Name: parser.GetText(1),
					Rating: parser.GetReal(2),
					HiredDate: parser.GetDate(3),
					IsActive: parser.GetBool(4)
				)))
			{
				asyncEmployees.Add(emp1);
			}
			SuppConsole.WriteLineParam(
				"5. Асинхронно прочитано записей из файла",
				asyncEmployees.Count);

			var displayAsyncList = asyncEmployees
				.Select(e => $"ID: {e.Id}, Имя: {e.Name}, Дата найма: {e.HiredDate?.ToString("yyyy-MM-dd")}")
				.ToArray();
			SuppConsole.WriteLineParam(
				"Содержимое асинхронной коллекции",
				SuppConsole.ArrToText(displayAsyncList));



			SuppConsole.PartStart("SuppIO");

			string testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFolder");
			string testFile = Path.Combine(testDir, "document.txt");
			SuppIO.CreateDirectoryIfNotExists(testDir);
			Console.WriteLine($"Директория создана или существует (проверка) : {Directory.Exists(testDir)}");
			File.WriteAllText(testFile, "Hello World");
			FileInfo fileInfo = new(testFile);
			Console.WriteLine($"Путь для нового файла (свободный) : {SuppIO.GetNewName(fileInfo, "unique.txt")}");
			Console.WriteLine($"Путь для существующего файла (занятый) : {SuppIO.GetNewName(fileInfo, "document.txt")}");
			SuppIO.Rename(fileInfo, "document.txt"); // Так как имя занято им же, имя изменится на document_.txt
			string renamedFile = Path.Combine(testDir, "document_.txt");
			Console.WriteLine($"Файл успешно переименован с уникальным суффиксом : {File.Exists(renamedFile)}");
			SuppIO.DeleteFileIfExists(renamedFile);
			Console.WriteLine($"Проверка удаления файла : {!File.Exists(renamedFile)}");
			SuppIO.DeleteDirectoryIfExists(testDir);
			Console.WriteLine($"Проверка удаления директории : {!Directory.Exists(testDir)}");

			string textFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			string binaryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bytes.dat");
			SuppIO.FileWrite(textFilePath, "Тестовая строка для записи в файл.");
			Console.WriteLine($"Запись строки в файл (Default: Create, UTF8) : {File.Exists(textFilePath)}");
			SuppIO.FileWrite(textFilePath, "\nДополнительная строка.", EncodingsEnum.UTF8, FileMode.Append);
			Console.WriteLine($"Дозапись текста в режиме Append : {File.ReadAllText(textFilePath).Contains("Дополнительная")}");
			byte[] sampleBytes = [0x41, 0x42, 0x43, 0x44]; // ABCD
			SuppIO.FileWrite(binaryFilePath, sampleBytes);
			Console.WriteLine($"Запись массива байт в файл : {File.Exists(binaryFilePath)}");
			Console.WriteLine($"Кодировка по умолчанию : {SuppIO.GetEncoding(EncodingsEnum.UTF8).WebName}");
			Console.WriteLine($"Кириллица Windows : {SuppIO.GetEncoding(EncodingsEnum.WINDOWS1251).CodePage}");
			Console.WriteLine($"DOS кодировка : {SuppIO.GetEncoding(EncodingsEnum.CP866).EncodingName}");

			string samplePath = "C:/Docs/Readme.TXT";
			string noExtPath = "C:/Docs/NoExtensionFile";
			Console.WriteLine($"Расширение с точкой : {SuppIO.GetFileExtension(samplePath, true)}");
			Console.WriteLine($"Расширение файла без расширения : '{SuppIO.GetFileExtension(noExtPath, true)}'");
			Console.WriteLine($"Части имени файла разделенные '.' : {SuppConsole.ArrToText(SuppIO.GetFilenameHalfs("archive.tar.gz"))}");
			Console.WriteLine($"Части имени для null-строки : {SuppConsole.ArrToText(SuppIO.GetFilenameHalfs(null!))}");
			Console.WriteLine($"Поиск ContentInfo по расширению (MIME) : {SuppIO.GetContentInfoFromExtension("png").ContentType}");
			Console.WriteLine($"Поиск ContentInfo по пути файла (Категория) : {SuppIO.GetContentInfoFromPath(samplePath).Group}");

			string demoFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "demo.txt");
			byte[] cryptoSalt = [0x01, 0x02, 0x03, 0x04, 0x05];
			File.WriteAllText(demoFile, "Microsoft .NET 10 Common Library Testing Content.");
			Console.WriteLine($"Чтение текстового файла (UTF8) : {SuppIO.FileRead(demoFile, EncodingsEnum.UTF8)}");
			Console.WriteLine($"Первые 10 байт файла в Base64 : {SuppIO.GetFileBegin(demoFile, 10)}");
			using (var stream = new FileStream(demoFile, FileMode.Open, FileAccess.Read))
				Console.WriteLine($"Чистый SHA1 хэш из потока : {SuppIO.GetFileSHA1(stream, [])}");
			Console.WriteLine($"HMAC-SHA1 хэш файла со строковой солью : {SuppIO.GetFileSHA1(demoFile, "SecretSalt123")}");
			Console.WriteLine($"HMAC-SHA1 хэш файла с байтовой солью : {SuppIO.GetFileSHA1(demoFile, cryptoSalt)}");
			using (var stream = new FileStream(demoFile, FileMode.Open, FileAccess.Read))
				Console.WriteLine($"Чистый SHA256 хэш из потока : {SuppIO.GetFileSHA256(stream, [])}");
			Console.WriteLine($"HMAC-SHA256 хэш файла со строковой солью : {SuppIO.GetFileSHA256(demoFile, "SecureSalt2026")}");
			Console.WriteLine($"HMAC-SHA256 хэш файла с байтовой солью : {SuppIO.GetFileSHA256(demoFile, cryptoSalt)}");

			string validFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "demo.txt");
			var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			Console.WriteLine($"Время изменения DateTimeOffset : {SuppIO.GetFileLastModified(validFile)}");
			Console.WriteLine($"Последнее изменение файлов каталога : {SuppIO.GetLastWriteTimeFiles(dirInfo)}");
			Console.WriteLine($"Размер 2049 байт в КБ (округление вверх) : {SuppIO.GetLengthOfKB(2049)}");
			Console.WriteLine($"Размер 500 байт в КБ (минимум 1 КБ) : {SuppIO.GetLengthOfKB(500)}");
			Console.WriteLine($"Исправление системного имени CON : {SuppIO.FixForbiddenFileName("CON")}");
			Console.WriteLine($"Безопасное расширение для .jpeg : {SuppIO.GetSafeFileExtension("image.JPEG")}");
			Console.WriteLine($"Полное безопасное имя файла : {SuppIO.GetSafeFilename("My:Un*safe/File.jpeg")}");
			Console.WriteLine($"Проверка пути на недопустимые символы : {SuppIO.HasInvalidPathChars("C:/Folder|Subfolder")}");
			Console.WriteLine($"Проверка имени на недопустимые символы : {SuppIO.HasInvalidFileNameChars("file*name.txt")}");



			SuppConsole.PartStart("SuppJson");

			var user = new UserDto
			{
				Id = 45,
				Login = "admin_koff",
				Roles = ["Administrator", "Developer"]
			};
			string jsonFile = "user.json";
			string genJsonFile = "user_gen.json";
			string jsonStr = SuppJson.GetJsonStringFromObject(user);
			SuppConsole.WriteLineParam("JSON-строка из объекта", jsonStr);
			var userFromStr = SuppJson.GetObjectFromJsonString<UserDto>(jsonStr);
			SuppConsole.WriteLineParam("Логин из JSON-строки", userFromStr?.Login ?? "null");
			SuppConsole.WriteLineParam("Роли из JSON-строки", SuppConsole.ArrToText(userFromStr?.Roles));
			string genStr = SuppJson.GetJsonStringFromObjectGen(user, MyProjectJsonContext.Default.UserDto);
			SuppConsole.WriteLineParam("JSON-строка (Source Gen)", genStr);
			var userFromGenStr = SuppJson.GetObjectFromJsonStringGen(genStr, MyProjectJsonContext.Default.UserDto);
			SuppConsole.WriteLineParam("ID из Source Gen строки", userFromGenStr?.Id ?? 0);
			await SuppJson.SaveObjectToJsonFileAsync(user, jsonFile);
			var userFromFile = await SuppJson.GetObjectFromJsonFileAsync<UserDto>(jsonFile);
			SuppConsole.WriteLineParam("Загружено из файла (классика)", userFromFile?.Login ?? "нет");
			await SuppJson.SaveObjectToJsonFileGenAsync(user, genJsonFile, MyProjectJsonContext.Default.UserDto);
			var userFromGenFile = await SuppJson.GetObjectFromJsonFileGenAsync(genJsonFile, MyProjectJsonContext.Default.UserDto);
			SuppConsole.WriteLineParam("Загружено из файла (Source Gen)", userFromGenFile?.Login ?? "нет");



			SuppConsole.PartStart("SuppLangEn");

			for (var i1 = -10; i1 <= 10; i1++)
				Console.WriteLine($"Плюрализация по шаблону : {SuppLangEn.GetPlural("In bag: {0} {1}", i1, "orange", "oranges")}");
			Console.WriteLine($"Множественное число слова (обычное) : {SuppLangEn.ToPlural("book")}");
			Console.WriteLine($"Множественное число (исключение) : {SuppLangEn.ToPlural("Child")}");
			Console.WriteLine($"Множественное число (UPPERCASE) : {SuppLangEn.ToPlural("MOUSE")}");
			Console.WriteLine($"Множественное число (на -y) : {SuppLangEn.ToPlural("city")}");
			Console.WriteLine($"Множественное число (латынь) : {SuppLangEn.ToPlural("bacterium")}");
			Console.WriteLine($"Множественное число через дефис : {SuppLangEn.ToPlural("mother-in-law")}");



			SuppConsole.PartStart("SuppLangRu");

			SuppConsole.WriteLineParam(
				"Пол для 'Пушкин Александр Сергеевич'",
				SuppLangRu.GetGender("Пушкин Александр Сергеевич"));
			SuppConsole.WriteLineParam(
				"Пол для 'Ахматова Анна Андреевна'",
				SuppLangRu.GetGender("Ахматова Анна Андреевна"));
			var maleTurkic = SuppLangRu.GetGender("Эфендиев Эльчин Ильяс оглы");
			var femaleTurkic = SuppLangRu.GetGender("Мамедова Фарида Саид кызы");
			SuppConsole.WriteLineParam("Пол для 'Эфендиев Эльчин Ильяс оглы'", maleTurkic);
			SuppConsole.WriteLineParam("Пол для 'Мамедова Фарида Саид кызы'", femaleTurkic);
			var upperCaseTest = SuppLangRu.GetGender("  ПЕТРОВИЧ  ");
			SuppConsole.WriteLineParam("Пол для небрежно введенного '  ПЕТРОВИЧ  '", upperCaseTest);
			var unknownTest = SuppLangRu.GetGender("Ковальчук");
			SuppConsole.WriteLineParam("Пол для неоднозначной фамилии 'Ковальчук'", unknownTest);

			var (fam0, init0) = SuppLangRu.GetFamilyAndInitials(
				"пушкин александр сергеевич", LetterCasesEnum.TitleCase);
			SuppConsole.WriteLineParam(
				"Базовый пример (кортеж)", $"{fam0} | {init0}");

			SuppConsole.WriteLineParam(
				"SuppLangRu.GetFamilyAndInitialsString(\"пушкин александр сергеевич\")",
				SuppLangRu.GetFamilyAndInitialsString("пушкин александр сергеевич"));
			SuppConsole.WriteLineParam(
				"SuppLangRu.GetFamilyAndInitialsString(\"Эфендиев Эльчин Ильяс оглы\")",
				SuppLangRu.GetFamilyAndInitialsString("Эфендиев Эльчин Ильяс оглы"));
			SuppConsole.WriteLineParam(
				"SuppLangRu.GetFamilyAndInitialsString(\"Ахмедова Лейла кызы\")",
				SuppLangRu.GetFamilyAndInitialsString("Ахмедова Лейла кызы"));
			SuppConsole.WriteLineParam(
				"SuppLangRu.GetFamilyAndInitialsString(\"Мамед-заде Нурлан Ариф улы\")",
				SuppLangRu.GetFamilyAndInitialsString("Мамед-заде Нурлан Ариф улы"));

			var (fam1, init1) = SuppLangRu.GetFamilyAndInitials(
				"Салтыков-Щедрин Михаил Евграфович");
			SuppConsole.WriteLineParam(
				"Транслит (сложная фамилия)",
				SuppLangRu.GetFamilyAndInitialsTranslit(fam1, init1));

			var (fam2, init2) = SuppLangRu.GetFamilyAndInitials(
				"Эфендиев Эльчин Ильяс оглы");
			SuppConsole.WriteLineParam(
				"Транслит (восточное имя)",
				SuppLangRu.GetFamilyAndInitialsTranslit(fam2, init2));

			SuppConsole.WriteLineParam(
				"SuppLangRu.FixTelephoneRuCityCode(\"89211234567\")",
				SuppLangRu.FixTelephoneRuCityCode("89211234567"));
			SuppConsole.WriteLineParam(
				"SuppLangRu.FixTelephoneRuCityCode(\"79211234567\")",
				SuppLangRu.FixTelephoneRuCityCode("79211234567"));

			SuppConsole.WriteLineParam(
				"SuppLangRu.GetTelephoneNumber(\"79211234567\")",
				SuppLangRu.GetTelephoneNumber("79211234567"));
			SuppConsole.WriteLineParam(
				"SuppLangRu.GetTelephoneNumber(\"3224567\")",
				SuppLangRu.GetTelephoneNumber("3224567"));
			SuppConsole.WriteLineParam(
				"SuppLangRu.GetTelephoneNumber(\"01\")",
				SuppLangRu.GetTelephoneNumber("01"));

			SuppConsole.WriteLineParam(
				"SuppLangRu.GetDocNumber(\"Паспорт №45-12/АБ__99\")",
				SuppLangRu.GetDocNumber("Паспорт №45-12/АБ__99"));

			var addrDict = new Dictionary<string, string> {
				{ "ГЛ", "Невский пр., д. 1" },
				{ "ОФ", "Лиговский пр., д. 44" }
			};
			SuppConsole.WriteLineParam(
				"Адрес с кабинетом",
				SuppLangRu.GetSubstitutionAddress("Прибыть в ГЛ:405", addrDict));
			SuppConsole.WriteLineParam(
				"Адрес без кабинета",
				SuppLangRu.GetSubstitutionAddress("Отправить на ОФ:ауд.", addrDict));

			var sbOriginal = new StringBuilder("Ёлка и ружьё под №1");
			var sbInPlace = new StringBuilder("Королёв и Семёнов");
			var sbNewUmlaut = SuppLangRu.GetFixUmlautRu(sbOriginal);
			var sbNewNumber = SuppLangRu.GetFixNumberRu(sbOriginal);
			Console.WriteLine($"Исправление символа ё : {SuppLangRu.GetFixUmlautRu('ё')}");
			Console.WriteLine($"Исправление строки с ё : {SuppLangRu.GetFixUmlautRu("Актёр играет её роль")}");
			Console.WriteLine($"Защита от null при исправлении строк : '{SuppLangRu.GetFixUmlautRu((string?)null)}'");
			Console.WriteLine($"Исправление знака номера : {SuppLangRu.GetFixNumberRu("Документ №123-А")}");
			SuppLangRu.FixUmlautRu(sbInPlace);
			Console.WriteLine($"Замена в StringBuilder по месту : {sbInPlace}");
			Console.WriteLine($"Новый SB (исправление ё) : {sbNewUmlaut}");
			Console.WriteLine($"Новый SB (исправление №) : {sbNewNumber}");
			Console.WriteLine($"Исходный SB остался неизменным : {sbOriginal}");
			Console.WriteLine($"Полный русский семпл : {SuppLangRu.GetSample()}");
			Console.WriteLine($"Средний русский семпл : {SuppLangRu.GetSampleSmall()}");
			Console.WriteLine($"Минимальный русский семпл : {SuppLangRu.GetSampleSmaller()}");

			var sbSrc = new StringBuilder("Подъезд №4, этаж 9.");
			var sbDst = new StringBuilder();
			Console.WriteLine($"Транслитерация (Смешанный регистр) : {SuppLangRu.GetTranslitRuToEn("Юрий Щекочихин")}");
			Console.WriteLine($"Транслитерация (ВЕРХНИЙ РЕГИСТР) : {SuppLangRu.GetTranslitRuToEn("ОБЪЕКТ СЪЁМКИ")}");
			SuppLangRu.TranslitRuToEn(sbSrc, sbDst);
			Console.WriteLine($"Потоковая транслитерация SB : {sbDst}");
			Console.WriteLine($"Генерация URL-slug для статьи : {SuppLangRu.ToSlug("Как правильно готовить борщ? Инструкция № 1 (# 1)!")}");
			Console.WriteLine($"Slug для пустых/null значений : '{SuppLangRu.ToSlug(null)}'");
			Console.WriteLine($"Форма для 1 элемента : {SuppLangRu.GetPlural(1, "комментарий", "комментария", "комментариев")}");
			Console.WriteLine($"Форма для 3 элементов : {SuppLangRu.GetPlural(3, "комментарий", "комментария", "комментариев")}");
			Console.WriteLine($"Форма для 12 элементов (исключение) : {SuppLangRu.GetPlural(12, "комментарий", "комментария", "комментариев")}");
			Console.WriteLine($"Форма для 101 элемента : {SuppLangRu.GetPlural(101, "комментарий", "комментария", "комментариев")}");
			Console.WriteLine($"Плюрализация по шаблону : {SuppLangRu.GetPlural("В корзине {0} {1}", 25, "товар", "товара", "товаров")}");
			Console.WriteLine($"Возраст (заканчивается на 1) : {SuppLangRu.GetPluralAge(21)}");
			Console.WriteLine($"Возраст (заканчивается на 4) : {SuppLangRu.GetPluralAge(34)}");
			Console.WriteLine($"Возраст (исключение 14) : {SuppLangRu.GetPluralAge(14)}");
			Console.WriteLine($"Число прописью (обычное) : {SuppLangRu.GetNumberToText(125)}");
			Console.WriteLine($"Число прописью (с тысячами) : {SuppLangRu.GetNumberToText(2002)}");
			Console.WriteLine($"Число прописью (отрицательное) : {SuppLangRu.GetNumberToText(-5000000)}");
			Console.WriteLine($"Число прописью (миллиарды) : {SuppLangRu.GetNumberToText(3125400000)}");
			Console.WriteLine($"Бухгалтерская сумма (целая) : {SuppLangRu.GetCurrencyToText(1500m)}");
			Console.WriteLine($"Бухгалтерская сумма (с копейками) : {SuppLangRu.GetCurrencyToText(123.456m)}");
			Console.WriteLine($"Бухгалтерская сумма (ноль рублей) : {SuppLangRu.GetCurrencyToText(0.75m)}");



			SuppConsole.PartStart("SuppLinq");

			IQueryable<TestUser> usersQuery = new List<TestUser>
			{
				new(1, "Иван", true, 20),
				new(2, "Петр", false, 25),
				new(3, "Алексей", true, 30),
				new(4, "Сергей", true, 19),
				new(5, "Дмитрий", true, 45),
				new(6, "Михаил", false, 60)
			}.AsQueryable();
			Expression<Func<TestUser, bool>>? filter = null;
			filter = SuppLinq.ApplyFilter_Add(filter, u => u.IsActive);
			filter = SuppLinq.ApplyFilter_Add(filter, u => u.Age > 18);
			var filteredUsers = usersQuery
				.Where(filter)
				.Select(u => u.Name)
				.ToList();
			SuppConsole.WriteLineParam(
				"Динамический фильтр (Иван, Алексей, Сергей, Дмитрий)",
				SuppConsole.ArrToText(filteredUsers));
			var searchFilters = new Expression<Func<TestUser, bool>>[]
			{
				u => u.IsActive == true,
				u => u.Age < 40
			};
			IQueryable<TestUser> preparedQuery = SuppLinq.PrepareQuery(
				usersQuery, searchFilters, "Name", 1, 2);
			var resultList = preparedQuery.Select(u => u.Name).ToList();
			SuppConsole.WriteLineParam(
				"Пагинация и фильтрация запроса (Алексей, Иван)",
				SuppConsole.ArrToText(resultList));



			SuppConsole.PartStart("SuppMath");

			Console.WriteLine($"Удержание числа в лимитах (10-50) : {SuppMath.GetRestrict(85, 10, 50)}");
			Console.WriteLine($"Округление double до int : {SuppMath.RoundToInt(-25.67)}");
			Console.WriteLine($"Округление отрицательного числа до uint : {SuppMath.RoundToUInt(-12.4)}");
			Console.WriteLine($"Линейный маппинг целых чисел : {SuppMath.Map(5, 0, 10, 100, 200, true)}");
			Console.WriteLine($"Поиск следующего числа кратного 5 : {SuppMath.GetNextDivisible(13, 5)}");
			Console.WriteLine($"Поиск индекса диапазона в точках 0,10,50,100 : {SuppMath.GetRangeIndex(35, [0, 10, 50, 100])}");



			SuppConsole.PartStart("SuppRandom");

			Console.WriteLine($"Случайное число от 1 до 10 включительно : {SuppRandom.GetInt(1, 10)}");
			Console.WriteLine($"Случайное число с перевернутым диапазоном (100, 5) : {SuppRandom.GetInt(100, 5)}");
			Console.WriteLine($"Случайное число в экстремальном диапазоне до MaxValue : {SuppRandom.GetInt(int.MaxValue - 5, int.MaxValue)}");
			Console.WriteLine($"Случайная строка по маске длиной от 5 до 12 символов : {SuppRandom.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5, 12)}");



			SuppConsole.PartStart("SuppReflection");

			var typesCount = SuppReflection.GetNamespaceTypes(Assembly.GetExecutingAssembly(), "Ans.Net10.Common").Length;
			var a11 = new int[] { 1, 2, 3, 4 };
			Console.WriteLine($"Алиас системного имени типа Int32 : {SuppReflection.FixCSharpName("Int32")}");
			Console.WriteLine($"Алиас для кастомного класса : {SuppReflection.FixCSharpName("MyCustomClass")}");
			Console.WriteLine($"Значение строки в C# формате : {SuppReflection.GetCSharpValue("Hello")}");
			Console.WriteLine($"Значение пустой строки : {SuppReflection.GetCSharpValue(string.Empty)}");
			Console.WriteLine($"Значение числа : {SuppReflection.GetCSharpValue(125L)}");
			Console.WriteLine($"Значение массива примитивов int[] (без падения!) : {SuppReflection.GetCSharpValue(a11)}");
			Console.WriteLine($"Значение перечисления (Enum Baseline) : {SuppReflection.GetCSharpValue(DayOfWeek.Friday)}");
			Console.WriteLine($"Количество классов библиотеки в текущем пространстве имен : {typesCount}");
			Console.WriteLine($"Значение DateTime в специализированном формате : {SuppReflection.GetCSharpValue(dt1)}");
			Console.WriteLine($"Значение DateOnly в специализированном формате : {SuppReflection.GetCSharpValue(dt1.GetDateOnly())}");
			Console.WriteLine($"Значение TimeOnly в формате ISO : {SuppReflection.GetCSharpValue(dt1.GetTimeOnly())}");



			SuppConsole.PartStart("SuppRegex");

			Console.WriteLine($"Escape (Экранирование спецсимволов регулярок) : {SuppRegex.Escape("Привет. Как дела? (Запрос [id=123] + $10)")}");
			Console.WriteLine($"Escape (Передача пустой строки) : \"{SuppRegex.Escape(string.Empty)}\"");
			Console.WriteLine($"Escape (Передача null) : {SuppRegex.Escape((string?)null) ?? "null"}");



			SuppConsole.PartStart("SuppString");

			string s1 = "Привет";
			string s2 = "";
			string s3 = "Мир";
			Console.WriteLine($"Хотя бы одна не пуста : {SuppString.HasAny(s1, s2, s3)}");
			Console.WriteLine($"Хотя бы одна не пуста (все пустые) : {SuppString.HasAny(s2, null)}");
			Console.WriteLine($"Все строки заполнены : {SuppString.HasAll(s1, s3)}");
			Console.WriteLine($"Все строки заполнены (есть пустая) : {SuppString.HasAll(s1, s2, s3)}");
			Console.WriteLine($"Конвертация Windows-1251 в UTF8 : {SuppString.Convert_WINDOWS1251_UTF8("Тест")}");

			var pair1 = SuppString.GetPair("timeout=30");
			var pair2 = SuppString.GetPair("=no_key");
			Console.WriteLine($"Join с шаблонами элементов : {SuppString.Join("Итог: {0}", "[{0}]", ", ", "A", "B", "", "C")}");
			Console.WriteLine($"Парсинг GetPair (обычный) : Ключ='{pair1.Key}', Значение='{pair1.Value}'");
			Console.WriteLine($"Парсинг GetPair (без ключа) : Ключ='{pair2.Key}', Значение='{pair2.Value}'");
			Console.WriteLine($"Преобразование регистра по Enum : {SuppString.GetModCase("тестовая СТРОКА", LetterCasesEnum.TitleCase)}");
			Console.WriteLine($"Очистка спецсимволов (ASCII < 32) : {SuppString.GetFixSpecChars("Линия1\nЛиния2\tТаб")}");

			string customInput = "Параметр ∑ и префикс Ω!";
			Console.WriteLine($"NameString с кодированием >126 : {SuppString.GetSafeNameString(customInput)}");
			Console.WriteLine($"FsString с кодированием >126   : {SuppString.GetSafeFsString(customInput)}");



			SuppConsole.PartStart("SuppStringBuilder");

			var sb1 = new StringBuilder("Успешный\tТекст\nБез\rОшибок");
			var sb2 = new StringBuilder("Чистый Текст");
			SuppStringBuilder.FixSpecChars(sb1);
			SuppStringBuilder.FixSpecChars(sb2);
			Console.WriteLine($"Очистка строки со знаками табуляции и переноса : {sb1?.ToString()}");
			Console.WriteLine($"Работа метода с чистым текстом (быстрый выход) : {sb2?.ToString()}");



			SuppConsole.PartStart("SuppValues");

			string? emptyStr = "";
			string? nullStr = null;
			int dbId = 0;
			Console.WriteLine($"Выбор первого непустого дефолта для строки : {SuppValues.Default(emptyStr, nullStr, "Резерв", "Второй Резерв")}");
			Console.WriteLine($"Подстановка дефолта для числового ID (если он равен 0) : {SuppValues.Default(dbId, 999, 0)}");
			Console.WriteLine($"Проверка HasAny на разнородных объектах (Zero-Allocation) : {SuppValues.HasAny(nullStr, "", 42, DayOfWeek.Monday)}");
			Console.WriteLine($"Проверка HasAll с пустым элементом в середине : {SuppValues.HasAll("Roman", 100, "", DateTime.Now)}");

			DateTime date1 = new(2026, 1, 1);
			DateTime? date2 = new(2026, 5, 1);
			DateTime? dateNull = null;
			int num1 = 42;
			int? num2 = 100;
			int? numNull = null;
			Console.WriteLine($"Max для структур (сравнение с датой): {SuppValues.MaxValue(date1, date2):dd.MM.yyyy}");
			Console.WriteLine($"Max для структур (вторая дата null): {SuppValues.MaxValue(date1, dateNull):dd.MM.yyyy}");
			Console.WriteLine($"Min для структур (сравнение с датой): {SuppValues.MinValue(date1, date2):dd.MM.yyyy}");
			Console.WriteLine($"Min для структур (вторая дата null): {SuppValues.MinValue(date1, dateNull):dd.MM.yyyy}");
			Console.WriteLine($"MaxNum для чисел (сравнение с числом): {SuppValues.MaxNum(num1, num2)}");
			Console.WriteLine($"MaxNum для чисел (второе число null): {SuppValues.MaxNum(num1, numNull)}");
			Console.WriteLine($"MinNum для чисел (сравнение с числом): {SuppValues.MinNum(num1, num2)}");
			Console.WriteLine($"MinNum для чисел (второе число null): {SuppValues.MinNum(num1, numNull)}");

			SuppConsole.WriteLineParam(
				"SuppValues.GetStringForWeb(DateTime.UtcNow)",
				SuppValues.GetStringForWeb(DateTime.UtcNow));
			SuppConsole.WriteLineParam(
				"SuppValues.GetStringForWeb(DateOnly.FromDateTime(DateTime.Today))",
				SuppValues.GetStringForWeb(DateOnly.FromDateTime(DateTime.Today)));
			SuppConsole.WriteLineParam(
				"SuppValues.GetStringForWeb(true)",
				SuppValues.GetStringForWeb(true));

			SuppConsole.WriteLineParam(
				"SuppValues.GetDigitalOnly(\"тел: +7 (999) 123-45-67\")",
				SuppValues.GetDigitalOnly("тел: +7 (999) 123-45-67"));

			SuppConsole.WriteLineParam(
				"SuppValues.GetCurrencyLoc(12345.678m)",
				SuppValues.GetCurrencyLoc(12345.678m));



			SuppConsole.PartStart("SuppXml");

			var sampleObj = new CatalogItem
			{
				Id = 101,
				Name = "Процессор Intel Core i9",
				Tags = ["Электроника", "Компоненты", "ПК"]
			};

			string softFile = "catalog_soft.xml";
			string hardFile = "catalog_hard.xml";
			string? xmlString = SuppXml.GetXmlStringFromObject(sampleObj, useFormatted: true);
			SuppConsole.WriteLineParam(
				"SuppXml.GetXmlStringFromObject(sampleObj, useFormatted: true)",
				xmlString ?? "null");
			var objFromStr = SuppXml.GetObjectFromXmlString<CatalogItem>(xmlString ?? string.Empty);
			SuppConsole.WriteLineParam(
				"Идентификатор из XML-строки",
				objFromStr?.Id ?? 0);
			SuppConsole.WriteLineParam(
				"Теги из XML-строки",
				SuppConsole.ArrToText(objFromStr?.Tags));

			XDocument? xDoc = SuppXml.GetXDocumentFromObject(sampleObj);
			SuppConsole.WriteLineParam("Корневой элемент XDocument", xDoc?.Root?.Name.LocalName ?? "нет");

			var objFromXDoc = SuppXml.GetObjectFromXDocument<CatalogItem>(xDoc!);
			SuppConsole.WriteLineParam("Имя из XDocument", objFromXDoc?.Name ?? "нет");

			await SuppXml.SaveSoftObjectToXmlFileAsync(sampleObj, softFile, Encoding.UTF8, useFormatted: true);
			var softLoaded = await SuppXml.GetSoftObjectFromXmlFileAsync<CatalogItem>(softFile);
			SuppConsole.WriteLineParam("Загружено через Soft-метод", softLoaded?.Name ?? "нет");

			await SuppXml.SaveHardObjectToXmlFileAsync(sampleObj, hardFile, Encoding.UTF8, useFormatted: true);
			var hardLoaded = await SuppXml.GetHardObjectFromXmlFileAsync<CatalogItem>(hardFile);
			SuppConsole.WriteLineParam("Загружено через Hard-метод", hardLoaded?.Name ?? "нет");

		}


		static async Task _outClassesAsync()
		{
			SuppConsole.SectionStart("CLASSES");



			SuppConsole.PartStart("_ListPaginatedModel_Base");

			var dbQuery = new List<UserEntity>
			{
				new() { Id = 1, FullName = "Иванов И." },
				new() { Id = 2, FullName = "Петров П." },
				new() { Id = 3, FullName = "Сидоров С." },
				new() { Id = 4, FullName = "Алексеев А." },
				new() { Id = 5, FullName = "Борисов Б." },
				new() { Id = 6, FullName = "Федоров Ф." },
				new() { Id = 7, FullName = "Михайлов М." }
			}.AsQueryable();
			var pagedViewModel = new UserListPagedModel(dbQuery, page: 2, itemsOnPage: 3);
			var namesOnPage = pagedViewModel.Items.Select(u => u.DisplayName).ToArray();
			SuppConsole.WriteLineParam(
				"Имена на 2-й странице (без двойных SQL-запросов)",
				SuppConsole.ArrToText(namesOnPage));
			SuppConsole.WriteLineParam(
				"Количество элементов на текущей странице",
				pagedViewModel.ItemsCount);
			SuppConsole.WriteLineParam(
				"Общее количество записей в БД (из хелпера)",
				pagedViewModel.Pagination.TotalItems);



			SuppConsole.PartStart("_TreeItem_Base");

			var root = new MenuItem("Главная", "/");
			var catalog = new MenuItem("Каталог", "/catalog");
			var contacts = new MenuItem("Контакты", "/contacts");
			root.AppendChildren(catalog, contacts);
			var electronics = new MenuItem("Электроника", "/catalog/electronics");
			var smartphones = new MenuItem("Смартфоны", "/catalog/electronics/smartphones");
			catalog.AppendChild(electronics);
			electronics.AppendChild(smartphones);
			SuppConsole.WriteLineParam("Корень имеет родителя?", root.HasParent);
			SuppConsole.WriteLineParam("Каталог имеет дочерние элементы?", catalog.HasChildren);
			SuppConsole.WriteLineParam("Количество прямых потомков корня", root.Children.Count());
			var parentTitles = smartphones.Parents.Cast<MenuItem>().Select(p => p.Title);
			SuppConsole.WriteLineParam("Цепочка родителей для 'Смартфоны'", SuppConsole.ArrToText(parentTitles));
			var foundNode = root.FindItem<MenuItem>(item => item.Url == "/catalog/electronics/smartphones");
			SuppConsole.WriteLineParam("Результат поиска узла по URL", foundNode?.Title ?? "Не найден");
			try
			{
				smartphones.AppendChild(catalog);
			}
			catch (InvalidOperationException ex)
			{
				SuppConsole.WriteLineParam("Защита от циклической петли", ex.Message);
			}



			SuppConsole.PartStart("_WebResult_Base");



			SuppConsole.PartStart("ClassIdWrapper");



			SuppConsole.PartStart("CollectionsComparer");

			var currentProducts = new List<ProductModel>
			{
				new(1, "Монитор 24'", 15000m),
				new(2, "Клавиатура механическая", 4500m),
				new(3, "Мышь беспроводная", 2300m)
			};
			var newestProducts = new List<ProductModel>
			{
				new(2, "Клавиатура механическая", 4800m),
				new(3, "Мышь беспроводная", 2300m),
				new(4, "Коврик для мыши XL", 1200m)
			};
			var collComparer = new CollectionsComparer<ProductModel, ProductModel, int>(
				current: currentProducts,
				newest: newestProducts,
				currentKeySelector: p => p.Id,
				newestKeySelector: p => p.Id,
				funcDataDiff: (oldProd, newProd) => oldProd.Price != newProd.Price || oldProd.Name != newProd.Name
			);
			SuppConsole.WriteLineParam("Есть ли добавленные позиции?", collComparer.HasAdded);
			SuppConsole.WriteLineParam("Количество добавленных товаров", collComparer.AddedCount);
			SuppConsole.WriteLineParam("Список добавленных (Имена)", SuppConsole.ArrToText(collComparer.Added.Select(x => x.Name)));
			SuppConsole.WriteLineParam("Есть ли удаленные позиции?", collComparer.HasDeleted);
			SuppConsole.WriteLineParam("Количество удаленных товаров", collComparer.DeletedCount);
			SuppConsole.WriteLineParam("Количество позиций с изменившейся ценой", collComparer.ChangedCount);
			var changedDetails = collComparer.Changed.Select(x => $"{x.Current.Name} (Старая цена: {x.Current.Price} -> Новая: {x.Newest.Price})");
			SuppConsole.WriteLineParam("Детали изменений", SuppConsole.ArrToText(changedDetails));
			collComparer.TestDebug();



			SuppConsole.PartStart("ConsoleMenu");



			SuppConsole.PartStart("ContentInfo");

			var jpegInfo = new ContentInfo(".jpg", "image/jpeg", ContentGroupEnum.Image, isWebImage: true, isJpeg: true);
			var invalidInfo = new ContentInfo(".xyz", "invalid/mime/type", ContentGroupEnum.Text);
			Console.WriteLine($"Информация о JPEG контенте : {jpegInfo}");
			Console.WriteLine($"Проверка флага IsImage : {jpegInfo.IsImage}");
			Console.WriteLine($"Парсинг валидного MediaType : {jpegInfo.MediaType}");
			Console.WriteLine($"Парсинг невалидного MediaType (дефолт) : {invalidInfo.MediaType}");



			SuppConsole.PartStart("CrudFace");

			var face1 = new CrudFace(
				"UserEmail",
				"Адрес электронной почты",
				"Email",
				"Введите рабочий email",
				"user@example.com",
				"https://docs.site");
			SuppConsole.WriteLineParam("face1", face1);
			SuppConsole.WriteLineParam("face1.Name", face1.Name);
			SuppConsole.WriteLineParam("face1.TitleRaw", face1.TitleRaw);
			SuppConsole.WriteLineParam("face1.Title", face1.Title);
			SuppConsole.WriteLineParam("face1.ShortTitleRaw", face1.ShortTitleRaw);
			SuppConsole.WriteLineParam("face1.ShortTitle", face1.ShortTitle);
			SuppConsole.WriteLineParam("face1.Description", face1.Description);
			SuppConsole.WriteLineParam("face1.Sample", face1.Sample);
			SuppConsole.WriteLineParam("face1.HelpLink", face1.HelpLink);
			SuppConsole.WriteLineParam("face1.HasFace", face1.HasFace);

			var face2 = new CrudFace("" +
				"SecretToken",
				"Токен||Служебный токен доступа");
			SuppConsole.WriteLineParam("face2", face2);
			SuppConsole.WriteLineParam("face2.Name", face2.Name);
			SuppConsole.WriteLineParam("face2.TitleRaw", face2.TitleRaw);
			SuppConsole.WriteLineParam("face2.Title", face2.Title);
			SuppConsole.WriteLineParam("face2.ShortTitleRaw", face2.ShortTitleRaw);
			SuppConsole.WriteLineParam("face2.ShortTitle", face2.ShortTitle);
			SuppConsole.WriteLineParam("face2.Description", face2.Description);
			SuppConsole.WriteLineParam("face2.Sample", face2.Sample);
			SuppConsole.WriteLineParam("face2.HelpLink", face2.HelpLink);
			SuppConsole.WriteLineParam("face2.HasFace", face2.HasFace);

			var face3 = new CrudFace("EmptyField", "");
			SuppConsole.WriteLineParam("face3", face3);
			SuppConsole.WriteLineParam("face3.Name", face3.Name);
			SuppConsole.WriteLineParam("face3.TitleRaw", face3.TitleRaw);
			SuppConsole.WriteLineParam("face3.Title", face3.Title);
			SuppConsole.WriteLineParam("face3.ShortTitleRaw", face3.ShortTitleRaw);
			SuppConsole.WriteLineParam("face3.ShortTitle", face3.ShortTitle);
			SuppConsole.WriteLineParam("face3.Description", face3.Description);
			SuppConsole.WriteLineParam("face3.Sample", face3.Sample);
			SuppConsole.WriteLineParam("face3.HelpLink", face3.HelpLink);
			SuppConsole.WriteLineParam("face3.HasFace", face3.HasFace);



			SuppConsole.PartStart("DateTimeHelper");

			var helper = new DateTimeHelper();
			Console.WriteLine($"GetPassed (Сегодня с подстановкой слова) : {helper.GetPassed(helper.Today.AddHours(14), true, true)}");
			Console.WriteLine($"GetPassed (Сегодня без подстановки слова) : {helper.GetPassed(helper.Today.AddHours(14), true, false)}");
			Console.WriteLine($"GetPassed (Вчера со временем) : {helper.GetPassed(helper.Yesterday.AddHours(10), true, true)}");
			Console.WriteLine($"GetPassed (Завтра только дата) : {helper.GetPassed(helper.Tomorrow, false, true)}");
			Console.WriteLine($"GetPassed (Послезавтра в этом году) : {helper.GetPassed(helper.TomorrowAfter, false, true)}");
			Console.WriteLine($"GetPassed (Прошлый год) : {helper.GetPassed(helper.CurrentYearBegin.AddDays(-10), false, true)}");
			Console.WriteLine($"GetPassed (Значение null) : {helper.GetPassed((DateTime?)null, true, true)}");
			Console.WriteLine($"GetPassed (DateOnly Сегодня) : {helper.GetPassed(DateOnly.FromDateTime(helper.Today), true)}");



			SuppConsole.PartStart("DictInt");

			var rawIntData = "100=Продолжайте;200=Успешно;404=Не найдено";
			var intDict = new DictInt(rawIntData);
			SuppConsole.WriteLineParam("intDict.ToString()", intDict.ToString());
			SuppConsole.WriteLineParam("intDict.Count", intDict.Count);
			SuppConsole.WriteLineParam("intDict[200]", intDict[200]);
			SuppConsole.WriteLineParam("intDict.GetValueOrKey(404)", intDict.GetValueOrKey(404));
			SuppConsole.WriteLineParam("intDict.GetValueOrKey(500)", intDict.GetValueOrKey(500));



			SuppConsole.PartStart("DictString");

			var rawStringData = "Host=localhost;fake\\=test=123\\;456;Port=5432;DbName=MainCatalog;";
			var stringDict = new DictString(rawStringData);
			stringDict.Add("UserAdmin", "SuperBoss");
			SuppConsole.WriteLineParam("stringDict.ToString()", stringDict.ToString());
			SuppConsole.WriteLineParam("stringDict.Count", stringDict.Count);
			SuppConsole.WriteLineParam("stringDict[\"Host\"]", stringDict["Host"]);
			SuppConsole.WriteLineParam("stringDict.GetValueOrKey(\"Port\")", stringDict.GetValueOrKey("Port"));



			SuppConsole.PartStart("DictTagStylers");

			var stylers = new DictTagStylers(
				"main_box|container active|padding:10px;margin:0;",
				"alert_error|btn btn-danger|display:none;",
				"",
				"text_muted||color:#6c757d;"
			);
			SuppConsole.WriteLineParam(
				"Количество успешно загруженных стилизаторов",
				stylers.Count);
			if (stylers.TryGetValue("main_box", out var mainStyler))
			{
				SuppConsole.WriteLineParam(
					"Классы для 'main_box'",
					mainStyler.Classes?.ToString() ?? "нет");
				SuppConsole.WriteLineParam(
					"Стили для 'main_box'",
					mainStyler.Styles?.ToString() ?? "нет");
			}
			string[] keys = new string[stylers.Count];
			int index = 0;
			foreach (var pair in stylers)
				keys[index++] = pair.Key;
			SuppConsole.WriteLineParam(
				"Зарегистрированные ключи в словаре",
				SuppConsole.ArrToText(keys));



			SuppConsole.PartStart("ImageResizeHelper");

			var resize = new ImageResizeHelper(1920, 1080);
			SuppConsole.WriteLineParam("resize.ToString()", resize.ToString());
			SuppConsole.WriteLineParam("resize.Orientation", resize.Orientation);
			SuppConsole.WriteLineParam("resize.Ratio", resize.Ratio);
			SuppConsole.WriteLineParam("resize.IsNearSquare", resize.IsNearSquare);
			resize.ScaleInside(500, 500, noIncrease: true);
			SuppConsole.WriteLineParam("ScaleInside 500x500 -> Новая ширина", resize.NewWidth);
			SuppConsole.WriteLineParam("ScaleInside 500x500 -> Новая высота", resize.NewHeight);
			var smallResize = new ImageResizeHelper(300, 200);
			smallResize.ScaleToWidth(600, noIncrease: true);
			SuppConsole.WriteLineParam("ScaleToWidth (300->600, noIncrease) -> Результат ширины", smallResize.NewWidth);
			SuppConsole.WriteLineParam("ScaleToWidth (300->600, noIncrease) -> Результат высоты", smallResize.NewHeight);
			uint imageWidth = 1200;
			uint cropBoxWidth = 800;
			uint startCenter = ImageResizeHelper.GetCropStart(imageWidth, cropBoxWidth, ImageShiftEnum.Center);
			uint startLeft = ImageResizeHelper.GetCropStart(imageWidth, cropBoxWidth, ImageShiftEnum.Start);
			uint startCustom = ImageResizeHelper.GetCropStart(imageWidth, cropBoxWidth, 75);
			SuppConsole.WriteLineParam("Начало обрезки при центрировании (1200 -> 800)", startCenter);
			SuppConsole.WriteLineParam("Начало обрезки с левого края (1200 -> 800)", startLeft);
			SuppConsole.WriteLineParam("Начало обрезки при кастомных 75% (1200 -> 800)", startCustom);



			SuppConsole.PartStart("IPSubnet");



			SuppConsole.PartStart("KeyComparer");

			var oldList = new List<int> { 1, 2, 3, 5 };
			var newList = new List<int> { 3, 5, 7, 8 };
			var intComparer = new KeysComparer(oldList, newList);
			SuppConsole.WriteLineParam("Числа - Есть добавленные?", intComparer.HasAdded);
			SuppConsole.WriteLineParam("Числа - Список добавленных (строкой)", intComparer.AddedString);
			SuppConsole.WriteLineParam("Числа - Есть удаленные?", intComparer.HasDeleted);
			SuppConsole.WriteLineParam("Числа - Список удаленных (строкой)", intComparer.DeletedString);
			var tagsBefore = "10,24,35,40";
			var tagsAfter = "24,40,55,60,70";
			var stringComparer = new KeysComparer(tagsBefore, tagsAfter);
			SuppConsole.WriteLineParam("Строки - Кол-во добавленных ID", stringComparer.Added.Count);
			SuppConsole.WriteLineParam("Строки - Что именно добавилось", SuppConsole.ArrToText(stringComparer.Added));
			SuppConsole.WriteLineParam("Строки - Что именно удалилось", SuppConsole.ArrToText(stringComparer.Deleted));
			var nullComparer = new KeysComparer(null, "5,6,7");
			SuppConsole.WriteLineParam("Устойчивость к null - Добавлено строк", nullComparer.AddedString);



			SuppConsole.PartStart("MailMessageModel");



			SuppConsole.PartStart("MemoryCacheHelper");

			var nativeCache = new MemoryCache(new MemoryCacheOptions());
			var customOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20)
			};
			var cacheHelper = new MemoryCacheHelper(nativeCache, customOptions);
			SuppConsole.WriteLineParam(
				"Время жизни кэша по умолчанию в текущем экземпляре хелпера (сек)",
				cacheHelper.DefaultCacheOptions.AbsoluteExpirationRelativeToNow?.TotalSeconds);
			int fetchCounter = 0;
			string func() { fetchCounter++; return "Active"; }
			string key = "db_status_flag";
			var val1 = cacheHelper.Get(key, func);
			SuppConsole.WriteLineParam(
				"Первый вызов (срабатывание фабрики)", val1);
			var val2 = cacheHelper.Get(key, func);
			SuppConsole.WriteLineParam(
				"Второй вызов (взято из сохраненного кэша)", val2);
			SuppConsole.WriteLineParam(
				"Фактическое количество обращений к источнику", fetchCounter);



			SuppConsole.PartStart("ObjInfoBuilder");

			var myAccount = new Account("RomanKoff", 5000) { Id = 101 };
			var info = new ObjInfoBuilder(typeof(Account), myAccount);
			var rwProps = info.ReadWriteProperties
				.Select(p => $"{p.TypeName} {p.Name} = {p.Value}")
				.ToArray();
			SuppConsole.WriteLineParam(
				"Свойства Read-Write (Тип Имя = Значение)",
				SuppConsole.ArrToText(rwProps));
			var roProps = info.ReadOnlyProperties
				.Select(p => $"{p.TypeName} {p.Name} = {p.Value}")
				.ToArray();
			SuppConsole.WriteLineParam(
				"Свойства Read-Only (Тип Имя = Значение)",
				SuppConsole.ArrToText(roProps));
			var woProps = info.WriteOnlyProperties
				.Select(p => $"{p.TypeName} {p.Name}")
				.ToArray();
			SuppConsole.WriteLineParam(
				"Свойства Write-Only (Корректный тип)",
				SuppConsole.ArrToText(woProps));
			var functionsList = info.Functions
				.Select(f => $"{f.Return} {f.Name}{f.Generics}")
				.ToArray();
			SuppConsole.WriteLineParam(
				"Методы-функции класса",
				SuppConsole.ArrToText(functionsList));
			var methodsList = info.Methods
				.Select(m => m.Name)
				.ToArray();
			SuppConsole.WriteLineParam(
				"Обычные методы класса (void)",
				SuppConsole.ArrToText(methodsList));



			SuppConsole.PartStart("OrderBuilder");

			var order = new OrderBuilder("Title,-Date,CreatedAt");
			SuppConsole.WriteLineParam("OrderBuilder", order);
			SuppConsole.WriteLineParam("order.Items[0].IsDescending", order.Items[0].IsDescending);
			SuppConsole.WriteLineParam("order.Items[1].IsDescending", order.Items[1].IsDescending);
			SuppConsole.WriteLineParam("order.GetLinqCode(\"\\t\")", order.GetLinqCode("\t"));



			SuppConsole.PartStart("PaginatedDataModel");

			var users = new[]
			{
				new { Id = 1, Name = "Алексей" },
				new { Id = 2, Name = "Борис" },
				new { Id = 3, Name = "Владимир" },
				new { Id = 4, Name = "Денис" },
				new { Id = 5, Name = "Егор" }
			}.AsQueryable();
			var modelStandard = new PaginatedDataModel(
				order: "Name",
				page: 2,
				itemsOnPage: 2,
				totalItems: 5,
				defaultItemsOnPage: 10,
				maxItemsOnPages: 20
			);
			var namesPage2 = modelStandard
				.GetQueryTransform(users)
				.Select(u => u.Name)
				.ToArray();
			SuppConsole.WriteLineParam(
				"Вторая страница выборки (по 2 элемента)",
				SuppConsole.ArrToText(namesPage2));
			var modelOverloaded = new PaginatedDataModel(
				order: "-Id",
				page: 1,
				itemsOnPage: 500, // Передано аномально большое число
				totalItems: 5,
				defaultItemsOnPage: 2,
				maxItemsOnPages: 3 // Будет жестко ограничено до 3
			);
			var idsCapped = modelOverloaded
				.GetQueryTransform(users)
				.Select(u => u.Id.ToString())
				.ToArray();
			SuppConsole.WriteLineParam(
				"Ограничение выборки по maxItemsOnPages",
				SuppConsole.ArrToText(idsCapped));
			var modelDefault = new PaginatedDataModel(
				order: null,
				page: 1,
				itemsOnPage: 0, // Сработает дефолтное значение
				totalItems: 5,
				defaultItemsOnPage: 4,
				maxItemsOnPages: 10
			);
			var idsDefault = modelDefault
				.GetQueryTransform(users)
				.Select(u => u.Id.ToString())
				.ToArray();
			SuppConsole.WriteLineParam(
				"Применение defaultItemsOnPage при значении 0",
				SuppConsole.ArrToText(idsDefault));



			SuppConsole.PartStart("PaginatedQueryableHelper");

			var logTable = new List<LogEntry>();
			for (int i = 1; i <= 12; i++)
				logTable.Add(new LogEntry { Id = i, Message = $"Лог номер {i}" });
			var logQueryable = logTable.AsQueryable();
			var pagedResult1 = new PaginatedQueryableHelper<LogEntry>(
				query: logQueryable,
				page: 2,
				itemsOnPage: 5);
			var currentIds = pagedResult1.Query.Select(l => l.Id.ToString()).ToArray();
			SuppConsole.WriteLineParam(
				"ID элементов на 2-й странице (размер 5)",
				SuppConsole.ArrToText(currentIds));
			SuppConsole.WriteLineParam(
				"Общее количество рассчитанных страниц",
				pagedResult1.PaginationHelper.TotalPages);
			var pagedResultExceptionSafe = new PaginatedQueryableHelper<LogEntry>(
				query: logQueryable,
				page: 1,
				itemsOnPage: 0); // Передан 0, код безопасно скорректирует в 1
			var safeCount = pagedResultExceptionSafe.Query.Count();
			SuppConsole.WriteLineParam(
				"Количество элементов на странице после коррекции нуля",
				safeCount);



			SuppConsole.PartStart("PaginationHelper");

			var pagerMiddle = new PaginationHelper(itemsOnPage: 20, totalItems: 500, currentPage: 12, offset: 3);
			SuppConsole.WriteLineParam("Середина списка - Всего страниц", pagerMiddle.TotalPages);
			SuppConsole.WriteLineParam("Середина списка - Сколько пропустить элементов (Skip)", pagerMiddle.SkipItems);
			SuppConsole.WriteLineParam("Середина списка - Стартовая видимая страница", pagerMiddle.StartPage);
			SuppConsole.WriteLineParam("Середина списка - Конечная видимая страница", pagerMiddle.EndPage);
			SuppConsole.WriteLineParam("Середина списка - Текстовый дамп", pagerMiddle);
			var pagerInvalid = new PaginationHelper(itemsOnPage: 10, totalItems: 100, currentPage: -5);
			SuppConsole.WriteLineParam("Коррекция - Был ли индекс невалидным?", pagerInvalid.NotValidIndex);
			SuppConsole.WriteLineParam("Коррекция - Скорректированная текущая страница", pagerInvalid.CurrentPage);
			var pagerFirst = new PaginationHelper(itemsOnPage: 15, totalItems: 150, currentPage: 1, offset: 4);
			SuppConsole.WriteLineParam("Начало списка - Является ли первой страницей?", pagerFirst.ActiveFirstPage);
			SuppConsole.WriteLineParam("Начало списка - Есть ли скрытые страницы до?", pagerFirst.HasItemsBefore);
			SuppConsole.WriteLineParam("Начало списка - Дамп", pagerFirst);



			SuppConsole.PartStart("PaginationModel");

			var helper1 = new PaginationHelper(itemsOnPage: 10, totalItems: 100, currentPage: 3);
			var model = new PaginationModel(helper1);
			SuppConsole.WriteLineParam("Модель - Текущая страница", model.CurrentPage);
			SuppConsole.WriteLineParam("Модель - Всего элементов", model.TotalItems);
			SuppConsole.WriteLineParam("Модель - Сколько пропустить (SkipItems)", model.SkipItems);
			SuppConsole.WriteLineParam("Модель - Активна первая страница?", model.ActiveFirstPage);
			var (current, _, totalPages, _, _, _, _, _, _, _, _, _, _, _, _) = model;
			SuppConsole.WriteLineParam("Проверка деконструкции структуры (Страниц всего)", totalPages);



			SuppConsole.PartStart("ParamsBuilder, ParamsCollection");

			var builder = new ParamsBuilder();
			builder.Append("search", "Net10");
			builder.Append("page", 2);
			builder.Append("per_page", 0); // Число 0 проигнорируется, параметр не добавится
			builder.Append("is_active", true);
			builder.Append("archive", false); // Значение false проигнорируется
			builder.Append("date", new DateOnly(2026, 09, 19));
			builder.Append("time", new TimeOnly(14, 30, 00));
			SuppConsole.WriteLineParam("Базовая строка параметров", builder.ToString());
			string tempUrl = builder.GetString("page", 3); // Перезаписываем существующий параметр "page" для конкретной ссылки
			SuppConsole.WriteLineParam("Временная строка со сменой страницы", tempUrl);
			string reportUrl = builder.GetString("format", "pdf");
			SuppConsole.WriteLineParam("Временная строка с добавлением формата", reportUrl);
			SuppConsole.WriteLineParam("Исходное состояние строителя не изменилось", builder.ToString());



			SuppConsole.PartStart("RegistryItem");

			var item1 = new RegistryItem(
				"complex=key",
				"value1;value2#section:test",
				0,
				false);
			SuppConsole.WriteLineParam(
				"item1", item1);
			SuppConsole.WriteLineParam("item1.Key", item1.Key);
			SuppConsole.WriteLineParam("item1.Value", item1.Value);

			var item2 = new RegistryItem("complex\\=key=value1\\;value2\\#section\\:test");
			SuppConsole.WriteLineParam(
				"item2", item2);
			SuppConsole.WriteLineParam("item2.Key", item2.Key);
			SuppConsole.WriteLineParam("item2.Value", item2.Value);



			SuppConsole.PartStart("RegistryList");

			var reg = new RegistryList("!;*;mode=Production;timeout=30;v\\;key=val");
			SuppConsole.WriteLineParam("Сериализованный реестр", reg);
			SuppConsole.WriteLineParam(
				"reg.Items.Select(x=>x.Key)",
				SuppConsole.ArrToText(reg.Items.Select(x => x.Key)));
			SuppConsole.WriteLineParam(
				"reg.Items.Select(x=>x.Value)",
				SuppConsole.ArrToText(reg.Items.Select(x => x.Value)));
			SuppConsole.WriteLineParam("reg.GetMaxWidth()", reg.GetMaxWidth());
			SuppConsole.WriteLineParam("reg.GetProposeMode()", reg.GetProposeMode());
			SuppConsole.WriteLineParam("reg.GetProposeWidth()", reg.GetProposeWidth());



			SuppConsole.PartStart("ResourcesHelper");

			var resourcesHelper = new ResourcesHelper();
			var emailFace = resourcesHelper.GetCrudFace("Email");
			if (emailFace != null)
			{
				SuppConsole.WriteLineParam("emailFace", emailFace);
				SuppConsole.WriteLineParam("emailFace.Name", emailFace.Name);
				SuppConsole.WriteLineParam("emailFace.TitleRaw", emailFace.TitleRaw);
				SuppConsole.WriteLineParam("emailFace.Title", emailFace.Title);
				SuppConsole.WriteLineParam("emailFace.ShortTitleRaw", emailFace.ShortTitleRaw);
				SuppConsole.WriteLineParam("emailFace.ShortTitle", emailFace.ShortTitle);
				SuppConsole.WriteLineParam("emailFace.Description", emailFace.Description);
				SuppConsole.WriteLineParam("emailFace.Sample", emailFace.Sample);
				SuppConsole.WriteLineParam("emailFace.HelpLink", emailFace.HelpLink);
				SuppConsole.WriteLineParam("emailFace.HasFace", emailFace.HasFace);
			}
			var unknownFace = resourcesHelper.GetCrudFace("Unknown_Field_Key");
			if (unknownFace != null)
			{
				SuppConsole.WriteLineParam("unknownFace", unknownFace);
				SuppConsole.WriteLineParam("unknownFace.Name", unknownFace.Name);
				SuppConsole.WriteLineParam("unknownFace.TitleRaw", unknownFace.TitleRaw);
				SuppConsole.WriteLineParam("unknownFace.Title", unknownFace.Title);
				SuppConsole.WriteLineParam("unknownFace.ShortTitleRaw", unknownFace.ShortTitleRaw);
				SuppConsole.WriteLineParam("unknownFace.ShortTitle", unknownFace.ShortTitle);
				SuppConsole.WriteLineParam("unknownFace.Description", unknownFace.Description);
				SuppConsole.WriteLineParam("unknownFace.Sample", unknownFace.Sample);
				SuppConsole.WriteLineParam("unknownFace.HelpLink", unknownFace.HelpLink);
				SuppConsole.WriteLineParam("unknownFace.HasFace", unknownFace.HasFace);
			}



			SuppConsole.PartStart("StringParser");

			string configLine = "AppService|1024|true|2026-09-19|18:45:00||ДопТекст";
			SuppConsole.WriteLineParam("configLine", configLine);
			var parser = new StringParser(configLine);
			SuppConsole.WriteLineParam("parser.Get(0)", parser.Get(0));
			SuppConsole.WriteLineParam("parser.Get(5, \"По умолчанию\")", parser.Get(5, "По умолчанию"));
			SuppConsole.WriteLineParam("parser.Get(20, \"Не найден\")", parser.Get(20, "Не найден"));
			SuppConsole.WriteLineParam("parser.GetInt(1)", parser.GetInt(1));
			SuppConsole.WriteLineParam("parser.GetInt(0, -1)", parser.GetInt(0, -1));
			SuppConsole.WriteLineParam("parser.GetBool(2)", parser.GetBool(2));
			var parsedDate = parser.GetDateOnly(3);
			SuppConsole.WriteLineParam(
				"parsedDate?.ToString(\"dd.MM.yyyy\") ?? \"null\"",
				parsedDate?.ToString("dd.MM.yyyy") ?? "null");
			SuppConsole.WriteLineParam(
				"parser.GetTimeOnly(4, \"HH:mm\") ?? \"null\"",
				parser.GetTimeOnly(4, "HH:mm") ?? "null");
			var emptyParser = new StringParser(string.Empty);
			SuppConsole.WriteLineParam(
				"Безопасный вызов при пустом источнике",
				emptyParser.Get(0, "Защищено"));



			SuppConsole.PartStart("TagAttributesBuilder, TagClassesBuilder, TagStyler, TagStylesBuilder");

			var classesBuilder = new TagClassesBuilder("btn btn-primary active btn-lg col-md-6 col-md-offset-3");
			classesBuilder.Append("btn-danger col-md-4");
			SuppConsole.WriteLineParam("Сборка и группировка CSS-классов", classesBuilder.ToString() ?? "null");

			var stylesBuilder = new TagStylesBuilder("color:red; margin-top:10px; padding: 5px;");
			stylesBuilder.Append("color:blue; display:block;"); // color перезапишется, display добавится
			stylesBuilder.ApplyOriginal("margin-top:25px; font-size:14px;"); // margin-top оригинальный (10px) сохранится, font-size добавится
			SuppConsole.WriteLineParam("Сборка инлайновых CSS-стилей", stylesBuilder.ToString());

			var attrBuilder = new TagAttributesBuilder("id=\"main-container\" data-id=\"45\" disabled");
			attrBuilder.Append("data-id=\"99\" href=\"/home\""); // Обновляем data-id и добавляем href
			SuppConsole.WriteLineParam("Исправленная генерация HTML-атрибутов", attrBuilder.ToString());

			var defaultStyler = new TagStyler("alert alert-warning", "padding:15px;");
			var customStyler = new TagStyler("alert-danger custom-theme", "margin:5px; padding:20px;");
			customStyler.ApplyBase(defaultStyler);
			string resultHtml = $"<div class=\"{customStyler.Classes}\" style=\"{customStyler.Styles}\">Внимание!</div>";
			SuppConsole.WriteLineParam("HTML блок, собранный через TagStyler", resultHtml);



			SuppConsole.PartStart("TinyBreakerHelper");

			var breaker = new TinyBreakerHelper(step: 3);
			var iterationResults = new List<string>();
			for (int i = 1; i <= 7; i++)
				if (breaker.Next())
					iterationResults.Add($"Триггер на {i}");
			SuppConsole.WriteLineParam("Срабатывания прерывателя (шаг 3)", SuppConsole.ArrToText(iterationResults.ToArray()));



			SuppConsole.PartStart("TinyLogWriterHelper");

			string tempLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_test.log");
			var logger = new TinyLogWriterHelper(tempLogFile, rewrite: true, length: 2);
			logger.AppendLine("Запись 1");
			logger.AppendLine("Запись 2");
			logger.AppendLine("Запись 3");
			logger.Save();
			string fileContent = File.ReadAllText(tempLogFile).Replace("\r\n", " | ").TrimEnd(' ', '|');
			SuppConsole.WriteLineParam("Содержимое сброшенного на диск лога", fileContent);



			SuppConsole.PartStart("TinyTimerHelper");

			var timer = new TinyTimerHelper(TimeSpan.FromMilliseconds(50), useEqualIntervals: true);
			bool checkImmediate = timer.Test();
			SuppConsole.WriteLineParam("Проверка таймера сразу после создания", checkImmediate);
			Thread.Sleep(60);
			bool checkAfterDelay = timer.Test();
			SuppConsole.WriteLineParam("Проверка таймера после задержки в 60мс", checkAfterDelay);



			SuppConsole.PartStart("TreeEntityHelper");

			var categories = new List<CategoryEntity>
			{
				new() { Id = 1, ParentPtr = null, Order = 1, Title = "Электроника" },
				new() { Id = 2, ParentPtr = 1, Order = 2, Title = "Смартфоны" },
				new() { Id = 3, ParentPtr = 1, Order = 1, Title = "Ноутбуки" },
				new() { Id = 4, ParentPtr = null, Order = 2, Title = "Одежда" },
				new() { Id = 5, ParentPtr = 4, Order = 1, Title = "Мужская одежда" },
				new() { Id = 6, ParentPtr = 2, Order = 1, Title = "Аксессуары для смартфонов" }
			};
			static IOrderedEnumerable<CategoryEntity> orderFunc
				(IEnumerable<CategoryEntity> src) => src.OrderBy(x => x.Order).ThenBy(x => x.Title);
			var helperFull = new TreeEntityHelper<CategoryEntity>(categories, null, orderFunc);
			SuppConsole.WriteLineParam(
				"Корневые элементы полного дерева",
				SuppConsole.ArrToText(helperFull.TopItems.Select(x => $"{x.Value?.Title} (Id: {x.Id})")));
			SuppConsole.WriteLineParam(
				"Все элементы полного дерева в порядке обхода (с учетом уровней)",
				SuppConsole.ArrToText(helperFull.AllItems.Select(x => $"{new string('-', x.Level * 2)} {x.Value?.Title}")));
			var helperFiltered = new TreeEntityHelper<CategoryEntity>(categories, 2, orderFunc);
			SuppConsole.WriteLineParam(
				"Плоский список дерева после исключения Id 2 (и его поддерева)",
				SuppConsole.ArrToText(helperFiltered.AllItems.Select(x => x.Value?.Title)));



			SuppConsole.PartStart("WebApiHelper");

			using var httpClient = new HttpClient();
			var appCache = new MemoryCache(new MemoryCacheOptions());
			var restClient = new WebApiHelper<UserRecord>(
				httpClient, _ApiUrl, appCache, jsonTypeInfo: MyContext.Default.UserRecord);
			var newPlayer = new UserRecord(0, "Cyber_King_10");
			var postResult = await restClient.SendPostAsync("", newPlayer);
			SuppConsole.WriteLineParam(
				"Выполнение POST-запроса через чистый Source Gen",
				postResult.StatusCode);



			SuppConsole.PartStart("WidthWrapper");

			var mediumWrapper = new WidthWrapper(
				html: "<input type=\"text\" />",
				width: WidthsEnum.Medium,
				cssClass: "form-group");
			SuppConsole.WriteLineParam("HTML обертка среднего размера", mediumWrapper.ToString());
			var tinyWrapper = new WidthWrapper(
				html: "<span>Pin</span>",
				width: WidthsEnum.ExtraSmall,
				cssClass: "badge");
			SuppConsole.WriteLineParam("HTML обертка минимального размера", tinyWrapper.ToString());
			var fluidWrapper = new WidthWrapper(
				html: "<p>Свободный текст</p>",
				width: WidthsEnum.Nothing,
				cssClass: "content-fluid");
			SuppConsole.WriteLineParam("HTML обертка без указания стиля ширины", fluidWrapper.ToString());
			var nullSafeWrapper = new WidthWrapper(null, WidthsEnum.Full, null);
			SuppConsole.WriteLineParam("HTML обертка при передаче null значений", nullSafeWrapper.ToString());



			SuppConsole.PartStart("Crud._CrudMasterRepository_Proto");
			SuppConsole.PartStart("Crud._CrudSlaveRepository_Proto");



			SuppConsole.PartStart("Json.AutoNumberToStringConverter");
			SuppConsole.PartStart("Json.AutoStringToNumberConverterFactory");
			SuppConsole.PartStart("Json.BoolConverter");
			SuppConsole.PartStart("Json.IntToStringConverter");

			string jsonInput = """
				{
				"StringProp": 12345,
				"IntProp": "99",
				"DoubleProp": "45.67",
				"BoolProp1": "true",
				"BoolProp2": 1,
				"IntStringProp": "777"
				}
				""";
			var model1 = JsonSerializer.Deserialize<TestModel>(jsonInput);
			SuppConsole.WriteLineParam(
				"AutoNumberToStringConverter (Число 12345 в строковое поле)",
				model1?.StringProp ?? "null");
			SuppConsole.WriteLineParam(
				"AutoStringToNumberConverterFactory (Строка \"99\" в int)",
				model1?.IntProp ?? 0);
			SuppConsole.WriteLineParam(
				"AutoStringToNumberConverterFactory (Строка \"45.67\" в double)",
				model1?.DoubleProp ?? 0);
			SuppConsole.WriteLineParam(
				"BoolConverter (Строка \"true\" в bool)",
				model1?.BoolProp1 ?? false);
			SuppConsole.WriteLineParam(
				"BoolConverter (Число 1 в bool)",
				model1?.BoolProp2 ?? false);
			SuppConsole.WriteLineParam(
				"IntToStringConverter (Строка \"777\" в int)",
				model1?.IntStringProp ?? 0);
			var model2 = new TestModel
			{
				StringProp = "Привет",
				IntProp = 500,
				DoubleProp = 12.34,
				BoolProp1 = true,
				BoolProp2 = false,
				IntStringProp = 888
			};
			string jsonOutput = JsonSerializer.Serialize(model2, SuppJson.DEFAULT_JSON_SERIALIZER_OPTIONS);
			SuppConsole.WriteLineParam(
				"Результат сериализации модели с конвертерами",
				$"\n{jsonOutput}");

		}


		static void _outServices()
		{
			SuppConsole.SectionStart("SERVICES");



			SuppConsole.PartStart("AnsMailerService");

			var devMailer = new FakeMailerService();
			var email1 = new MailMessageModel { Subject = "Тест-заглушка" };
			devMailer.SendAsync(email1);
			SuppConsole.WriteLineParam("Выполнение FakeMailerService", "Успешно пройдено (без исключений)");

			//var options = new TestMailerOptions();
			//var productionMailer = new AnsMailerService(options);
			//var email2 = new MailMessageModel
			//{
			//	To = AnsMailerService.GetMailboxAddress("Клиент Техподдержки", "krv@guap.ru"),
			//	Subject = "Восстановление доступа",
			//	ContentHtml = "<h2>Ваш новый пароль сгенерирован службой SuppCrypto.</h2>"
			//};
			//productionMailer.SendAsync(email2);

			var addressInfo = AnsMailerService.GetMailboxAddress("Roman Koff", "krv@guap.ru");
			SuppConsole.WriteLineParam(
				"Сборка адреса через хелпер сервиса",
				$"{addressInfo.Name} <{addressInfo.Address}>");

		}


		static void _writeDateSpan(
			DateTime d1,
			DateTime d2)
		{
			Console.WriteLine($"{d1} + {d2} : {SuppDateTime.GetSpan(d1, d2, false)}");
			Console.WriteLine($"{d1} + {d2} : {SuppDateTime.GetSpan(d1, d2, true)}");
		}

	}









	enum UserRole
	{
		[Description("Администратор системы")]
		Admin,
		[Description("Модератор контента")]
		Moderator,
		Guest
	}



	class Client
	{
		public required string Name { get; set; }
		public int Age { get; set; }
	}



	class SampleItem
	{
		public string Title { get; set; } = "Книга";
		public int? Qty { get; set; }

		public static void MyGenericMethod<T>(
			params int[] numbers)
		{
			Console.WriteLine(numbers);
		}
	}



	record struct ProductModel(int Id, string Name, decimal Price);



	class LogEntry
	{
		public int Id { get; set; }
		public string Message { get; set; } = string.Empty;
	}



	class Account
	{
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
		public Account(string username, int initialBalance) { }
		public int Id { get; set; }
		public string Username { get; private set; } = "RomanKoff";
		public string Password { set { } } // Write-only
		public void Deposit(int amount) { } // Void метод
		public bool TryWithdraw<T>(int amount, out T receipt) where T : class { receipt = default!; return true; }
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter
	}



	class UserEntity
	{
		public int Id { get; set; }
		public string SecretHash { get; set; } = string.Empty; // Секретные данные СУБД
		public string FullName { get; set; } = string.Empty;
	}



	class UserViewModel
	{
		public int Id { get; set; }
		public string DisplayName { get; set; } = string.Empty;
	}



	class UserListPagedModel
		: _ListPaginatedModel_Base<UserEntity, UserViewModel>
	{
		public UserListPagedModel(
			IQueryable<UserEntity> query,
			int page,
			int itemsOnPage)
			: base(
				query,
				u => new UserViewModel { Id = u.Id, DisplayName = u.FullName }, // Правило проекции
				page,
				itemsOnPage)
		{
		}
	}



	class TestModel
	{
		[JsonConverter(typeof(AutoNumberToStringConverter))]
		public string StringProp { get; set; } = string.Empty;

		[JsonConverter(typeof(AutoStringToNumberConverterFactory))]
		public int IntProp { get; set; }

		[JsonConverter(typeof(AutoStringToNumberConverterFactory))]
		public double DoubleProp { get; set; }

		[JsonConverter(typeof(BoolConverter))]
		public bool BoolProp1 { get; set; }

		[JsonConverter(typeof(BoolConverter))]
		public bool BoolProp2 { get; set; }

		[JsonConverter(typeof(IntToStringConverter))]
		public int IntStringProp { get; set; }
	}



	class MenuItem(
		string title,
		string url)
		: _TreeItem_Base
	{
		public string Title { get; } = title;
		public string Url { get; } = url;
	}



	class CategoryEntity
		: ITreeEntity
	{
		public int Id { get; set; }
		public int? ParentPtr { get; set; }
		public int Order { get; set; }
		public string Title { get; set; } = string.Empty;
	}



	class OrderItemEntity
		: ISlaveEntity
	{
		public int Id { get; set; }
		public int MasterPtr { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public decimal Price { get; set; }
	}



	class OrderItemRepository
		: _CrudSlaveRepository_Proto<OrderItemEntity>
	{
		public OrderItemRepository(DbContext db) : base(db) { }

		public override OrderItemEntity GetNew(int masterPtr)
		{
			return new OrderItemEntity
			{
				Id = 0,
				MasterPtr = masterPtr,
				ProductName = "Новый товар",
				Price = 0.00m
			};
		}
	}



	record TestUser(int Id, string Name, bool IsActive, int Age);



	public class CatalogItem
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public List<string> Tags { get; set; } = [];
	}



	public class UserDto
	{
		public int Id { get; set; }
		public string Login { get; set; } = string.Empty;
		public string[] Roles { get; set; } = [];
	}
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
	[JsonSerializable(typeof(UserDto))]
	public partial class MyProjectJsonContext : JsonSerializerContext { }



	record EmployeeMetric(
		int Id,
		string Name,
		double Rating,
		DateTime? HiredDate,
		bool IsActive);



	record class TodoItem(int Id, string Title, bool Completed);



	record class TodoMock(int Id, string Title);



	record class RaspModel(string Years, DateTime DateRelease);



	record class EmployeeDto(int Id, string Name, string Role);



	record class UserRecord(int Id, string Nickname);


	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
	[JsonSerializable(typeof(UserRecord))]
	internal partial class MyContext : JsonSerializerContext { }

}
