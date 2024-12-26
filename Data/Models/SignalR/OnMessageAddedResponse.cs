using Data.Dto;
using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnMessageAddedResponse : SignalRModelBase<OnMessageAddedResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnMessageAddedClient;

        public MessagesDto Message { get; set; } = null;
    }
}
