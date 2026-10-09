using Ans.Net10.Web;



/*--- builder ---*/

var builder = WebApplication.CreateBuilder(args);

builder.Add_AnsNet10Web();



/*--- app ---*/

var app = builder.Build();

await app.Use_AnsNet10WebAsync();

await app.RunAsync();
