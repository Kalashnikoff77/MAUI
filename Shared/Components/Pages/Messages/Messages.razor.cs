﻿using Data.Dto.Requests;
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

        async Task MarkMessagesAsReadAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);

            // Проверим, помечено ли сообщение как прочитанное и адресовано ли нам?
            if (index >= 0 && LastMessagesList[index].Recipient?.Id == CurrentState.Account?.Id && LastMessagesList[index].Sender != null && LastMessagesList[index].ReadDate == null)
            {
                var recipientId = CurrentState.Account?.Id == LastMessagesList[index].Recipient?.Id ? LastMessagesList[index].Sender?.Id : LastMessagesList[index].Recipient?.Id;
                var onMessagesUpdatedRequest = new SignalGlobalRequest 
                {
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest 
                    {
                        MarkMessagesAsRead = true,
                        RecipientId = recipientId,
                        MessagesIds = new List<int> { LastMessagesList[index].Id }
                    } 
                };
                await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
            }
        }

        async Task MarkAllMessagesAsReadAsync(int messageId)
        {
            var index = LastMessagesList.FindIndex(x => x.Id == messageId);

            var recipientId = CurrentState.Account?.Id == LastMessagesList[index].Recipient?.Id ? LastMessagesList[index].Sender?.Id : LastMessagesList[index].Recipient?.Id;
            if (index >= 0 && LastMessagesList[index].Sender != null && recipientId != null)
            {
                var onMessagesUpdatedRequest = new SignalGlobalRequest
                {
                    OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                    {
                        MarkAllMessagesAsRead = true,
                        RecipientId = recipientId,
                        MessageId = messageId
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
