using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using MudBlazor.Services;
using SH.Web.Services;
using Shared.Components.Dialogs;
using Web.Components;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel((context, options) =>
//{
//    options.Listen(System.Net.IPAddress.Any, 7443, options => options.UseHttps(@"C:\Projects\Projects\MAUI\More\certificate.pfx", "Oleg184977"));
//    options.Listen(System.Net.IPAddress.Any, 780);
//});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDataProtection()
    .SetApplicationName("SwingHouse.Ru")
    .UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IComponentRenderer<>), typeof(ComponentRenderer<>));
builder.Services.AddScoped<IJSProcessor, JSProcessor>();
builder.Services.AddScoped<IFormFactor, FormFactor>();
builder.Services.AddScoped<ShowDialogs>();

builder.Services.AddScoped<CurrentState>();

builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Shared._Imports).Assembly);

app.Run();
