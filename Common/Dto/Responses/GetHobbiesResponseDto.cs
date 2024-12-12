namespace Common.Dto.Responses
{
    public class GetHobbiesResponseDto : ResponseDtoBase
    {
        public List<HobbiesDto> Hobbies { get; set; } = null!;
    }
}
