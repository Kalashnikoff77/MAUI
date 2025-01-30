namespace Data.Dto.Requests
{
    public class GetNotificationsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Notifications/Get";

        public int RecipientId { get; set; }

        public int? NotificationId { get; set; }

        public int? GetPreviousFromId { get; set; }
        public int? GetNextAfterId { get; set; }

        public bool MarkAsRead { get; set; } = false;
    }
}
