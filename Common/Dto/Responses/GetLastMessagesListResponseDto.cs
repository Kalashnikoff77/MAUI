using Common.Dto.Sp;

namespace Common.Dto.Responses
{
    public class GetLastMessagesListResponseDto : ResponseDtoBase
    {
        public List<LastMessagesForAccountSpDto>? LastMessagesList { get; set; }
    }
}
