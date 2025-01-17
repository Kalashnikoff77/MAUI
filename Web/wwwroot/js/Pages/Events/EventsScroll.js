var _dotNetReference;

export async function SetScrollEvent(div, dotNetReference) {
    _dotNetReference = dotNetReference; // Сохраним ссылку на C#
    var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // Получим сообщения
    $('#ScrollItems').append(result); // Добавим полученные сообщения в окно
    $('#' + div).on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    var div = event.target.id; // Получим id блока

    //console.log('Height: ' + h + ' - FromTop: ' + $('#' + div).scrollTop());
    //console.log($('#Scroll').scrollTop() + '-' + $('#ScrollItems').height());

    //console.log($('#ScrollItems').height() + ' - ' + $('#Scroll').scrollTop() + ' = ' + ($('#ScrollItems').height() - $('#Scroll').scrollTop()));

    var scrollTop = $(window).scrollTop();
    var scrollBottom = $('#' + div).height() - $(window).height() - scrollTop;
    console.log($('#ScrollItems').height() + ' - ' + $('#Scroll').scrollTop());

    if (($('#ScrollItems').outerHeight() - $('#Scroll').scrollTop()) < 500) {
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
