using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetSchedulesForAccountsResponseDto : ResponseDtoBase
    {
        public IEnumerable<SchedulesForAccountsViewDto> Accounts { get; set; } = null!;
    }
}
