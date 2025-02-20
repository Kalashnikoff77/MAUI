using Data.Enums;

namespace Data.Dto.Requests
{
    public class DeleteRelationRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/DeleteRelation";

        public int RecipientId { get; set; }
        public EnumRelations EnumRelation { get; set; }
    }
}
