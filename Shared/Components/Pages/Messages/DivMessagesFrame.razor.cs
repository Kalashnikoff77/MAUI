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
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Components.Pages.Messages
{
    /// <summary>
    /// Фрейм с сообщениями двух пользователей
    /// </summary>
    public partial class DivMessagesFrame : IDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Parameter, Required] public AccountsViewDto Account { get; set; } = null!;
        
        [Inject] IRepository<GetMessagesRequestDto, GetMessagesResponseDto> _repoGetMessages { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<OneMessage> _renderer { get; set; } = null!;

        DotNetObjectReference<DivMessagesFrame> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;
        IDisposable? OnMessagesReloadHandler;
        List<MessagesDto> messages = new List<MessagesDto>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetReference = DotNetObjectReference.Create(this);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Pages/Messages/MessagesScroll.js");
                await _JSModule.InvokeVoidAsync("SetScrollEvent", "DivMessagesFrame", _dotNetReference);
                await _JSProcessor.SetDotNetReference(_dotNetReference);

                OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
                {
                    var newMessages = await GetNextMessages();
                    await _JSModule.InvokeVoidAsync("AppendNewMessages", "DivMessagesFrame", newMessages);
                });
            }
        }

        [JSInvokable]
        public async Task<string> GetPreviousMessages()
        {
            var request = new GetMessagesRequestDto
            {
                RecipientId = Account.Id,
                GetPreviousFromId = messages.Count > 0 ? messages.Min(m => m.Id) : null,
                MarkAsRead = true,
                Take = StaticData.MESSAGES_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoGetMessages.HttpPostAsync(request);
            messages.InsertRange(0, apiResponse.Response.Messages);

            // Обновим список последних сообщений на странице /messages
            var lastMessagesRequest = new SignalGlobalRequest { OnLastMessagesReload = new OnLastMessagesReload { RecipientId = Account.Id } };
            await CurrentState.SignalRServerAsync(lastMessagesRequest);

            // Ручная генерация компонентов сообщений
            return await RenderMessages(apiResponse.Response.Messages);
        }

        public async Task<string> GetNextMessages()
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

            // Ручная генерация компонентов сообщений
            return await RenderMessages(apiResponse.Response.Messages);
        }

        /// <summary>
        /// Ручная генерация компонента Message (сообщения пользователя)
        /// </summary>
        async Task<string> RenderMessages(List<MessagesDto> messages)
        {
            var html = new StringBuilder(5000);

            foreach (var message in messages)
                html.Append(await _renderer.RenderAsync(new Dictionary<string, object?> { { "CurrentState", CurrentState }, { "Message", message } }));

            return html.ToString();
        }

        public void Dispose()
        {
            OnMessagesReloadHandler?.Dispose();
            _dotNetReference?.Dispose();
        }
    }
}
