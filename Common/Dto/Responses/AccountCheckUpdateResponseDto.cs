namespace Common.Dto.Responses
{
    public class AccountCheckUpdateResponseDto : ResponseDtoBase
    {
        public bool AccountNameExists { get; set; }
        public bool AccountEmailExists { get; set; }
    }
}
