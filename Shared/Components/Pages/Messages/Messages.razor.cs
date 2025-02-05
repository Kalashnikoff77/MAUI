﻿using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
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
        [Inject] IRepository<UpdateRelationRequestDto, ResponseDtoBase> _repoUpdateRelation { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        IDisposable? OnUpdateMessagesCountHandler;
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
                OnUpdateMessagesCountHandler = OnUpdateMessagesCountHandler.SignalRClient<OnUpdateMessagesCountResponse>(CurrentState, async (response) =>
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

                // Обновим список последних сообщений на странице /messages
                var lastMessagesRequest = new SignalGlobalRequest { OnUpdateMessagesCount = new OnUpdateMessagesCount { RecipientId = LastMessagesList[index].Sender!.Id } };
                await CurrentState.SignalRServerAsync(lastMessagesRequest);

                // Пометим одно сообщение как прочитанное в MessageDialog
                var messagesIds = new List<int> { LastMessagesList[index].Id };
                var markMessagesAsReadRequest = new SignalGlobalRequest { OnMarkMessagesAsRead = new OnMarkMessagesAsRead { RecipientId = LastMessagesList[index].Sender!.Id, MessagesIds = messagesIds } };
                await CurrentState.SignalRServerAsync(markMessagesAsReadRequest);
            }
        }

        async Task MarkAllAsReadAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);
            if (index >= 0 && LastMessagesList[index].Sender != null)
            {
                var apiResponse = await _markMessagesAsRead.HttpPostAsync(new MarkMessagesAsReadRequestDto { MessageId = messageId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });

                // Обновим список последних сообщений на странице /messages
                var lastMessagesRequest = new SignalGlobalRequest { OnUpdateMessagesCount = new OnUpdateMessagesCount { RecipientId = LastMessagesList[index].Sender!.Id } };
                await CurrentState.SignalRServerAsync(lastMessagesRequest);

                // Пометим сообщения как прочитанные в MessageDialog
                var markMessagesAsReadRequest = new SignalGlobalRequest { OnMarkMessagesAsRead = new OnMarkMessagesAsRead { RecipientId = LastMessagesList[index].Sender!.Id, MessagesIds = null } };
                await CurrentState.SignalRServerAsync(markMessagesAsReadRequest);
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

                var updateAccountRelationRequest = new SignalGlobalRequest { OnUpdateAccountRelation = new OnUpdateAccountRelation { RecipientId = blockingUserId } };
                await CurrentState.SignalRServerAsync(updateAccountRelationRequest);
            }
        }

        async Task OnSearch(string text)
        {
        }

        public void Dispose() =>
            OnUpdateMessagesCountHandler?.Dispose();
    }
}
