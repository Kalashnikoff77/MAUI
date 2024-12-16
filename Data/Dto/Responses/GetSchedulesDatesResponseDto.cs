using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetSchedulesDatesResponseDto : ResponseDtoBase
    {
        public IEnumerable<SchedulesDatesViewDto>? SchedulesDates { get; set; } = null!;
    }
}
