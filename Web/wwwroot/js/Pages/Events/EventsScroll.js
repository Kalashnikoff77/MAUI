var _dotNetObject;

export async function SetScrollEvent(div, dotNetObject) {
    _dotNetObject = dotNetObject; // Сохраним ссылку на C#
    var result = await _dotNetObject.invokeMethodAsync('GetNextSchedules'); // Получим сообщения
    console.log(result);
    $('#' + div).append(result); // Добавим полученные сообщения в окно
    $('#' + div).on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    var div = event.target.id; // Получим id блока
    if (document.getElementById(div).scrollTop < 250) {
//        $('#' + div).off('scroll'); // Временно отключим обработчик
//        var result = await _dotNetObject.invokeMethodAsync('GetNextSchedules'); // Получим новые сообщения
//        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
//            $('#' + div).append(result).on('scroll', DivScroller);
//        }
        console.log('EVENTS - ScrollEvent');
    }
}

// Добавление новых сообщений
export async function AppendNewSchedules(div, messages) {
    if (messages != '' && messages != null) { // Если есть сообщения, то добавляем их
        $('#' + div).append(messages);
        window.ScrollDivToBottom(div); // Прокрутим окно в самый низ
    }
}
