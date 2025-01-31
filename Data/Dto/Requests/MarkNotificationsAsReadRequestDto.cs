namespace Data.Dto.Requests
{
    public class MarkNotificationsAsReadRequestDto : RequestDtoBase
    {
        public override string Uri => "/notifications/MarkAsRead";

        public int? NotificationId { get; set; }

        public bool MarkAllAsRead { get; set; } = false;
    }
}
