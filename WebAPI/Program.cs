using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.Filters;
using WebAPI.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // указывает, будет ли валидироватьс€ издатель при валидации токена
            ValidIssuer = builder.Configuration.GetValue<string>("JwtValidIssuer"), // строка, представл€юща€ издател€
            ValidateAudience = true, // будет ли валидироватьс€ потребитель токена
            ValidAudience = builder.Configuration.GetValue<string>("JwtValidAudience"), // установка потребител€ токена
            ValidateLifetime = true, // будет ли валидироватьс€ врем€ существовани€
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("IssuerSigningKey")!)), // установка ключа безопасности
            ValidateIssuerSigningKey = true // валидаци€ ключа безопасности
        };
    });

builder.Services.AddAutoMapper((cfg) => 
{
    cfg.AllowNullCollections = true;
}, typeof(Mapping));

// ќтключение автоматической валидации вход€щих параметров в WebAPI. јвтовалидаци€ возвращает немеделнный ответ 400 - Bad Request.
builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

builder.Services.AddMvc(options =>
{
    //options.Filters.Add(typeof(ApiValidateModelAttribute));
    options.Filters.Add<CustomExceptionFilter>();
});

// ћаксимальный размер загружаемых файлов = 35 ћб.
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 35 * 1024 * 1024;
});

builder.Services.AddMemoryCache();

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
           .WithOrigins("https://localhost:7000");
}));

//builder.Services.AddDbContext<SwContext>(options =>
//{
//    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
//});

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("CorsPolicy");


app.MapControllers();

app.Run();
