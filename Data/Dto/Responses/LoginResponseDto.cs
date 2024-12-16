using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class LoginResponseDto : ResponseDtoBase
    {
        public AccountsViewDto? Account { get; set; }
    }
}
