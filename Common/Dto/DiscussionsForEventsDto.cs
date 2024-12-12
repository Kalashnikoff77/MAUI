namespace Common.Dto
{
    public class DiscussionsForEventsDto : DtoBase
    {
        public int EventId { get; set; }

        /// <summary>
        /// На какое сообщение отвечает данное сообщение
        /// </summary>
        public int? DiscussionId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Text { get; set; } = null!;
    }
}
