namespace Data.Entities.Sp
{
    public class LastMessagesForAccountSpEntity : EntityBase
    {
        public short Type { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string? Text { get; set; }

        public string? Sender { get; set; }
        public string? Recipient { get; set; }

        public int? TotalMessages { get; set; }
        public int? UnreadMessages { get; set; }
    }
}
