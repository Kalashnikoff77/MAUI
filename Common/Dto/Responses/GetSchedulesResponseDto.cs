using Common.Dto.Views;

namespace Common.Dto.Responses
{
    public class GetSchedulesResponseDto : ResponseDtoBase
    {
        public SchedulesForEventsViewDto? Schedule { get; set; }

        /// <summary>
        /// Список расписаний в рамках FROM TO
        /// </summary>
        public List<SchedulesForEventsViewDto>? Schedules { get; set; }
    }
}
