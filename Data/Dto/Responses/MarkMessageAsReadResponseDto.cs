using Data.Dto.Sp;

namespace Data.Dto.Responses
{
    public class MarkMessageAsReadResponseDto : ResponseDtoBase
    {
        public LastMessagesForAccountSpDto? UpdatedMessage { get; set; }
    }
}
