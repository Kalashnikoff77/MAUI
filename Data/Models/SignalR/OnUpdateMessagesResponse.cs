using Data.Enums;

namespace Data.Models.SignalR
{
    /// <summary>
    /// Вызывается, когда меняется кол-во непрочитанных сообщений
    /// </summary>
    public class OnUpdateMessagesResponse : SignalRModelBase<OnUpdateMessagesResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateMessagesClient;
    }
}
