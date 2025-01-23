namespace Data.Dto.Requests
{
    public class GetMessagesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Get";

        public int RecipientId { get; set; }

        /// <summary>
        /// Получить определённое сообщение с указанным Id
        /// </summary>
        public int? MessageId { get; set; }

        public int? GetPreviousFromId { get; set; }
        public int? GetNextAfterId { get; set; }

        public bool MarkAsRead { get; set; } = false;
    }
}
