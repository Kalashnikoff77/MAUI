var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function GetPreviousDiscussions() {
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousDiscussionsAsync'); // Получим сообщения
    $('#Scroll').prepend(result); // Добавим полученные сообщения в окно
    window.ScrollDivToBottom('Scroll'); // Прокрутим окно в самый низ
    $('#Scroll').on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    if (document.getElementById('Scroll').scrollTop < 250) {
        $('#Scroll').off('scroll'); // Временно отключим обработчик
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousDiscussionsAsync'); // Получим новые сообщения
        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
            $('#Scroll').prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// Добавление новых сообщений
export async function AppendNewDiscussions(discussion) {
    if (discussion != '' && discussion != null) { // Если есть сообщения, то добавляем их
        $('#Scroll').append(discussion);
        window.ScrollDivToBottom('Scroll'); // Прокрутим окно в самый низ
    }
}
