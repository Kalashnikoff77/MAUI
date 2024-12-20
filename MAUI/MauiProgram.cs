using Blazored.LocalStorage;
using Data.Services;
using MAUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Data.State;
using System.Reflection;
using Shared.Components.Dialogs;

namespace MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            builder.Services.AddScoped<IJSProcessor, JSProcessor>();
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddScoped<CurrentState>();
            builder.Services.AddScoped<ShowDialogs>();

            builder.Services.AddMudServices();
           
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var a = Assembly.GetExecutingAssembly();
            var appSettings = $"{a.GetName().Name}.appsettings.json";
            using var stream = a.GetManifestResourceStream(appSettings);
            if (stream == null)
                throw new Exception("Не найден файл конфигурации appsettings.json!");
            builder.Configuration.AddConfiguration(new ConfigurationBuilder().AddJsonStream(stream).Build());

            return builder.Build();
        }
    }
}
