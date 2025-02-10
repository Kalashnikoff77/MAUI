namespace Data.Dto.Requests
{
    public class DeleteMessagesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Delete";

        public int MessageId { get; set; }

        public int RecipientId { get; set; }

        public bool DeleteAll { get; set; } = false;
    }
}
