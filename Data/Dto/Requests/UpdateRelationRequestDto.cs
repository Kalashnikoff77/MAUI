using Data.Enums;

namespace Data.Dto.Requests
{
    public class UpdateRelationRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/UpdateRelation";

        public int RecipientId { get; set; }
        public EnumRelations EnumRelation { get; set; }
    }
}
