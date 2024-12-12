namespace DataContext.Entities
{
    public class MessagesEntity : EntityBase
    {
        public DateTime CreateDate { get; set; }

        public DateTime? ReadDate { get; set; } = null!;

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string Text { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
