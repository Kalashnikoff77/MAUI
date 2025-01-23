using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnUpdateLastMessagesResponse : SignalRModelBase<OnUpdateLastMessagesResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateLastMessagesClient;
    }
}
