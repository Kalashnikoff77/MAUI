using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetAccountsResponseDto : ResponseDtoBase
    {
        public AccountsViewDto Account { get; set; } = null!;
        public List<AccountsViewDto> Accounts { get; set; } = null!;
    }
}
