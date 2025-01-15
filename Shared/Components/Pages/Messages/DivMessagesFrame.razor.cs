using Data.Dto;
using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Models;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        [Parameter, Required] public AccountsViewDto Account { get; set; } = null!;

        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IJSProcessor _JSProcessor { get; set; } = null!;
        [Inject] IRepository<GetMessagesRequestDto, GetMessagesResponseDto> _repoGetMessages { get; set; } = null!;
        [Inject] IServiceProvider _serviceProvider { get; set; } = null!;

        DotNetObjectReference<DivMessagesFrame> _objectReference { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;

        List<MessagesDto> messages = new List<MessagesDto>();

        protected override void OnParametersSet()
        {
            OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
                await _JSProcessor.AppendNewMessages("DivMessagesFrame", await GetNextMessages()));
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _objectReference = DotNetObjectReference.Create(this);
                await _JSProcessor.SetScrollEvent("DivMessagesFrame", _objectReference);
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
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());
            var html = new StringBuilder(5000);
            foreach (var message in messages)
            {
                html.Append(await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var dictionary = new Dictionary<string, object?>
                    {
                        { "CurrentState", CurrentState },
                        { "message", message }
                    };
                    var output = await htmlRenderer.RenderComponentAsync<OneMessage>(ParameterView.FromDictionary(dictionary));
                    return output.ToHtmlString();
                }));
            }
            return html.ToString();
        }

        public void Dispose()
        {
            OnMessagesReloadHandler?.Dispose();
            _objectReference.Dispose();
        }
    }
}
