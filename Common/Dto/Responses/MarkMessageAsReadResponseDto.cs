using Common.Dto.Sp;

namespace Common.Dto.Responses
{
    public class MarkMessageAsReadResponseDto : ResponseDtoBase
    {
        public LastMessagesForAccountSpDto? UpdatedMessage { get; set; }
    }
}
