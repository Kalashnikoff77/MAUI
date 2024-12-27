using Data.Dto;
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
    public partial class MessagesDialog : IDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;
        [Parameter, Required] public AccountsViewDto Account { get; set; } = null!;

        [Inject] IRepository<AddMessageRequestDto, AddMessageResponseDto> _repoAddMessage { get; set; } = null!;
        [Inject] IRepository<GetMessagesRequestDto, GetMessagesResponseDto> _repoGetMessages { get; set; } = null!;

        IDisposable? OnMessageAddedHandler;

        List<MessagesDto>? Messages { get; set; }
        Dictionary<int, AccountsViewDto> Accounts { get; set; } = null!;

        string? Text;

        protected override async Task OnParametersSetAsync()
        {
            OnMessageAddedHandler = OnMessageAddedHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
            {
                var request = new GetMessagesRequestDto
                {
                    RecipientId = Account.Id,
                    MarkAsRead = true,
                    Token = CurrentState.Account?.Token,
                    GetNextAfterId = Messages?.Max(x => x.Id)
                };

                var apiResponse = await _repoGetMessages.HttpPostAsync(request);
                Messages = apiResponse.Response.Messages;
                Accounts = apiResponse.Response.Accounts;

                await InvokeAsync(StateHasChanged);
            });

            var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload() };
            await CurrentState.SignalRServerAsync(request);
        }

        async Task SubmitMessageAsync()
        {
            var response = await _repoAddMessage.HttpPostAsync(new AddMessageRequestDto 
            {
                RecipientId = Account.Id,
                Text = Text,
                Token = CurrentState.Account?.Token
            });

            var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload { RecipientId = Account.Id } };
            await CurrentState.SignalRServerAsync(request);

            Text = null;
        }


        public void Dispose() =>
            OnMessageAddedHandler?.Dispose();
    }
}
