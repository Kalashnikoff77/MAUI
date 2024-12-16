namespace Data.Dto.Responses
{
    public class AddMessageResponseDto : ResponseDtoBase
    {
        public MessagesDto Message { get; set; } = null!;
    }
}
