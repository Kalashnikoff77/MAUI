using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnUpdateAccountRelationResponse : SignalRModelBase<OnUpdateAccountRelationResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateAccountRelation;

    }
}
