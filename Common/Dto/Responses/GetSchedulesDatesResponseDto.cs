using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetSchedulesDatesResponseDto : ResponseDtoBase
    {
        public IEnumerable<SchedulesDatesViewDto>? SchedulesDates { get; set; } = null!;
    }
}
