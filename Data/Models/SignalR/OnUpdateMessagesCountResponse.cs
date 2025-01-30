using Data.Enums;

namespace Data.Models.SignalR
{
    /// <summary>
    /// Вызывается, когда меняется кол-во непрочитанных сообщений
    /// </summary>
    public class OnUpdateMessagesCountResponse : SignalRModelBase<OnUpdateMessagesCountResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateMessagesCountClient;
    }
}
