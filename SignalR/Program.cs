using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;
using SignalR;
using SignalR.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Seq("http://localhost:5341", restrictedToMinimumLevel: LogEventLevel.Verbose)
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug));

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opt => { opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }); });
builder.Services.AddSingleton<Accounts>();

builder.Services.AddJwtToken(builder);

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalRHub>("/signalrhub");
});

app.Run();
