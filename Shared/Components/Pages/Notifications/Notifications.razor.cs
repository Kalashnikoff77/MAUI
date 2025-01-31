using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Sp;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Notifications
{
    public partial class Notifications : IDisposable
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;
        [Inject] IRepository<GetLastNotificationsListRequestDto, GetLastNotificationsListResponseDto> _repoGetLastNotificationsList { get; set; } = null!;
        [Inject] IRepository<MarkNotificationsAsReadRequestDto, ResponseDtoBase> _markNotificationsAsRead { get; set; } = null!;
        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        IDisposable? OnUpdateNotificationsCountHandler;
        List<LastNotificationsForAccountSpDto> LastNotificationsList = new List<LastNotificationsForAccountSpDto>();

        protected override async Task OnParametersSetAsync()
        {
            if (CurrentState.Account != null)
            {
                var request = new GetLastNotificationsListRequestDto() { Token = CurrentState.Account.Token };
                var apiResponse = await _repoGetLastNotificationsList.HttpPostAsync(request);
                LastNotificationsList = apiResponse.Response.LastNotificationsList ?? new List<LastNotificationsForAccountSpDto>();
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                OnUpdateNotificationsCountHandler = OnUpdateNotificationsCountHandler.SignalRClient<OnUpdateNotificationsCountResponse>(CurrentState, async (response) =>
                {
                    var request = new GetLastNotificationsListRequestDto() { Token = CurrentState.Account?.Token };
                    var apiResponse = await _repoGetLastNotificationsList.HttpPostAsync(request);
                    LastNotificationsList = apiResponse.Response.LastNotificationsList ?? new List<LastNotificationsForAccountSpDto>();
                    await InvokeAsync(StateHasChanged);
                });
            }
        }

        async Task MarkAsReadAsync(int markAsReadNotificationId)
        {
            var index = LastNotificationsList.FindIndex(x => x.Id == markAsReadNotificationId);
            // Проверим, помечено ли уведомление как прочитанное и адресовано ли нам?
            if (index >= 0 && LastNotificationsList[index].Recipient?.Id == CurrentState.Account?.Id && LastNotificationsList[index].Sender != null && LastNotificationsList[index].ReadDate == null)
            {
                // Помечаем уведомление как прочитанное в БД
                var apiResponse = await _markNotificationsAsRead.HttpPostAsync(new MarkNotificationsAsReadRequestDto { NotificationId = markAsReadNotificationId, MarkAllAsRead = false, Token = CurrentState.Account?.Token });

                // Обновим список последних уведомлений на странице /notifications
                var lastNotificationRequest = new SignalGlobalRequest { OnUpdateNotificationsCount = new OnUpdateNotificationsCount() };
                await CurrentState.SignalRServerAsync(lastNotificationRequest);
            }
        }

        async Task MarkAllAsReadAsync(int markAsReadNotificationId)
        {
            var index = LastNotificationsList.FindIndex(x => x.Id == markAsReadNotificationId);
            if (index >= 0 && LastNotificationsList[index].Sender != null)
            {
                var apiResponse = await _markNotificationsAsRead.HttpPostAsync(new MarkNotificationsAsReadRequestDto { NotificationId = markAsReadNotificationId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });

                // Обновим список последних уведомлений на странице /notifications
                var lastNotificationsRequest = new SignalGlobalRequest { OnUpdateNotificationsCount = new OnUpdateNotificationsCount() };
                await CurrentState.SignalRServerAsync(lastNotificationsRequest);
            }
        }

        public void Dispose() =>
            OnUpdateNotificationsCountHandler?.Dispose();
    }
}
