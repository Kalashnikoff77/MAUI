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

namespace Shared.Components.Pages.Messages.MessagesDialog
{
    /// <summary>
    /// Фрейм с сообщениями двух пользователей
    /// </summary>
    public partial class DivMessagesFrame : IAsyncDisposable
    {
        [Parameter, Required] public AccountsViewDto Account { get; set; } = null!;

        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        
        [Inject] IRepository<GetMessagesRequestDto, GetMessagesResponseDto> _repoGetMessages { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<OneDivMessage> _renderer { get; set; } = null!;

        DotNetObjectReference<DivMessagesFrame> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;

        List<MessagesDto> messages = new List<MessagesDto>();

        protected override void OnParametersSet()
        {
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
                    await _JSModule.InvokeVoidAsync("AppendNewMessages", await GetNextMessages()));

                _dotNetReference = DotNetObjectReference.Create(this);
                await _JSProcessor.SetDotNetReference(_dotNetReference);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Pages/Messages/DivMessagesFrameScroll.js");
                await _JSModule.InvokeVoidAsync("LoadItems");
                await _JSModule.InvokeVoidAsync("SetScrollEvent");
            }
        }


        [JSInvokable]
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

        public async ValueTask DisposeAsync()
        {
            try
            {
                OnMessagesReloadHandler?.Dispose();
                if (_JSModule != null)
                    await _JSModule.DisposeAsync();
                _dotNetReference?.Dispose();
            }
            // Отлов ошибки, когда соединение SignalR разорвано, но производится попытка вызвать JS. Возникает при перезагрузке страницы (F5)
            catch (JSDisconnectedException ex)
            {
            }
        }
    }
}
