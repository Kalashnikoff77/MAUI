using Data.Dto.Functions;

namespace Data.Dto.Views
{
    public class SchedulesForEventsViewDto : SchedulesForEventsDto
    {
        public EventsViewDto? Event { get; set; }

        public List<FeaturesDto>? Features { get; set; } = new List<FeaturesDto>();

        public GetScheduleStatisticFunctionDto? Statistic { get; set; }
    }
}
