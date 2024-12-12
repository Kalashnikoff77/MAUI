using Common.Dto.Views;

namespace Common.Dto.Requests
{
    public abstract class EventRequestDtoBase : RequestDtoBase
    {
        public EventsViewDto Event { get; set; } = null!;
    }
}
