using System.Text.Json;

namespace Common.Dto.Requests
{
    public class GetSchedulesForAccountsRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/GetSchedulesForAccounts";

        public int ScheduleId { get; set; }
    }
}
