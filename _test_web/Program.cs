using Ans.Net10.Common.Services;
using Ans.Net10.Web;
using Ans.Net10.Web.Services;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

var webOptions = AppSettingsFactory.GetOptions<LibWebOptions>(builder.Configuration);

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
	SuppForwardedHeaders.Configure(o, webOptions);
});

builder.Services.AddMvc();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHybridCache();
builder.Services.AddHttpClient();

if (webOptions.MailService == null)
	builder.Services.AddSingleton<IMailerService, FakeMailerService>();
else
	builder.Services.AddSingleton<IMailerService, AnsMailerService>(
		_ => new AnsMailerService(webOptions.MailService));

builder.Services.AddScoped<IViewRenderService, AnsViewRenderService>();
builder.Services.AddScoped<CurrentContext>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapControllers();

app.Run();
