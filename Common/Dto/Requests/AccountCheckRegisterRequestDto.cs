namespace Common.Dto.Requests
{
    public class AccountCheckRegisterRequestDto : RequestDtoBase
    {
        public override string Uri => "/accounts/checkregister";
        public string AccountName { get; set; } = null!;
        public string AccountEmail { get; set; } = null!;
    }
}
