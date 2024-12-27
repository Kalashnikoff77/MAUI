using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnMessagesReloadResponse : SignalRModelBase<OnMessagesReloadResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnMessagesReloadClient;
    }
}
