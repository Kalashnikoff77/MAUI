var _dotNetReference;

export async function GetPreviousMessages(dotNetObject) {
    _dotNetReference = dotNetObject; // Сохраним ссылку на C#
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // Получим сообщения
    $('#Scroll').prepend(result); // Добавим полученные сообщения в окно
    window.ScrollDivToBottom('Scroll'); // Прокрутим окно в самый низ
    $('#Scroll').on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    if (document.getElementById('Scroll').scrollTop < 250) {
        $('#Scroll').off('scroll'); // Временно отключим обработчик
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // Получим новые сообщения
        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
            $('#Scroll').prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// Добавление новых сообщений
export async function AppendNewMessages(messages) {
    if (messages != '' && messages != null) { // Если есть сообщения, то добавляем их
        $('#Scroll').append(messages);
        window.ScrollDivToBottom('Scroll'); // Прокрутим окно в самый низ
    }
}


export function MarkMessageAsRead(id, htmlItem) {
    $('#id_' + id).replaceWith(htmlItem);
}
