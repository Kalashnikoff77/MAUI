using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Messages
{
    public partial class Messages : IDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetLastMessagesListRequestDto, GetLastMessagesListResponseDto> _repoGetLastMessagesList { get; set; } = null!;
        [Inject] IRepository<MarkMessageAsReadRequestDto, ResponseDtoBase> _markMessageAsRead { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        IDisposable? OnUpdateLastMessagesHandler;
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
                OnUpdateLastMessagesHandler = OnUpdateLastMessagesHandler.SignalRClient<OnUpdateLastMessagesResponse>(CurrentState, async (response) =>
                {
                    var request = new GetLastMessagesListRequestDto() { Token = CurrentState.Account?.Token };
                    var apiResponse = await _repoGetLastMessagesList.HttpPostAsync(request);
                    LastMessagesList = apiResponse.Response.LastMessagesList ?? new List<LastMessagesForAccountSpDto>();
                    await InvokeAsync(StateHasChanged);
                });
            }
        }

        async Task MarkAsReadAsync(int markAsReadMessageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == markAsReadMessageId);
            // Проверим, помечено ли сообщение как прочитанное и адресовано ли нам?
            if (index >= 0 && LastMessagesList[index].Recipient?.Id == CurrentState.Account?.Id && LastMessagesList[index].Sender != null && LastMessagesList[index].ReadDate == null)
            {
                var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = false, Token = CurrentState.Account?.Token });

                // Обновим список последних сообщений на странице /messages
                var lastMessagesRequest = new SignalGlobalRequest { OnUpdateLastMessages = new OnUpdateLastMessages { RecipientId = LastMessagesList[index].Sender!.Id } };
                await CurrentState.SignalRServerAsync(lastMessagesRequest);

                // Пометим сообщения как прочитанные в MessageDialog
                var messagesIds = new List<int> { LastMessagesList[index].Id };
                var markMessagesAsReadRequest = new SignalGlobalRequest { OnMarkMessagesAsRead = new OnMarkMessagesAsRead { RecipientId = LastMessagesList[index].Sender!.Id, MessagesIds = messagesIds } };
                await CurrentState.SignalRServerAsync(markMessagesAsReadRequest);
            }
        }

        async Task MarkAllAsReadAsync(int markAsReadMessageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == markAsReadMessageId);
            if (index >= 0 && LastMessagesList[index].Sender != null)
            {
                var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });

                // Обновим список последних сообщений на странице /messages
                var lastMessagesRequest = new SignalGlobalRequest { OnUpdateLastMessages = new OnUpdateLastMessages { RecipientId = LastMessagesList[index].Sender!.Id } };
                await CurrentState.SignalRServerAsync(lastMessagesRequest);

                // Пометим сообщения как прочитанные в MessageDialog
                var markMessagesAsReadRequest = new SignalGlobalRequest { OnMarkMessagesAsRead = new OnMarkMessagesAsRead { RecipientId = LastMessagesList[index].Sender!.Id, MessagesIds = null } };
                await CurrentState.SignalRServerAsync(markMessagesAsReadRequest);
            }
        }

        async Task BlockAccountAsync()
        {
        }

        async Task OnSearch(string text)
        {
        }

        public void Dispose() =>
            OnUpdateLastMessagesHandler?.Dispose();
    }
}
