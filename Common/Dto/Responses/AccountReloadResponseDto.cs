using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class AccountReloadResponseDto : ResponseDtoBase
    {
        public AccountsViewDto Account { get; set; } = new AccountsViewDto();
    }
}
