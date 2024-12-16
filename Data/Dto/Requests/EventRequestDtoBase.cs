using Data.Dto.Views;

namespace Data.Dto.Requests
{
    public abstract class EventRequestDtoBase : RequestDtoBase
    {
        public EventsViewDto Event { get; set; } = null!;
    }
}
