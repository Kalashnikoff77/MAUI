using Data.Dto.Sp;
using Data.Dto.Views;
using Data.State;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Messages
{
    public partial class OneMessage
    {
        [Parameter, EditorRequired] public CurrentState CurrentState { get; set; } = null!;
        [Parameter, EditorRequired] public LastMessagesForAccountSpDto Message { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback<int> MarkAsReadCallbackAsync { get; set; }
        [Parameter, EditorRequired] public EventCallback<int> MarkAllAsReadAsync { get; set; }
        [Parameter, EditorRequired] public EventCallback BlockAccountAsync { get; set; }

        [Inject] ShowDialogs ShowDialogs { get; set; } = null!;

        AccountsViewDto? accountsViewDto { get; set; }

        protected override void OnInitialized()
        {
            accountsViewDto = CurrentState?.Account?.Id == Message.Sender?.Id ? Message.Recipient : Message.Sender;
        }

    }
}
