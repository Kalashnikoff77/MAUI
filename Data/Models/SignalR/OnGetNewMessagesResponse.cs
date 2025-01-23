using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnGetNewMessagesResponse : SignalRModelBase<OnGetNewMessagesResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnGetNewMessagesClient;
    }
}
