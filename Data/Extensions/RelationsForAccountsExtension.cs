using Data.Dto;
using Data.Dto.Views;
using Data.Enums;
using Data.Models;

namespace Data.Extensions
{
    public static class RelationsForAccountsExtension
    {
        /// <summary>
        /// Получить информацию о взаимосвязях двух пользователей (блокировка, дружба...)
        /// </summary>
        /// <param name="relations">Список связей</param>
        /// <param name="relationType">Тип связи</param>
        /// <param name="account1">Первый аккаунт</param>
        /// <param name="account2">Второй аккаунт</param>
        /// <returns>Null - связи нет, иначе RelationInfo</returns>
        public static RelationInfo? GetRelationInfo(this List<RelationsForAccountsDto> relations, EnumRelations relationType, AccountsViewDto? account1, AccountsViewDto? account2)
        {
            if (account1 == null || account2 == null)
                return null;

            var blockingInfo = relations?
                .FirstOrDefault(x => x.Type == relationType && ((x.SenderId == account1.Id && x.RecipientId == account2.Id) || (x.RecipientId == account1.Id && x.SenderId == account2.Id)));

            if (blockingInfo == null)
                return null;

            var result = new RelationInfo();

            if (blockingInfo.SenderId == account1.Id)
            {
                result.Sender = account1;
                result.Recipient = account2;
            }
            else
            {
                result.Sender = account2;
                result.Recipient = account1;
            }

            result.IsConfirmed = blockingInfo.IsConfirmed;

            return result;
        }
    }
}
