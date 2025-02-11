using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Dto.Views;
using Data.Enums;
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
        [Inject] IRepository<MarkMessagesAsReadRequestDto, ResponseDtoBase> _markMessagesAsRead { get; set; } = null!;
        [Inject] IRepository<DeleteMessagesRequestDto, ResponseDtoBase> _deleteMessages { get; set; } = null!;
        [Inject] IRepository<UpdateRelationRequestDto, ResponseDtoBase> _repoUpdateRelation { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        IDisposable? OnMessagesUpdatedHandler;
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
                OnMessagesUpdatedHandler = OnMessagesUpdatedHandler.SignalRClient<OnMessagesUpdatedResponse>(CurrentState, async (response) =>
                {
                    var request = new GetLastMessagesListRequestDto() { Token = CurrentState.Account?.Token };
                    var apiResponse = await _repoGetLastMessagesList.HttpPostAsync(request);
                    LastMessagesList = apiResponse.Response.LastMessagesList ?? new List<LastMessagesForAccountSpDto>();
                    await InvokeAsync(StateHasChanged);
                });
            }
        }

        async Task MarkAsReadAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);
            // Проверим, помечено ли сообщение как прочитанное и адресовано ли нам?
            if (index >= 0 && LastMessagesList[index].Recipient?.Id == CurrentState.Account?.Id && LastMessagesList[index].Sender != null && LastMessagesList[index].ReadDate == null)
            {
                // Помечаем сообщение как прочитанное в БД
                var apiResponse = await _markMessagesAsRead.HttpPostAsync(new MarkMessagesAsReadRequestDto { MessageId = messageId, MarkAllAsRead = false, Token = CurrentState.Account?.Token });

                var onMessagesUpdatedRequest = new SignalGlobalRequest 
                { 
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest 
                    { 
                        MarkMessagesAsRead = true, 
                        RecipientId = LastMessagesList[index].Sender!.Id, 
                        MessagesIds = new List<int> { LastMessagesList[index].Id }
                    } 
                };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }
        }

        async Task MarkAllAsReadAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);
            if (index >= 0 && LastMessagesList[index].Sender != null)
            {
                var apiResponse = await _markMessagesAsRead.HttpPostAsync(new MarkMessagesAsReadRequestDto { MessageId = messageId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });

                var onMessagesUpdatedRequest = new SignalGlobalRequest
                {
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                    {
                        MarkMessagesAsRead = true,
                        RecipientId = LastMessagesList[index].Sender!.Id,
                        MessagesIds = null
                    }
                };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }
        }

        async Task BlockAccountAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);
            if (index >= 0 && LastMessagesList[index].Sender != null)
            {
                int blockingUserId = LastMessagesList[index].Sender!.Id == CurrentState.Account!.Id ? LastMessagesList[index].Recipient!.Id : LastMessagesList[index].Sender!.Id;

                var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto
                {
                    RecipientId = blockingUserId,
                    EnumRelation = EnumRelations.Blocked,
                    Token = CurrentState.Account?.Token
                });

                // Обновим состояния у обоих пользователей
                var accountReloadRequest = new SignalGlobalRequest { OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = blockingUserId } };
                await CurrentState.SignalRServerAsync(accountReloadRequest);

                // Обновим список последних сообщений на странице /messages
                var onMessagesUpdatedRequest = new SignalGlobalRequest { OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest { RecipientId = blockingUserId } };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }
        }

        async Task DeleteAllMessagesAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);
            if (index >= 0 && LastMessagesList[index].Sender != null)
            {
                int deletingAccountMessages = LastMessagesList[index].Sender!.Id == CurrentState.Account!.Id ? LastMessagesList[index].Recipient!.Id : LastMessagesList[index].Sender!.Id;

                var apiResponse = await _deleteMessages.HttpPostAsync(new DeleteMessagesRequestDto { DeleteAll = true, RecipientId = deletingAccountMessages, Token = CurrentState.Account?.Token });

                var onMessagesUpdatedRequest = new SignalGlobalRequest
                {
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                    {
                        DeleteMessages = true,
                        RecipientId = deletingAccountMessages,
                        MessagesIds = null
                    }
                };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }
        }

        async Task OnSearch(string text)
        {
        }

        public void Dispose() =>
            OnMessagesUpdatedHandler?.Dispose();
    }
}
