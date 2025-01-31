using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Shared.Components.Layout
{
    public partial class NavMenu : IAsyncDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetNotificationsCountRequestDto, GetNotificationsCountResponseDto> _repoGetNotificationsCount { get; set; } = null!;
        [Inject] IRepository<GetMessagesCountRequestDto, GetMessagesCountResponseDto> _repoGetMessagesCount { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;

        IDisposable? OnUpdateMessagesCountHandler;
        IDisposable? OnUpdateNotificationsCountHandler;

        DotNetObjectReference<NavMenu> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        int unreadNotificationsCount;
        int unreadMessagesCount;

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
                // Оставить здесь, т.к. требуется как аутентификация пользователя, так и подключение по SignalR.
                OnUpdateMessagesCountHandler = OnUpdateMessagesCountHandler.SignalRClient<OnUpdateMessagesCountResponse>(CurrentState, async (response) =>
                    await GetNewItemsCount());
                OnUpdateNotificationsCountHandler = OnUpdateNotificationsCountHandler.SignalRClient<OnUpdateNotificationsCountResponse>(CurrentState, async (response) =>
                    await GetNewItemsCount());

                await GetNewItemsCount();
            }
        }


        [JSInvokable]
        public async Task<int> GetNotificationsCountAsync()
        {
            var notificationsCountResponse = await _repoGetNotificationsCount.HttpPostAsync(new GetNotificationsCountRequestDto() { Token = CurrentState.Account?.Token });
            unreadNotificationsCount = notificationsCountResponse.Response.UnreadCount;
            return unreadNotificationsCount;
        }

        [JSInvokable]
        public async Task<int> GetMessagesCountAsync()
        {
            var messagesCountResponse = await _repoGetMessagesCount.HttpPostAsync(new GetMessagesCountRequestDto() { Token = CurrentState.Account?.Token });
            unreadMessagesCount = messagesCountResponse.Response.UnreadCount;
            return unreadMessagesCount;
        }


        async Task GetNewItemsCount()
        {
            await _JSModule.InvokeVoidAsync("GetNotificationsCount");
            await _JSModule.InvokeVoidAsync("GetMessagesCount");
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                OnUpdateMessagesCountHandler?.Dispose();
                OnUpdateNotificationsCountHandler?.Dispose();
                if (_JSModule != null)
                    await _JSModule.DisposeAsync();
                _dotNetReference?.Dispose();
            }
            // Отлов ошибки, когда соединение SignalR разорвано, но производится попытка вызвать JS. Возникает при перезагрузке страницы (F5)
            catch (JSDisconnectedException ex)
            {
            }
        }
    }
}
