namespace Data.Dto.Requests
{
    public class RememberRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/Remember";

        public string Email { get; set; } = null!;
    }
}
