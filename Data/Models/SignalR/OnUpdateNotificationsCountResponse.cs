using Data.Enums;

namespace Data.Models.SignalR
{
    /// <summary>
    /// Вызывается, когда меняется кол-во непрочитанных уведомлений
    /// </summary>
    public class OnUpdateNotificationsCountResponse : SignalRModelBase<OnUpdateNotificationsCountResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnUpdateNotificationsCountClient;
    }
}
