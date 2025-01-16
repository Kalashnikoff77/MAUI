using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Data.Services
{
    public class ComponentRenderer<T> : IComponentRenderer<T> where T : IComponent
    {
        IServiceProvider _serviceProvider { get; set; } = null!;

        public ComponentRenderer(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public async Task<string> RenderAsync(Dictionary<string, object?> param)
        {
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());
            var html = new StringBuilder(5000);

            var result = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await htmlRenderer.RenderComponentAsync<T>(ParameterView.FromDictionary(param));
                return output.ToHtmlString();
            });

            return result.ToString();
        }
    }
}
