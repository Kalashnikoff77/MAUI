using Data.Enums;

namespace Data.Dto.Requests
{
    public class AddMessageRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Add";

        public EnumMessages Type { get; set; }

        public int RecipientId { get; set; }

        public string Text { get; set; } = null!;
    }
}
