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
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.Text;

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

        [Inject] IServiceProvider serviceProvider { get; set; } = null!;

        IDisposable? OnMessagesReloadHandler;

        List<MessagesDto> messages = new List<MessagesDto>();

        string? text;
        bool sending;
        int lastElementId = 0;

        protected override void OnParametersSet()
        {
            OnMessagesReloadHandler = OnMessagesReloadHandler.SignalRClient<OnMessagesReloadResponse>(CurrentState, async (response) =>
            {
                await GetMessages();
                //await InvokeAsync(StateHasChanged);
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await _JSProcessor.SetScrollEvent("DivMessagesFrame", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async Task<string> GetMessages()
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
            await using var htmlRenderer = new HtmlRenderer(serviceProvider, serviceProvider.GetRequiredService<ILoggerFactory>());
            var html = new StringBuilder(5000);
            foreach (var message in apiResponse.Response.Messages) 
            {
                html.Append(await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var dictionary = new Dictionary<string, object?>
                    {
                        { "CurrentState", CurrentState },
                        { "message", message }
                    };
                    var output = await htmlRenderer.RenderComponentAsync<Message>(ParameterView.FromDictionary(dictionary));
                    return output.ToHtmlString();
                }));
            }
            return html.ToString();
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

                lastElementId = response.Response.NewId;

                var request = new SignalGlobalRequest { OnMessagesReload = new OnMessagesReload { RecipientId = Account.Id } };
                await CurrentState.SignalRServerAsync(request);

                text = null;
                sending = false;
            }
        }

        async Task GetPreviousMessagesAsync()
        {
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

            lastElementId = response.Response.Messages.Any() ? response.Response.Messages.Max(m => m.Id) : 0;
        }


        public void Dispose() =>
            OnMessagesReloadHandler?.Dispose();
    }
}
