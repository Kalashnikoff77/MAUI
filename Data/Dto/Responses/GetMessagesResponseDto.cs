using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetMessagesResponseDto : ResponseDtoBase
    {
        /// <summary>
        /// Если запрашивается много сообщений
        /// </summary>
        public List<MessagesDto> Messages { get; set; } = new List<MessagesDto>();

        /// <summary>
        /// Если запрашивается одно сообщение
        /// </summary>
        public MessagesDto? Message { get; set; } = null;

        /// <summary>
        /// Id аккаунтов, чьи сообщения запрашиваются
        /// </summary>
        public Dictionary<int, AccountsViewDto> Accounts { get; set; } = null!;
    }
}
