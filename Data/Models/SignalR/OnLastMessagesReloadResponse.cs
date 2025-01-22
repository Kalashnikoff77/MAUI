using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnLastMessagesReloadResponse : SignalRModelBase<OnLastMessagesReloadResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnLastMessagesReloadClient;
    }
}
