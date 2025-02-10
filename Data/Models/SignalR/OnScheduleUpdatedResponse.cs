using Data.Dto.Views;
using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnScheduleUpdatedResponse : SignalRModelBase<OnScheduleUpdatedResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnScheduleUpdatedClient;

        public SchedulesForEventsViewDto? UpdatedSchedule { get; set; }
    }
}
