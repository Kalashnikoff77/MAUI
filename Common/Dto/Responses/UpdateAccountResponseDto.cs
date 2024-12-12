namespace Common.Dto.Responses
{
    public class UpdateAccountResponseDto : ResponseDtoBase
    {
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
    }
}
