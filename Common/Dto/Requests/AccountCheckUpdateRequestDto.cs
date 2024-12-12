namespace Common.Dto.Requests
{
    public class AccountCheckUpdateRequestDto : RequestDtoBase
    {
        public override string Uri => "/accounts/checkupdate";
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
    }
}
