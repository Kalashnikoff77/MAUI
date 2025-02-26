using Data.Models.SignalR;
using Data.State;
using MudBlazor;

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

        async Task AccountsReload(int recipientId)
        {
            await CurrentState.SignalRServerAsync(new SignalGlobalRequest
            {
                OnReloadAccountRequest = new OnReloadAccountRequest { AdditionalAccountId = recipientId }
            });
        }

        async Task<DialogResult?> ShowConfirmDialog(string contentText, string buttonText, Color buttonColor)
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
