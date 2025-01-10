using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;

        List<MessagesDto> messages = new List<MessagesDto>();
        Dictionary<int, AccountsViewDto> accounts { get; set; } = null!;

        string? text;
        bool sending;
        bool moreMessagesButton = false;
        int currentElementId = 0;

        DotNetObjectReference<MessagesDialog> dotNetReference = null!;

        protected override async Task OnParametersSetAsync()
        {
            OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
            {
                //await _JSProcessor.FreezeScrollBar("DivMessagesFrame");

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
                
                //await _JSProcessor.ScrollToElementWithinDiv($"id_{currentElementId}", "DivMessagesFrame");
                //await _JSProcessor.UnFreezeScrollBar("DivMessagesFrame");
            });

            var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload() };
            await CurrentState.SignalRServerAsync(request);
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                await _JSProcessor.SetScrollEvent("DivMessagesFrame");
            else
            {
                dotNetReference = DotNetObjectReference.Create(this);
                await _JSRuntime.InvokeVoidAsync("BlazorUniversity.startRandomGenerator", dotNetReference);
            }
        }

        [JSInvokable]
        public string Method(string text)
        {
            return "123";
        }

        async Task SubmitMessageAsync()
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                sending = true;
                var response = await _repoAddMessage.HttpPostAsync(new AddMessageRequestDto
                {
                    RecipientId = Account.Id,
                    Text = text,
                    Token = CurrentState.Account?.Token
                });

                moreMessagesButton = messages.Count < response.Response.Count;
                currentElementId = response.Response.NewId;

                var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload { RecipientId = Account.Id } };
                await CurrentState.SignalRServerAsync(request);

                text = null;
                sending = false;
            }
        }

        async Task GetPreviousMessagesAsync()
        {
            //await _JSProcessor.SetScrollEvent("DivMessagesFrame");

            //await _JSProcessor.FreezeScrollBar("DivMessagesFrame");
            var request = new GetMessagesRequestDto
            {
                RecipientId = Account.Id,
                GetPreviousFromId = messages.Count > 0 ? messages.Min(m => m.Id) : null,
                MarkAsRead = true,
                Take = StaticData.MESSAGES_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var response = await _repoGetMessages.HttpPostAsync(request);
            messages.InsertRange(0, response.Response.Messages);

            StateHasChanged();

            currentElementId = response.Response.Messages.Any() ? response.Response.Messages.Max(m => m.Id) : 0;
            moreMessagesButton = messages.Count < response.Response.Count;

            //await _JSProcessor.ScrollToElementWithinDiv($"id_{currentElementId}", "DivMessagesFrame");
        }


        public void Dispose() =>
            OnMessagesReloadHandler?.Dispose();
    }
}
