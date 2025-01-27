var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function ScrollDivToBottom() {
    window.ScrollDivToBottom('ScrollDiscussions'); // Прокрутим окно в самый низ
}

export async function LoadDiscussions() {
    $('#ScrollDiscussions').off('scroll'); // Временно отключим обработчик
    var result = await _dotNetReference.invokeMethodAsync('LoadDiscussionsAsync'); // Получим сообщения
    if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
        $('#ScrollDiscussions').prepend(result).on('scroll', ScrollEvent);
    }
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    if (document.getElementById('ScrollDiscussions').scrollTop < 250) {
        LoadDiscussions();
    }
}

// Добавление новых сообщений
export async function AppendNewDiscussions(discussions) {
    if (discussions != '' && discussions != null) { // Если есть сообщения, то добавляем их
        $('#ScrollDiscussions').append(discussions);
        window.ScrollDivToBottom('ScrollDiscussions'); // Прокрутим окно в самый низ
    }
}
