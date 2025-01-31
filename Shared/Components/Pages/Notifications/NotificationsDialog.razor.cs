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
using System.Text;

namespace Shared.Components.Pages.Notifications
{
    public partial class NotificationsDialog : IAsyncDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [CascadingParameter] protected MudDialogInstance MudDialog { get; set; } = null!;

        [Inject] IRepository<GetNotificationsRequestDto, GetNotificationsResponseDto> _repoGetNotifications { get; set; } = null!;
        [Inject] IJSRuntime _JSRuntime { get; set; } = null!;
        [Inject] IComponentRenderer<OneNotification> _renderer { get; set; } = null!;

        DotNetObjectReference<NotificationsDialog> _dotNetReference { get; set; } = null!;
        IJSObjectReference _JSModule { get; set; } = null!;

        List<NotificationsDto> notifications = new List<NotificationsDto>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotNetReference = DotNetObjectReference.Create(this);
                _JSModule = await _JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/Pages/Notifications/NotificationsScroll.js");
                await _JSModule.InvokeVoidAsync("Initialize", _dotNetReference);
                await _JSModule.InvokeVoidAsync("LoadItems");
                await _JSModule.InvokeVoidAsync("ScrollDivToBottom");
            }
        }

        [JSInvokable]
        public async Task<string> LoadItemsAsync()
        {
            var request = new GetNotificationsRequestDto
            {
                GetPreviousFromId = notifications.Count > 0 ? notifications.Min(m => m.Id) : null,
                MarkAsRead = true,
                Take = StaticData.NOTIFICATIONS_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoGetNotifications.HttpPostAsync(request);
            notifications.InsertRange(0, apiResponse.Response.Notifications);

            // Обновим список последних сообщений на странице /notification
            var lastNotificationsRequest = new SignalGlobalRequest { OnUpdateNotificationsCount = new OnUpdateNotificationsCount() };
            await CurrentState.SignalRServerAsync(lastNotificationsRequest);

            // Ручная генерация компонентов сообщений
            return await RenderNotifications(apiResponse.Response.Notifications);
        }

        public async Task<string> GetNewNotifications()
        {
            var request = new GetNotificationsRequestDto
            {
                GetNextAfterId = notifications.Count > 0 ? notifications.Max(m => m.Id) : null,
                MarkAsRead = true,
                Take = StaticData.NOTIFICATIONS_PER_BLOCK,
                Token = CurrentState.Account?.Token
            };
            var apiResponse = await _repoGetNotifications.HttpPostAsync(request);
            notifications.AddRange(apiResponse.Response.Notifications);

            // Ручная генерация компонентов сообщений
            return await RenderNotifications(apiResponse.Response.Notifications);
        }

        /// <summary>
        /// Ручная генерация компонента OneNotification (сообщения пользователя)
        /// </summary>
        async Task<string> RenderNotifications(List<NotificationsDto> notifications)
        {
            var html = new StringBuilder(5000);

            foreach (var notification in notifications)
                html.Append(await _renderer.RenderAsync(new Dictionary<string, object?> { { "Notification", notifications } }));

            return html.ToString();
        }


        public async ValueTask DisposeAsync()
        {
            try
            {
                _dotNetReference?.Dispose();
                if (_JSModule != null)
                    await _JSModule.DisposeAsync();
            }
            // Отлов ошибки, когда соединение SignalR разорвано, но производится попытка вызвать JS. Возникает при перезагрузке страницы (F5)
            catch (JSDisconnectedException ex)
            {
            }
        }
    }
}
