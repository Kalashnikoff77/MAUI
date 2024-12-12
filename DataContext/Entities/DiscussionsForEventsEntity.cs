namespace DataContext.Entities
{
    public class DiscussionsForEventsEntity : EntityBase
    {
        public int EventId { get; set; }

        public int SenderId { get; set; }
        public int? RecipientId { get; set; }

        /// <summary>
        /// На какое сообщение отвечает данное сообщение
        /// </summary>
        public int? DiscussionId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Text { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
