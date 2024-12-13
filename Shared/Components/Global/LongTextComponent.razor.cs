using Common.Extensions;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Shared.Components.Global
{
    public partial class LongTextComponent
    {
        [Parameter, EditorRequired] public string Text { get; set; } = null!;
        [Parameter] public int MaxTextLength { get; set; } = 260;

        MarkupString htmlText = new MarkupString();
        StringBuilder formattedText = null!;
        bool isShortText = true;

        protected override void OnInitialized() => CheckText();

        void OnWrap()
        {
            isShortText = !isShortText;
            CheckText();
        }

        void CheckText()
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                formattedText = new StringBuilder(MaxTextLength);
                if (Text.Length > MaxTextLength && isShortText)
                    htmlText = new MarkupString(formattedText.Clear().Append(Text.Substring(0, MaxTextLength)).Append("...").ToString());
                else
                    htmlText = Text.ReplaceNewLineWithBR();
            }
        }
    }
}
