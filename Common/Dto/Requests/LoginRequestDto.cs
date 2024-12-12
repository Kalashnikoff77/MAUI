namespace Common.Dto.Requests
{
    public class LoginRequestDto : RequestDtoBase
    {
        public override string Uri => "/Accounts/Login";

        public string? Email { get; set; } = null!;

        public string? Password { get; set; } = null!;

        public bool Remember { get; set; }
    }
}
