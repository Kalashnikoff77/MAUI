var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function ScrollDivToBottom() {
    window.ScrollDivToBottom('ScrollMessages'); // Прокрутим окно в самый низ
}

export async function LoadItems() {
    $('#ScrollMessages').off('scroll'); // Временно отключим обработчик
    var result = await _dotNetReference.invokeMethodAsync('LoadItemsAsync'); // Получим сообщения
    if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
        $('#ScrollMessages').prepend(result).on('scroll', ScrollEvent);
    }
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    if (document.getElementById('ScrollMessages').scrollTop < 250) {
        LoadItems();
    }
}

// Добавление новых сообщений
export async function AppendNewMessages(messages) {
    if (messages != '' && messages != null) { // Если есть сообщения, то добавляем их
        $('#ScrollMessages').append(messages);
        window.ScrollDivToBottom('ScrollMessages'); // Прокрутим окно в самый низ
    }
}

export function MarkMessageAsRead(id, htmlItem) {
    $('#messageid_' + id).replaceWith(htmlItem);
}
