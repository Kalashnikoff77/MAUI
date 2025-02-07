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
        [Inject] IRepository<GetMessagesCountRequestDto, GetMessagesCountResponseDto> _repoGetMessagesCount { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;

        IDisposable? OnMessagesUpdatedHandler;

        DotNetObjectReference<NavMenu> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

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
                if (CurrentState.Account != null)
                {
                    OnMessagesUpdatedHandler = OnMessagesUpdatedHandler.SignalRClient<OnMessagesUpdatedResponse>(CurrentState, async (response) =>
                        await _JSModule.InvokeVoidAsync("GetMessagesCount"));

                    await _JSModule.InvokeVoidAsync("GetMessagesCount");
                }
            }
        }


        [JSInvokable]
        public async Task<int> GetMessagesCountAsync()
        {
            var messagesCountResponse = await _repoGetMessagesCount.HttpPostAsync(new GetMessagesCountRequestDto() { Token = CurrentState.Account?.Token });
            unreadMessagesCount = messagesCountResponse.Response.UnreadCount;
            return unreadMessagesCount;
        }


        public async ValueTask DisposeAsync()
        {
            try
            {
                OnMessagesUpdatedHandler?.Dispose();

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
