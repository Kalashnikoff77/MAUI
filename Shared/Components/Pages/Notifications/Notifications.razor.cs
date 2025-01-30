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
        //[Inject] IRepository<MarkMessageAsReadRequestDto, ResponseDtoBase> _markMessageAsRead { get; set; } = null!;
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

        async Task MarkAsReadAsync(int markAsReadMessageId)
        {
            //var index = NotificationsList.FindIndex(x => x.Id == markAsReadMessageId);
            //// Проверим, помечено ли сообщение как прочитанное и адресовано ли нам?
            //if (index >= 0 && NotificationsList[index].Recipient?.Id == CurrentState.Account?.Id && NotificationsList[index].Sender != null && NotificationsList[index].ReadDate == null)
            //{
            //    // Помечаем сообщение как прочитанное в БД
            //    var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = false, Token = CurrentState.Account?.Token });

            //    // Обновим список последних сообщений на странице /messages
            //    var lastMessagesRequest = new SignalGlobalRequest { OnUpdateMessagesCount = new OnUpdateMessagesCount { RecipientId = NotificationsList[index].Sender!.Id } };
            //    await CurrentState.SignalRServerAsync(lastMessagesRequest);

            //    // Пометим одно сообщение как прочитанное в MessageDialog
            //    var messagesIds = new List<int> { NotificationsList[index].Id };
            //    var markMessagesAsReadRequest = new SignalGlobalRequest { OnMarkMessagesAsRead = new OnMarkMessagesAsRead { RecipientId = NotificationsList[index].Sender!.Id, MessagesIds = messagesIds } };
            //    await CurrentState.SignalRServerAsync(markMessagesAsReadRequest);
            //}
        }

        async Task MarkAllAsReadAsync(int markAsReadMessageId)
        {
            //var index = NotificationsList.FindIndex(x => x.Id == markAsReadMessageId);
            //if (index >= 0 && NotificationsList[index].Sender != null)
            //{
            //    var apiResponse = await _markMessageAsRead.HttpPostAsync(new MarkMessageAsReadRequestDto { MessageId = markAsReadMessageId, MarkAllAsRead = true, Token = CurrentState.Account?.Token });

            //    // Обновим список последних сообщений на странице /messages
            //    var lastMessagesRequest = new SignalGlobalRequest { OnUpdateMessagesCount = new OnUpdateMessagesCount { RecipientId = NotificationsList[index].Sender!.Id } };
            //    await CurrentState.SignalRServerAsync(lastMessagesRequest);

            //    // Пометим сообщения как прочитанные в MessageDialog
            //    var markMessagesAsReadRequest = new SignalGlobalRequest { OnMarkMessagesAsRead = new OnMarkMessagesAsRead { RecipientId = NotificationsList[index].Sender!.Id, MessagesIds = null } };
            //    await CurrentState.SignalRServerAsync(markMessagesAsReadRequest);
            //}
        }

        public void Dispose() =>
            OnUpdateNotificationsCountHandler?.Dispose();
    }
}
