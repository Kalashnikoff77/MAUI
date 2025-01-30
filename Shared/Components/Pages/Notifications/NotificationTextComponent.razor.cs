using Data.Dto.Sp;
using Data.Extensions;
using Data.State;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Shared.Components.Pages.Notifications
{
    public partial class NotificationTextComponent
    {
        [CascadingParameter] public CurrentState CurrentState { get; set; } = null!;

        [Parameter, EditorRequired] public LastNotificationsForAccountSpDto Notification { get; set; } = null!;
        [Parameter] public int MaxTextLength { get; set; } = 260;
        [Parameter] public EventCallback<int> MarkAsReadCallback { get; set; }
        [Parameter] public int? MarkAsReadId { get; set; }

        MarkupString htmlText = new MarkupString();
        StringBuilder formattedText = null!;
        bool isShortText = true;

        // Родительский компоненты вызывает OnInitialized только один раз, поэтому нужно обрабатывать текст в методе OnParametersSet
        protected override void OnParametersSet() => CheckText();

        async Task OnWrap()
        {
            // Вызовем сперва функцию обратного вызова, если есть
            if (MarkAsReadCallback.HasDelegate && MarkAsReadId.HasValue)
                await MarkAsReadCallback.InvokeAsync(MarkAsReadId.Value);

            isShortText = !isShortText;
            CheckText();
        }

        void CheckText()
        {
            if (!string.IsNullOrWhiteSpace(Notification.Text))
            {
                formattedText = new StringBuilder(MaxTextLength);
                if (Notification.Text.Length > MaxTextLength && isShortText)
                    htmlText = new MarkupString(formattedText.Clear().Append(Notification.Text.Substring(0, MaxTextLength)).Append("...").ToString());
                else
                    htmlText = Notification.Text.ReplaceNewLineWithBR();
            }
        }
    }
}
