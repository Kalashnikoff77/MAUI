export async function LoadItems() {
    var result = await _dotNetReference.invokeMethodAsync('LoadItems'); // Получим сообщения
    $('#ScrollItems').append(result); // Добавим полученные сообщения в окно
}

export async function SetScrollEvent() {
    $(window).on('scroll', ScrollEvent); // Установим обработчик события прокрутки
}


// Обработчик события прокрутки
async function ScrollEvent(event) {
    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var result = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (result > -500) {
        $(window).off('scroll'); // Временно отключим обработчик
        var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // Получим новые сообщения
        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
            $('#ScrollItems').append(result);
            $(window).on('scroll', ScrollEvent);
        }
    }
}
