using Data.Dto;
using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnMarkMessagesAsReadResponse : SignalRModelBase<OnMarkMessagesAsReadResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnMarkMessagesAsReadClient;

        public IEnumerable<MessagesDto> Messages { get; set; } = null!;
    }
}
