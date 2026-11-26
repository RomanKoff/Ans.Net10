using Ans.Net10.Web;
using Ans.Net10.Web.Api;



/*--- builder ---*/

var builder = WebApplication.CreateBuilder(args);
builder.Add_AnsNet10Web();



/*--- app ---*/

var app = builder.Build();
app.AnsApi_Console("/_system/console");
app.AnsApi_Compile("/_system/compilation");

await app.Use_AnsNet10WebAsync();
await app.RunAsync();
