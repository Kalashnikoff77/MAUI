using Data.Dto;
using Data.Dto.Views;
using Data.Enums;

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
        /// <returns>null - связи нет, Item1 - отправитель связи, Item2 - получатель связи</returns>
        public static Tuple<AccountsViewDto, AccountsViewDto>? GetRelationInfo(this List<RelationsForAccountsDto> relations, EnumRelations relationType, AccountsViewDto? account1, AccountsViewDto? account2)
        {
            if (account1 == null || account2 == null)
                return null;

            var blockingInfo = relations?
                .FirstOrDefault(x => x.Type == relationType && ((x.SenderId == account1.Id && x.RecipientId == account2.Id) || (x.RecipientId == account1.Id && x.SenderId == account2.Id)));

            if (blockingInfo == null)
                return null;

            return blockingInfo.SenderId == account1.Id ? new Tuple<AccountsViewDto, AccountsViewDto>(account1, account2) : new Tuple<AccountsViewDto, AccountsViewDto>(account2, account1);
        }
    }
}
