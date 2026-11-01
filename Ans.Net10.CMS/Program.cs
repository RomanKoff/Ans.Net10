using Ans.Net10.Web;



/*--- builder ---*/

var builder = WebApplication.CreateBuilder(args);
builder.Add_AnsNet10Web();



/*--- app ---*/

var app = builder.Build();

await app.Use_AnsNet10WebAsync();
//await app.Use_AnsNet10WebAsync(sitemap =>
//{
//	sitemap.AddTopNodeDef(
//		new NodemapItemDef("_system", "Системный модуль", topPages: [
//			new PagemapItemDef("tests", "Тестирование приложения", true, childPages: [
//				new PagemapItemDef("pages", "Проверка страниц")
//			]),
//		]));
//});

await app.RunAsync();
