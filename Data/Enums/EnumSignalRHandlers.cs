namespace Data.Enums
{
    public enum EnumSignalRHandlers
    {
        /// <summary>
        /// При добавлении обсуждения в мероприятие
        /// </summary>
        OnScheduleChangedClient,

        /// <summary>
        /// Пользователь изменил свой аватар
        /// </summary>
        OnAvatarChangedClient,

        /// <summary>
        /// Добавлено новое сообщение в диалогах
        /// </summary>
        OnMessagesReloadClient,



        // Требуется проверить, используются ли эти перечисления
        NewMessageAddedServer,
        NewMessageAddedClient,

        NewEventDiscussionAddedServer,
        NewEventDiscussionAddedClient,

        NewNotificationAddedServer,
        NewNotificationAddedClient,

        UpdateMessagesCountServer,
        UpdateMessagesCountClient,

        UpdateNotificationsCountServer,
        UpdateNotificationsCountClient,

        UpdateOnlineAccountsClient,

        AvatarChangedServer,
        AvatarChangedClient,

        UpdateRelationsServer,
        GetRelationsClient,
        GetRelationsServer
    }
}
