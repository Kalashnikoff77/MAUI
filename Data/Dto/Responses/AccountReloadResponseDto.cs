using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class AccountReloadResponseDto : ResponseDtoBase
    {
        public AccountsViewDto Account { get; set; } = new AccountsViewDto();
    }
}
