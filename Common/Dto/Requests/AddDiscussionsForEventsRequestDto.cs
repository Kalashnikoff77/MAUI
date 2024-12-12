namespace Common.Dto.Requests
{
    public class AddDiscussionsForEventsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/AddDiscussion";

        public int EventId { get; set; }

        /// <summary>
        /// Кому отправляется ответ
        /// </summary>
        public int? RecipientId { get; set; }

        /// <summary>
        /// На какое сообщение отправляется ответ
        /// </summary>
        public int? DiscussionId { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; } = null!;
    }
}
