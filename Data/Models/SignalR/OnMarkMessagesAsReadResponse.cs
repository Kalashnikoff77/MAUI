using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnMarkMessagesAsReadResponse : SignalRModelBase<OnMarkMessagesAsReadResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnMarkMessagesAsReadClient;

        /// <summary>
        /// Список Id сообщений для отметки (null - помечать все)
        /// </summary>
        public IEnumerable<int>? MessagesIds { get; set; } = null!;
    }
}
