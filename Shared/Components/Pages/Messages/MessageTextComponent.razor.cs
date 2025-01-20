using Data.Dto.Sp;
using Data.Extensions;
using Data.State;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Shared.Components.Pages.Messages
{
    public partial class MessageTextComponent
    {
        [Parameter, EditorRequired] public CurrentState CurrentState { get; set; } = null!;
        [Parameter, EditorRequired] public LastMessagesForAccountSpDto Message { get; set; } = null!;
        [Parameter] public int MaxTextLength { get; set; } = 260;
        [Parameter] public EventCallback<int> MarkAsReadCallback { get; set; }
        [Parameter] public int? MarkAsReadId { get; set; }

        MarkupString htmlText = new MarkupString();
        StringBuilder formattedText = null!;
        bool isShortText = true;

        protected override void OnInitialized() => CheckText();

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
            if (!string.IsNullOrWhiteSpace(Message.Text))
            {
                formattedText = new StringBuilder(MaxTextLength);
                if (Message.Text.Length > MaxTextLength && isShortText)
                    htmlText = new MarkupString(formattedText.Clear().Append(Message.Text.Substring(0, MaxTextLength)).Append("...").ToString());
                else
                    htmlText = Message.Text.ReplaceNewLineWithBR();
            }
        }
    }
}
