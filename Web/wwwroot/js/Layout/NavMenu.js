var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function GetNotificationsCount() {
    var result = await _dotNetReference.invokeMethodAsync('GetNotificationsCountAsync'); // ѕолучим кол-во непрочитанных уведомлений
    ChangeNumberFadeInOut('#NumberOfNotifications', result);
}

export async function GetMessagesCount() {
    var result = await _dotNetReference.invokeMethodAsync('GetMessagesCountAsync'); // ѕолучим кол-во непрочитанных сообщений
    ChangeNumberFadeInOut('#NumberOfMessages', result);
}
