var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function GetNotificationsCount() {
    var result = await _dotNetReference.invokeMethodAsync('GetNotificationsCountAsync'); // ѕолучим кол-во непрочитанных уведомлений
    ChangeNumberFadeInOut('#NumberOfNotifications', result);
}
