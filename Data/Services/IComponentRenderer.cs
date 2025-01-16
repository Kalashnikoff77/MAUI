using Microsoft.AspNetCore.Components;

namespace Data.Services
{
    public interface IComponentRenderer<T> where T : IComponent
    {
        Task<string> RenderAsync(Dictionary<string, object?> param);
    }
}
