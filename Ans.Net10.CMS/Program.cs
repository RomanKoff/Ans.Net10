using Ans.Net10.Web;



/*--- builder ---*/

var builder = WebApplication.CreateBuilder(args);
builder.Add_AnsNet10Web();



/*--- app ---*/

var app = builder.Build();

await app.Use_AnsNet10WebAsync(sitemap =>
{
	sitemap.AddTopNodeDef(
		new NodemapDef("_system", "Системный модуль", topPages: [
			new PagemapDef("tests", "Тестирование приложения", true, childPages: [
				new PagemapDef("pages", "Проверка страниц")
			]),
		]));
});

await app.RunAsync();
