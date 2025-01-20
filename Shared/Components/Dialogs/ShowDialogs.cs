using Data.Dto.Views;
using MudBlazor;
using Shared.Components.Pages.Account;
using Shared.Components.Pages.Events;
using Shared.Components.Pages.Messages.MessagesDialog;

namespace Shared.Components.Dialogs
{
    public class ShowDialogs
    {
        IDialogService _dialog { get; set; } = null!;

        public ShowDialogs(IDialogService dialog) => _dialog = dialog;

        /// <summary>
        /// Карточка аккаунта
        /// </summary>
        public async Task AccountInfoCardDialogAsync(AccountsViewDto account)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true, BackdropClick = true };

            var dialogParams = new DialogParameters<AccountInfoCardDialog>
            {
                { x => x.Account, account }
            };
            await _dialog.ShowAsync<AccountInfoCardDialog>(account.Name, dialogParams, dialogOptions);
        }

        /// <summary>
        /// Карточка мероприятия
        /// </summary>
        public async Task ScheduleInfoCardDialogAsync(SchedulesForEventsViewDto schedule)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true, BackdropClick = true };

            var dialogParams = new DialogParameters<ScheduleInfoCardDialog>
            {
                { x => x.ScheduleId, schedule.Id }
            };
            await _dialog.ShowAsync<ScheduleInfoCardDialog>(schedule.Event?.Name, dialogParams, dialogOptions);
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
            await _dialog.ShowAsync<RegisterForScheduleDialog>($"Подтверждение регистрации", dialogParams, dialogOptions);
        }


        /// <summary>
        /// Окно общения двух пользователей
        /// </summary>
        public async Task MessagesDialogAsync(AccountsViewDto account)
        {
            DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, CloseButton = true };

            var dialogParams = new DialogParameters<MessagesDialog>
            {
                { x => x.Account, account }
            };
            await _dialog.ShowAsync<MessagesDialog>(null, dialogParams, dialogOptions);
        }
    }
}
