using Common.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using SH.Services;
using SH.Shared.Services;
using System.Reflection;

namespace SH
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

            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            builder.Services.AddMudServices();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            //var a = Assembly.GetExecutingAssembly();
            //using var stream = a.GetManifestResourceStream("appsettings.json");

            //var config = new ConfigurationBuilder()
            //            .AddJsonStream(stream)
            //            .Build();
            //builder.Configuration.AddConfiguration(config);

            return builder.Build();
        }
    }

    public class Settings
    {
        public int WebAPIHost { get; set; }
    }
}
