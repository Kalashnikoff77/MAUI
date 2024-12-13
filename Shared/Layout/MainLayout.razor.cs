using Microsoft.AspNetCore.Components;
using Shared.State;

namespace Shared.Layout
{
    public partial class MainLayout
    {
        [Inject] CurrentState CurrentState { get; set; } = null!;
    }
}
