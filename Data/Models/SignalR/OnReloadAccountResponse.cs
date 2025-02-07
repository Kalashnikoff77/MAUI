using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnReloadAccountResponse : SignalRModelBase<OnReloadAccountResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateAccountRelation;

    }
}
