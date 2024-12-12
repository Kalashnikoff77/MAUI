namespace DataContext.Entities.Sp
{
    public class LastMessagesForAccountSpEntity : EntityBase
    {
        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string? Text { get; set; }

        public string? Sender { get; set; }
        public string? Recipient { get; set; }

        public int? UnreadMessages { get; set; }
    }
}
