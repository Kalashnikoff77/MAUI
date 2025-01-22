using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace Shared.Components.Pages.Messages
{
    public partial class MessagesDialog
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter, Required] public AccountsViewDto Recipient { get; set; } = null!;

        [Inject] IRepository<AddMessageRequestDto, AddMessageResponseDto> _repoAddMessage { get; set; } = null!;

        string? text;
        bool sending;

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        async Task SubmitMessageAsync()
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                sending = true;
                var response = await _repoAddMessage.HttpPostAsync(new AddMessageRequestDto
                {
                    RecipientId = Recipient.Id,
                    Text = text,
                    Token = CurrentState.Account?.Token
                });

                // Обновим сообщения в диалоговом окне
                var messagesRequest = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload { RecipientId = Recipient.Id } };
                await CurrentState.SignalRServerAsync(messagesRequest);

                // Обновим список последних сообщений на странице /messages
                var lastMessagesRequest = new SignalGlobalRequest { OnLastMessagesReload = new OnLastMessagesReload { RecipientId = Recipient.Id } };
                await CurrentState.SignalRServerAsync(lastMessagesRequest);

                text = null;
                sending = false;
            }
        }
    }
}
