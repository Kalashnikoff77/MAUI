using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Dto.Views;
using Data.Enums;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Shared.Components.Pages.Messages;

namespace Shared.Components.Dialogs
{
    public partial class ShowDialogs
    {
        /// <summary>
        /// Окно общения двух пользователей
        /// </summary>
        public async Task MessagesDialogAsync(AccountsViewDto account)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true };

            var dialogParams = new DialogParameters<MessagesDialog>
            {
                { x => x.Recipient, account }
            };
            await _dialogService.ShowAsync<MessagesDialog>(null, dialogParams, dialogOptions);
        }

        /// <summary>
        /// Подтверждение удаления всей переписки
        /// </summary>
        public async Task DeleteAllMessagesDialogAsync(List<LastMessagesForAccountSpDto> lastMessagesList, int messageId)
        {
            if (CurrentState.Account != null)
            {
                var result = await ShowConfirmDialog("Удалить всю переписку?", "Да", Color.Error);

                if (result != null && result.Canceled == false)
                {
                    var index = lastMessagesList.FindIndex(x => x.Id == messageId);
                    var recipientId = lastMessagesList[index].Sender?.Id == CurrentState.Account.Id ? lastMessagesList[index].Recipient?.Id : lastMessagesList[index].Sender?.Id;

                    if (index >= 0 && lastMessagesList[index].Sender != null && recipientId != null)
                    {
                        // 1. Удалим все сообщения в таблице Messages
                        var _deleteMessages = _serviceProvider.GetService<IRepository<DeleteMessagesRequestDto, ResponseDtoBase>>()!;
                        var apiResponse = await _deleteMessages.HttpPostAsync(new DeleteMessagesRequestDto
                        {
                            DeleteAll = true,
                            RecipientId = recipientId.Value,
                            Token = CurrentState.Account.Token
                        });

                        // 2. Добавим информационное сообщение в Messages
                        var _addMessages = _serviceProvider.GetService<IRepository<AddMessageRequestDto, AddMessageResponseDto>>()!;
                        var apiResult = await _addMessages.HttpPostAsync(new AddMessageRequestDto
                        {
                            Type = EnumMessages.AllMessagesDeleted,
                            RecipientId = recipientId.Value,
                            Text = StaticData.NotificationTypes[EnumMessages.AllMessagesDeleted].Text,
                            Token = CurrentState.Account.Token
                        });

                        // 3. Сообщим обоим пользователям о событии
                        var onMessagesUpdatedRequest = new SignalGlobalRequest
                        {
                            OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest
                            {
                                DeleteMessages = true,
                                RecipientId = recipientId.Value,
                                MessagesIds = null
                            }
                        };
                        await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                    }
                }
            }
        }
    }
}
