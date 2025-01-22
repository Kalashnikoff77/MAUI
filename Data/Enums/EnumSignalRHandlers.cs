namespace Data.Enums
{
    public enum EnumSignalRHandlers
    {
        /// <summary>
        /// Пользователь залогинился
        /// </summary>
        UpdateOnlineAccountsClient,

        /// <summary>
        /// При добавлении обсуждения в мероприятие
        /// </summary>
        OnScheduleChangedClient,

        /// <summary>
        /// Пользователь изменил свой аватар
        /// </summary>
        OnAvatarChangedClient,

        /// <summary>
        /// Обновить сообщения у двух пользователей (диалоговое окно страницы /messages)
        /// </summary>
        OnMessagesReloadClient,

        /// <summary>
        /// Обновить список последних сообщений у двух пользователей (страница /messages)
        /// </summary>
        OnLastMessagesReloadClient,


        // Требуется проверить, используются ли эти перечисления
        //NewMessageAddedServer,
        //NewMessageAddedClient,

        //NewEventDiscussionAddedServer,
        //NewEventDiscussionAddedClient,

        //NewNotificationAddedServer,
        //NewNotificationAddedClient,

        //UpdateMessagesCountServer,
        //UpdateMessagesCountClient,

        //UpdateNotificationsCountServer,
        //UpdateNotificationsCountClient,

        //UpdateOnlineAccountsClient,

        //AvatarChangedServer,
        //AvatarChangedClient,

        //UpdateRelationsServer,
        //GetRelationsClient,
        //GetRelationsServer
    }
}
