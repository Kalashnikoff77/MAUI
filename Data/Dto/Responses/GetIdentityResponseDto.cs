namespace Data.Dto.Responses
{
    public class GetIdentityResponseDto : ResponseDtoBase
    {
        public IdentitiesDto Identity { get; set; } = null!;
    }
}
