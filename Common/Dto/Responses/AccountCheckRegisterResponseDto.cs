namespace Common.Dto.Responses
{
    public class AccountCheckRegisterResponseDto : ResponseDtoBase
    {
        public bool AccountNameExists { get; set; }
        public bool AccountEmailExists { get; set; }
    }
}
