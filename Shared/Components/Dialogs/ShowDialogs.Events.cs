using Data.Dto.Views;
using MudBlazor;
using Shared.Components.Pages.Events;

namespace Shared.Components.Dialogs
{
    public partial class ShowDialogs
    {
        /// <summary>
        /// Карточка мероприятия
        /// </summary>
        public async Task EventInfoCardDialogAsync(SchedulesForEventsViewDto schedule)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true, BackdropClick = true };

            var dialogParams = new DialogParameters<ScheduleInfoCardDialog>
            {
                { x => x.ScheduleId, schedule.Id }
            };
            await _dialogService.ShowAsync<ScheduleInfoCardDialog>(schedule.Event?.Name, dialogParams, dialogOptions);
        }

        /// <summary>
        /// Подтверждение регистрация на клубное мероприятие
        /// </summary>
        public async Task RegistrationForEventDialogAsync(SchedulesForEventsViewDto schedule, bool isRegistered)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true };

            var dialogParams = new DialogParameters<RegisterForScheduleDialog>
            {
                { x => x.ScheduleForEventView, schedule },
                { x => x.IsRegistered, isRegistered }
            };
            await _dialogService.ShowAsync<RegisterForScheduleDialog>($"Подтверждение регистрации", dialogParams, dialogOptions);
        }
    }
}
