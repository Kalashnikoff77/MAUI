using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetFriendsForAccountsResponseDto : ResponseDtoBase
    {
        public List<FriendsForAccountsViewDto> Accounts { get; set; } = null!;
    }
}
