using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnUpdateMessageResponse : SignalRModelBase<OnUpdateMessageResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateMessageClient;

        public int MessageId { get; set; }
    }
}
