var _dotNetReference;

export async function SetScrollEvent(dotNetReference) {
    _dotNetReference = dotNetReference; // Сохраним ссылку на C#
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // Получим сообщения
    $('#ScrollItems').prepend(result); // Добавим полученные сообщения в окно
    window.ScrollDivToBottom('ScrollItems'); // Прокрутим окно в самый низ
    $('#ScrollItems').on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    if (document.getElementById('ScrollItems').scrollTop < 250) {
        $('#ScrollItems').off('scroll'); // Временно отключим обработчик
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // Получим новые сообщения
        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
            $('#ScrollItems').prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// Добавление новых сообщений
export async function AppendNewMessages(messages) {
    if (messages != '' && messages != null) { // Если есть сообщения, то добавляем их
        $('#ScrollItems').append(messages);
        window.ScrollDivToBottom('ScrollItems'); // Прокрутим окно в самый низ
    }
}
