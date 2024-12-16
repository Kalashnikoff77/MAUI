using Data.Dto.Sp;

namespace Data.Dto.Responses
{
    public class GetLastMessagesListResponseDto : ResponseDtoBase
    {
        public List<LastMessagesForAccountSpDto>? LastMessagesList { get; set; }
    }
}
