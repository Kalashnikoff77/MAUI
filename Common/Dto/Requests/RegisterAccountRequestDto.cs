namespace Common.Dto.Requests
{
    public class RegisterAccountRequestDto : AccountRequestDtoBase
    {
        public override string Uri => "/Accounts/Register";

        public bool AcceptTerms { get; set; } = true;
    }
}
