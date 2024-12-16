using Data.Enums;

namespace Data.Dto.Requests
{
    public class RelationsUpdateRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/UpdateRelations";

        public int RecipientId { get; set; }
        public EnumRelations EnumRelation { get; set; }
    }
}
