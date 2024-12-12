namespace Common.Dto.Requests
{
    public class AddNotificationRequestDto : RequestDtoBase
    {
        public override string Uri => "/Notifications/Add";

        public string Text { get; set; } = null!;

        public int RecipientId { get; set; }
    }
}
