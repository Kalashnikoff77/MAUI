using Data.Dto.Views;
using Data.Models.SignalR;
using Data.State;
using MudBlazor;
using Shared.Components.Pages.Account;
using Shared.Components.Pages.Events;

namespace Shared.Components.Dialogs
{
    public partial class ShowDialogs
    {
        public CurrentState CurrentState { get; set; } = null!;

        IServiceProvider _serviceProvider { get; set; } = null!;
        IDialogService _dialogService { get; set; } = null!;

        public ShowDialogs(IDialogService dialogService, CurrentState CurrentState, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dialogService = dialogService;
            this.CurrentState = CurrentState;
        }

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
            await _dialogService.ShowAsync<AccountInfoCardDialog>(account.Name, dialogParams, dialogOptions);
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

        async Task AccountsReload(int recipientId)
        {
            await CurrentState.SignalRServerAsync(new SignalGlobalRequest
            {
                OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = recipientId }
            });
        }

        async Task<DialogResult?> ShowDialog(string contentText, string buttonText, Color buttonColor)
        {
            var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.ContentText, contentText },
                    { x => x.ButtonText, buttonText },
                    { x => x.Color, buttonColor }
                };
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small };
            var resultDialog = await _dialogService.ShowAsync<ConfirmDialog>($"Подтверждение", parameters, options);
            return await resultDialog.Result;
        }
    }
}
