using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace Shared.Components.Pages.Messages
{
    public partial class Messages : IAsyncDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetLastMessagesListRequestDto, GetLastMessagesListResponseDto> _repoGetLastMessagesList { get; set; } = null!;
        [Inject] IRepository<MarkMessageAsReadRequestDto, MarkMessageAsReadResponseDto> _markMessageAsRead { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<OneMessage> _renderer { get; set; } = null!;

        GetLastMessagesListRequestDto _request = new GetLastMessagesListRequestDto { Take = StaticData.LAST_MESSAGES_PER_BLOCK };
        List<LastMessagesForAccountSpDto> lastMessagesList = new List<LastMessagesForAccountSpDto>();

        DotNetObjectReference<Messages> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (CurrentState.IsAccountLoggedIn)
            {
                var cs = CurrentState;
                _dotNetReference = DotNetObjectReference.Create(this);
                await _JSProcessor.SetDotNetReference(_dotNetReference);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", $"{CurrentState.WebUrl}/js/Pages/Messages/MessagesScroll.js");
                await ReloadItemsAsync();

                OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
                {
                    _request = new GetLastMessagesListRequestDto() { Token = CurrentState.Account?.Token };
                    var apiResponse = await _repoGetLastMessagesList.HttpPostAsync(_request);
                    lastMessagesList = apiResponse.Response.LastMessagesList ?? new List<LastMessagesForAccountSpDto>();
                    await InvokeAsync(StateHasChanged);
                });
            }
        }


        [JSInvokable]
        public async Task<string> LoadItems()
        {
            var request = new GetLastMessagesListRequestDto() { Token = CurrentState.Account?.Token };
            var apiResponse = await _repoGetLastMessagesList.HttpPostAsync(request);
            request.Skip = request.Skip + StaticData.LAST_MESSAGES_PER_BLOCK;

            var htmlItems = new StringBuilder(5000);
            if (apiResponse.Response.LastMessagesList != null)
            {
                lastMessagesList.AddRange(apiResponse.Response.LastMessagesList);

                // Ручная генерация компонентов мероприятий
                var markAsReadCallback = EventCallback.Factory.Create<int>(this, MarkAsReadCallbackAsync);
                var markAllCallback = EventCallback.Factory.Create<int>(this, MarkAllAsReadAsync);
                var blockCallback = EventCallback.Factory.Create(this, BlockAccountAsync);
                foreach (var message in apiResponse.Response.LastMessagesList)
                {
                    htmlItems.Append(await _renderer.RenderAsync(new Dictionary<string, object?> {
                        { "CurrentState", CurrentState },
                        { "Message", message },
                        { "MarkAsReadCallbackAsync", markAsReadCallback },
                        { "MarkAllAsReadAsync", markAllCallback },
                        { "BlockAccountAsync", blockCallback }
                    }));
                }
            }
            return htmlItems.ToString();
        }

        async Task ReloadItemsAsync()
        {
            _request.Skip = 0;
            lastMessagesList.Clear();
            await _JSModule.InvokeVoidAsync("ClearItems");
            await _JSModule.InvokeVoidAsync("LoadItems");
        }

        async Task MarkAsReadCallbackAsync(int markAsReadMessageId)
        {
            var index = lastMessagesList.FindIndex(x => x.Id == markAsReadMessageId);
            // Проверим, помечено ли сообщение как прочитанное и адресовано ли нам?
            if (index >= 0 && lastMessagesList[index].Recipient?.Id == CurrentState.Account?.Id && lastMessagesList[index].ReadDate == null)
            {
                var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = false, Token = CurrentState.Account?.Token });
                if (apiResponse.Response.UpdatedMessage != null)
                    lastMessagesList[index] = apiResponse.Response.UpdatedMessage;
            }
        }

        async Task MarkAllAsReadAsync(int markAsReadMessageId)
        {
            var index = lastMessagesList.FindIndex(x => x.Id == markAsReadMessageId);
            if (index >= 0 && lastMessagesList[index].Sender != null)
            {
                var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });
                if (apiResponse.Response.UpdatedMessage != null)
                    lastMessagesList[index] = apiResponse.Response.UpdatedMessage;
            }
        }

        async Task BlockAccountAsync()
        {
        }

        async Task OnSearch(string text)
        {
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                OnMessagesReloadHandler?.Dispose();
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
