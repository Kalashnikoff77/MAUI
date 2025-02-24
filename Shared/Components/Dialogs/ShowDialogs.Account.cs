using Data.Dto.Requests;
using Data.Dto.Responses;
using Data.Dto.Views;
using Data.Enums;
using Data.Extensions;
using Data.Models.SignalR;
using Data.Services;
using Data.State;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;

namespace Shared.Components.Dialogs
{
    public partial class ShowDialogs
    {
        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        public async Task BlockAccountDialogAsync(AccountsViewDto? account1, AccountsViewDto? account2)
        {
            if (CurrentState.Account != null && account1 != null && account2 != null)
            {
                var hasBlockRelation = CurrentState.Account.Relations?.GetRelationInfo(EnumRelations.Blocked, account1, account2);

                var result = await ShowDialog(hasBlockRelation == null ? $"Заблокировать пользователя?" : $"Разблокировать пользователя?", hasBlockRelation == null ? "Заблокировать" : "Разблокировать", hasBlockRelation == null ? Color.Error : Color.Success);

                if (result != null && result.Canceled == false)
                {
                    var recipientId = account1.Id == CurrentState.Account.Id ? account2.Id : account1.Id;

                    var _repoUpdateRelation = _serviceProvider.GetService<IRepository<UpdateRelationRequestDto, UpdateRelationResponseDto>>()!;
                    // Добавим связь блокировки в БД
                    var apiUpdateResponse = await _repoUpdateRelation.HttpPostAsync(new UpdateRelationRequestDto
                    {
                        RecipientId = recipientId,
                        EnumRelation = EnumRelations.Blocked,
                        Token = CurrentState.Account?.Token
                    });

                    // Обновим состояния у обоих пользователей
                    await AccountsReload(recipientId);

                    // Сообщим обоим пользователям о блокировке
                    var onMessagesUpdatedRequest = new SignalGlobalRequest { OnMessagesUpdatedRequest = new OnMessagesUpdatedRequest { RecipientId = recipientId } };
                    if (apiUpdateResponse.Response.IsRelationAdded)
                        onMessagesUpdatedRequest.OnMessagesUpdatedRequest.BlockAccount = true;
                    else
                        onMessagesUpdatedRequest.OnMessagesUpdatedRequest.UnblockAccount = true;
                    await CurrentState.SignalRServerAsync(onMessagesUpdatedRequest);
                }
            }
        }
    }
}
