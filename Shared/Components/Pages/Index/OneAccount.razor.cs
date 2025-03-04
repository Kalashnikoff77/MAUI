using Data.Dto.Views;
using Data.Enums;
using Data.Extensions;
using Data.Models;
using Data.State;
using Microsoft.AspNetCore.Components;
using Shared.Components.Dialogs;

namespace Shared.Components.Pages.Index
{
    public partial class OneAccount
    {
        [CascadingParameter] CurrentState CurrentState { get; set; } = null!;
        [Parameter, EditorRequired] public ShowDialogs ShowDialogs { get; set; } = null!;
        [Parameter, EditorRequired] public AccountsViewDto Account { get; set; } = null!;

        RelationInfo? IsBlockedUser;
        RelationInfo? HasFriendship;

        protected override void OnParametersSet()
        {
            if (CurrentState.Account != null)
            {
                IsBlockedUser = CurrentState.Account.Relations?.GetRelationInfo(EnumRelations.Blocked, CurrentState.Account, Account);
                HasFriendship = CurrentState.Account.Relations?.GetRelationInfo(EnumRelations.Friend, CurrentState.Account, Account);
            }
        }
    }
}
