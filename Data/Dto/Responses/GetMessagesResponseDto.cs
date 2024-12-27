using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetMessagesResponseDto : ResponseDtoBase
    {
        public List<MessagesDto> Messages { get; set; } = new List<MessagesDto>();
        public Dictionary<int, AccountsViewDto> Accounts { get; set; } = null!;
    }
}
