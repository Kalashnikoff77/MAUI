using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace Shared.Components.Layout
{
    public partial class NavMenu
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetNotificationsCountRequestDto, GetNotificationsCountResponseDto> _repoGetNotificationsCount { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;

        DotNetObjectReference<NavMenu> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        int unreadNotificationsCount;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetReference = DotNetObjectReference.Create(this);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", $"{CurrentState.WebUrl}/js/Layout/NavMenu.js");
                await _JSModule.InvokeVoidAsync("Initialize", _dotNetReference);
            }
            else
            {
                await _JSModule.InvokeVoidAsync("GetNotificationsCount");
            }
        }

        [JSInvokable]
        public async Task<int> GetNotificationsCountAsync()
        {
            var notificationsCountResponse = await _repoGetNotificationsCount.HttpPostAsync(new GetNotificationsCountRequestDto() { Token = CurrentState.Account?.Token });
            unreadNotificationsCount = notificationsCountResponse.Response.UnreadCount;
            return unreadNotificationsCount;
        }

    }
}
