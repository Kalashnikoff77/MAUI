namespace Common.Dto.Responses
{
    public class GetFeaturesResponseDto : ResponseDtoBase
    {
        public List<FeaturesDto> Features { get; set; } = null!;
    }
}
