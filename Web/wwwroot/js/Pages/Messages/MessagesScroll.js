var _dotNetReference;

export async function SetScrollEvent(div, dotNetObject) {
    _dotNetReference = dotNetObject; // Сохраним ссылку на C#
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // Получим сообщения
    $('#' + div).prepend(result); // Добавим полученные сообщения в окно
    window.ScrollDivToBottom(div); // Прокрутим окно в самый низ
    $('#' + div).on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    var div = event.target.id; // Получим id блока
    if (document.getElementById(div).scrollTop < 250) {
        $('#' + div).off('scroll'); // Временно отключим обработчик
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // Получим новые сообщения
        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
            $('#' + div).prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// Добавление новых сообщений
export async function AppendNewMessages(div, messages) {
    if (messages != '' && messages != null) { // Если есть сообщения, то добавляем их
        $('#' + div).append(messages);
        window.ScrollDivToBottom(div); // Прокрутим окно в самый низ
    }
}
