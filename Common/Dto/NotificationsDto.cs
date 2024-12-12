namespace Common.Dto
{
    public class NotificationsDto : DtoBase
    {
        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public int SenderId { get; set; }

        public string? Text { get; set; }
    }
}
