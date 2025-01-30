namespace Data.Entities.Sp
{
    public class LastNotificationsForAccountSpEntity
    {
        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string? Text { get; set; }

        public string? Sender { get; set; }
        public string? Recipient { get; set; }

        public int? UnreadNotifications { get; set; }
    }
}
