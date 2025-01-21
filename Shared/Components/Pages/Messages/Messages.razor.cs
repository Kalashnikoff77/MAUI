using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Messages
{
    public partial class Messages : IDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetLastMessagesListRequestDto, GetLastMessagesListResponseDto> _repoGetLastMessagesList { get; set; } = null!;
        [Inject] IRepository<MarkMessageAsReadRequestDto, MarkMessageAsReadResponseDto> _markMessageAsRead { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;
        List<LastMessagesForAccountSpDto> LastMessagesList = new List<LastMessagesForAccountSpDto>();

        protected override async Task OnParametersSetAsync()
        {
            if (CurrentState.Account != null)
            {
                var request = new GetLastMessagesListRequestDto() { Token = CurrentState.Account.Token };
                var apiResponse = await _repoGetLastMessagesList.HttpPostAsync(request);
                LastMessagesList = apiResponse.Response.LastMessagesList ?? new List<LastMessagesForAccountSpDto>();
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
                {
                    var request = new GetLastMessagesListRequestDto() { Token = CurrentState.Account?.Token };
                    var apiResponse = await _repoGetLastMessagesList.HttpPostAsync(request);
                    LastMessagesList = apiResponse.Response.LastMessagesList ?? new List<LastMessagesForAccountSpDto>();
                    await InvokeAsync(StateHasChanged);
                });
            }
        }

        async Task MarkAsReadCallbackAsync(int markAsReadMessageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == markAsReadMessageId);
            // Проверим, помечено ли сообщение как прочитанное и адресовано ли нам?
            if (index >= 0 && LastMessagesList[index].Recipient?.Id == CurrentState.Account?.Id && LastMessagesList[index].ReadDate == null)
            {
                var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = false, Token = CurrentState.Account?.Token });
                if (apiResponse.Response.UpdatedMessage != null)
                    LastMessagesList[index] = apiResponse.Response.UpdatedMessage;
            }
        }

        async Task MarkAllAsReadAsync(int markAsReadMessageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == markAsReadMessageId);
            if (index >= 0 && LastMessagesList[index].Sender != null)
            {
                var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });
                if (apiResponse.Response.UpdatedMessage != null)
                    LastMessagesList[index] = apiResponse.Response.UpdatedMessage;
            }
        }

        async Task BlockAccountAsync()
        {
        }

        async Task OnSearch(string text)
        {
        }

        public void Dispose() =>
            OnMessagesReloadHandler?.Dispose();
    }
}
