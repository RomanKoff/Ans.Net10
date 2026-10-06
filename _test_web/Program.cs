using Ans.Net10.Web;
using Serilog;
using Serilog.Events;



var builder = WebApplication.CreateBuilder(args);

string appName1 = builder.Environment.ApplicationName.ToLower();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
	loggerConfiguration
		.MinimumLevel.Debug() // .MinimumLevel.Warning()
		.Enrich.FromLogContext()
		.WriteTo.Console()
		.WriteTo.Logger(lc => lc
			.Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Warning)
			.WriteTo.Map(
				keySelector: x => x.Timestamp.DateTime,
				configure: (dateTime1, wt1) => wt1.File(
					path: $"logs/{appName1}/{dateTime1:yyyy-MM}/warn_{dateTime1:dd-HH}.txt",
					rollingInterval: RollingInterval.Infinite,
					retainedFileCountLimit: null,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{Exception}"
				)))
		.WriteTo.Logger(lc => lc
			.Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Error)
			.WriteTo.Map(
				keySelector: x => x.Timestamp.DateTime,
				configure: (dateTime1, wt1) => wt1.File(
					path: $"logs/{appName1}/{dateTime1:yyyy-MM}/err_{dateTime1:dd-HH}.txt",
					rollingInterval: RollingInterval.Infinite,
					retainedFileCountLimit: null,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{Exception}"
				)))
		.WriteTo.Logger(lc => lc
			.MinimumLevel.Fatal()
			.WriteTo.Map(
				keySelector: x => x.Timestamp.DateTime,
				configure: (dateTime1, wt1) => wt1.File(
					path: $"logs/{appName1}/{dateTime1:yyyy-MM}/fatal_{dateTime1:dd-HH}.txt",
					rollingInterval: RollingInterval.Infinite,
					retainedFileCountLimit: null,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{Exception}"
				)));
});

builder.Add_AnsNet10Web();



var app = builder.Build();

app.Use_AnsNet10Web();

app.Run();
