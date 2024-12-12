namespace Common.Dto.Requests
{
    public class AddMessageRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Add";

        public int RecipientId { get; set; }

        public string Text { get; set; } = null!;
    }
}
