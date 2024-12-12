using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class LoginResponseDto : ResponseDtoBase
    {
        public AccountsViewDto? Account { get; set; }
    }
}
