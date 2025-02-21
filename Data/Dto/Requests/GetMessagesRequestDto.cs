using Data.Enums;

namespace Data.Dto.Requests
{
    public class GetMessagesRequestDto : RequestDtoBase
    {
        public override string Uri => "/Messages/Get";

        public int RecipientId { get; set; }

        public EnumMessages? Type { get; set; }

        public int? GetPreviousFromId { get; set; }
        public int? GetNextAfterId { get; set; }
    }
}
