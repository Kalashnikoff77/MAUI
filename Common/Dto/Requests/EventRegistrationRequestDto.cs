namespace Common.Dto.Requests
{
    public class EventRegistrationRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/RegistrationForEvent";

        public int ScheduleId { get; set; }
    }
}
