namespace Common.Dto.Requests
{
    public class GetNotificationsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Notifications/Get";

        public bool MarkAsRead { get; set; } = false;
    }
}
