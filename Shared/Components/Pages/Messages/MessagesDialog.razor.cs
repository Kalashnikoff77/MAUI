using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
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
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;

        List<MessagesDto> messages = new List<MessagesDto>();
        Dictionary<int, AccountsViewDto> accounts { get; set; } = null!;

        string? text;
        bool sending;
        bool moreMessagesButton = false;
        int currentElementId = 0;

        protected override async Task OnParametersSetAsync()
        {
            OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
            {
                var request = new GetMessagesRequestDto
                {
                    RecipientId = Account.Id,
                    GetNextAfterId = messages.Count > 0 ? messages.Max(m => m.Id) : null,
                    MarkAsRead = true,
                    Take = StaticData.MESSAGES_PER_BLOCK,
                    Token = CurrentState.Account?.Token
                };

                var apiResponse = await _repoGetMessages.HttpPostAsync(request);
                messages.AddRange(apiResponse.Response.Messages);
                accounts = apiResponse.Response.Accounts;

                moreMessagesButton = messages.Count < apiResponse.Response.Count;

                currentElementId = messages.Any() ? messages.Max(m => m.Id) : 0;

                await InvokeAsync(StateHasChanged);

                await _JSProcessor.ScrollToElementWithinDiv($"id_{currentElementId}", "DivMessagesFrame");
            });

            var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload() };
            await CurrentState.SignalRServerAsync(request);
        }


        async Task SubmitMessageAsync()
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var response = await _repoAddMessage.HttpPostAsync(new AddMessageRequestDto
                {
                    RecipientId = Account.Id,
                    Text = text,
                    Token = CurrentState.Account?.Token
                });

                var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload { RecipientId = Account.Id } };
                await CurrentState.SignalRServerAsync(request);

                text = null;
            }
        }

        async Task GetMessagesAsync()
        {

        }

        public void Dispose() =>
            OnMessagesReloadHandler?.Dispose();
    }
}
