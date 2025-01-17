var _dotNetReference;

export async function SetScrollEvent(div, dotNetReference) {
    _dotNetReference = dotNetReference; // Сохраним ссылку на C#
    var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // Получим сообщения
    $('#ScrollItems').append(result); // Добавим полученные сообщения в окно
    $(window).on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    var div = event.target.id; // Получим id блока

    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var result = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (result > -1000) {
    //if (document.getElementById(div).scrollTop + document.getElementById(div).height < 250) {
//        $('#' + div).off('scroll'); // Временно отключим обработчик
//        var result = await _dotNetObject.invokeMethodAsync('GetNextSchedules'); // Получим новые сообщения
//        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
//            $('#' + div).append(result).on('scroll', DivScroller);
//        }
        console.log('EVENTS - ScrollEvent - 3');
    }
}

// Добавление новых сообщений
export async function AppendNewSchedules(div, messages) {
    if (messages != '' && messages != null) { // Если есть сообщения, то добавляем их
        $('#' + div).append(messages);
        window.ScrollDivToBottom(div); // Прокрутим окно в самый низ
    }
}
