var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function ScrollDivToBottom() {
    window.ScrollDivToBottom('ScrollNotifications'); // Прокрутим окно в самый низ
}

export async function LoadItems() {
    $('#ScrollNotifications').off('scroll'); // Временно отключим обработчик
    var result = await _dotNetReference.invokeMethodAsync('LoadItemsAsync'); // Получим сообщения
    if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
        $('#ScrollNotifications').prepend(result).on('scroll', ScrollEvent);
    }
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    if (document.getElementById('ScrollNotifications').scrollTop < 250) {
        LoadItems();
    }
}
