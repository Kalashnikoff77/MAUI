using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetSchedulesForAccountsResponseDto : ResponseDtoBase
    {
        public IEnumerable<SchedulesForAccountsViewDto> Accounts { get; set; } = null!;
    }
}
