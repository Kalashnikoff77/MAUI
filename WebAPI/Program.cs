using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.Filters;
using WebAPI.Mapping;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(System.Net.IPAddress.Any, 7011, options => options.UseHttps(@"C:\Projects\Projects\MAUI\More\certificate.pfx", "Oleg184977"));
    options.Listen(System.Net.IPAddress.Any, 7010);
});

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
            ValidateIssuer = true, // ���������, ����� �� �������������� �������� ��� ��������� ������
            ValidIssuer = builder.Configuration.GetValue<string>("JwtValidIssuer"), // ������, �������������� ��������
            ValidateAudience = true, // ����� �� �������������� ����������� ������
            ValidAudience = builder.Configuration.GetValue<string>("JwtValidAudience"), // ��������� ����������� ������
            ValidateLifetime = true, // ����� �� �������������� ����� �������������
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("IssuerSigningKey")!)), // ��������� ����� ������������
            ValidateIssuerSigningKey = true // ��������� ����� ������������
        };
    });

builder.Services.AddAutoMapper((cfg) => 
{
    cfg.AllowNullCollections = true;
}, typeof(Mapping));

// ���������� �������������� ��������� �������� ���������� � WebAPI. ������������� ���������� ����������� ����� 400 - Bad Request.
builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

builder.Services.AddMvc(options =>
{
    //options.Filters.Add(typeof(ApiValidateModelAttribute));
    options.Filters.Add<CustomExceptionFilter>();
});

// ������������ ������ ����������� ������ = 35 ��.
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
           .WithOrigins("https://swinghouse.ru:7000");
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
