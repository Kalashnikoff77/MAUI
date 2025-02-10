using Data.Dto;
using Data.Dto.Views;
using Data.Enums;
using Data.Models;
using Data.State;
using System.Text;

namespace Data.Extensions
{
    public static class AccountDtoExtension
    {
        /// <summary>
        /// Вывод всех пользователей учётки: пол + возраст (М45 Ж46)
        /// </summary>
        /// <param name="finish">HTML, добавляемый в конец строки</param>
        /// <returns>"М45 Ж46"</returns>
        public static string ToGendersAgesString(this AccountsViewDto account, string? finish = null)
        {
            StringBuilder? genders = new StringBuilder(20);

            if (account.Users!.Count > 0)
            {
                genders.Append(account.Users
                    .OrderBy(o => o.Gender)
                    .Select(s => s.ToGenderAgeString())
                    .Aggregate((a, b) => a + " " + b));
                genders.Append(finish);
            }

            return genders.ToString();
        }

        /// <summary>
        /// Вывод одного пользователя: пол + возраст (М45)
        /// </summary>
        /// <param name="finish">HTML, добавляемый в конец строки</param>
        /// <returns>"М45"</returns>
        public static string? ToGenderAgeString(this UsersDto user, string? finish = null)
        {
            if (user != null)
            {
                    var age = DateTime.Today.Year - user.BirthDate.Year;
                    if (user.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;
                    return $"{StaticData.Genders[user.Gender].ShortName}{age}";
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Конвертация страны и региона в текст
        /// </summary>
        /// <param name="finish">HTML, добавляемый в конец строки</param>
        /// <returns>"Россия, Москва"</returns>
        public static string ToRegionString(this AccountsViewDto account) =>
            $"{account.Country!.Name}, {account.Country.Region.Name}";


        /// <summary>
        /// Конвертация аватара учётки
        /// </summary>
        /// <param name="size">Размер аватара: "EnumImageSize.s64x64"</param>
        /// <returns>Путь к аватару</returns>
        public static string ToAvatarUri(this AccountsViewDto account, EnumImageSize size)
        {
            var photo = account.Avatar;
            return photo != null ? $"/images/AccountsPhotos/{account.Id}/{photo.Guid}/{size}.jpg" : $"/images/AccountsPhotos/no-avatar/{size}.jpg";
        }


        /// <summary>
        /// Получить информацию о блокировке пользователей
        /// </summary>
        /// <returns>Item1 (bool) - есть ли блокировка, Item2 - отправитель блокировки, Item3 - получатель блокировки</returns>
        public static Tuple<bool, AccountsViewDto, AccountsViewDto> GetBlockInfo(this AccountsViewDto currentAccount, AccountsViewDto? account1, AccountsViewDto? account2)
        {
            var blockingInfo = currentAccount.Relations?
                .FirstOrDefault(x => x.Type == (short)EnumRelations.Blocked && ((x.SenderId == account1?.Id && x.RecipientId == account2?.Id) || (x.RecipientId == account1?.Id && x.SenderId == account2?.Id)));

            var isBlocked = blockingInfo == null ? false : true;

            var result = new Tuple<bool, AccountsViewDto, AccountsViewDto>(isBlocked, new AccountsViewDto(), new AccountsViewDto());
            return result;
        }
    }
}
