using Data.Dto.Views;
using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnScheduleChangedResponse : SignalRModelBase<OnScheduleChangedResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnScheduleChangedClient;

        public SchedulesForEventsViewDto? UpdatedSchedule { get; set; }
    }
}
