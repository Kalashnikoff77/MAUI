namespace Data.Dto.Requests
{
    public class DeleteMessageRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Delete";

        public int MessageId { get; set; }

        public int RecipientId { get; set; }
    }
}
