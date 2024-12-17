using Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using SignalR;
using SignalR.Models;
using SignalR.Services;
using System.Text;

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
builder.Services.AddScoped<IFormFactor, FormFactor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // указывает, будет ли валидироваться издатель при валидации токена
            ValidIssuer = builder.Configuration.GetRequiredSection("JWT:JwtValidIssuer").Value, // строка, представляющая издателя
            ValidateAudience = true, // будет ли валидироваться потребитель токена
            ValidAudience = builder.Configuration.GetRequiredSection("JWT:JwtValidAudience").Value, // установка потребителя токена
            ValidateLifetime = true, // будет ли валидироваться время существования
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetRequiredSection("JWT:IssuerSigningKey").Value!)), // установка ключа безопасности
            ValidateIssuerSigningKey = true // валидация ключа безопасности
        };
    });

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
