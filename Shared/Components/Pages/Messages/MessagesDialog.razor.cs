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

        IDisposable? OnMessageAddedHandler;

        string? _text { get; set; } = null!;

        protected override void OnAfterRender(bool firstRender)
        {
            OnMessageAddedHandler = OnMessageAddedHandler.SignalRClient<OnMessageAddedResponse>(CurrentState, async (response) =>
            {

                await InvokeAsync(StateHasChanged);
            });
        }

        async Task SubmitMessageAsync()
        {
            _text = "Супер пупер - 22";

            var response = await _repoAddMessage.HttpPostAsync(new AddMessageRequestDto 
            {
                RecipientId = Account.Id,
                Text = _text,
                Token = CurrentState.Account?.Token
            });

            var request = new SignalGlobalRequest
            {
                OnMessageAdded = new OnMessageAdded { Message = response.Response.Message }
            };
            await CurrentState.SignalRServerAsync(request);

            _text = null;
        }


        public void Dispose() =>
            OnMessageAddedHandler?.Dispose();
    }
}
