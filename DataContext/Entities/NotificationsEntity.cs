namespace DataContext.Entities
{
    public class NotificationsEntity : EntityBase
    {
        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public int RecipientId { get; set; }

        public string? Text { get; set; }

        public int SenderId { get; set; }
    }
}
