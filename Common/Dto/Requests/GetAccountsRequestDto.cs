using Common.Enums;

namespace Common.Dto.Requests
{
    public class GetAccountsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/Get";

        public EnumOrders Order { get; set; } = EnumOrders.IdDesc;

        public int? Id { get; set; }
        public Guid? Guid { get; set; }

        public bool IsUsersIncluded { get; set; } = false;
        public bool IsPhotosIncluded { get; set; } = false;
        public bool IsRelationsIncluded { get; set; } = false;
        public bool IsHobbiesIncluded { get; set; } = false;
        public bool IsSchedulesIncluded { get; set; } = false;
    }
}
