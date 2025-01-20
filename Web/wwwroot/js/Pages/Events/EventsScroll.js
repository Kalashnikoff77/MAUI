export async function LoadItems() {
    var result = await _dotNetReference.invokeMethodAsync('LoadItems'); // Получим сообщения
    if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
        $('#ScrollItems').append(result); // Добавим полученные сообщения в окно
    };
}

export async function SetScrollEvent() {
    $(window).off('scroll'); // Выключим обработчик на случай, если он включён
    $(window).on('scroll', ScrollEvent); // Установим обработчик события прокрутки заново
}

export function ClearItems() {
    $('#ScrollItems').empty();
}


// Обработчик события прокрутки
async function ScrollEvent(event) {
    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var scrollBottom = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (scrollBottom > -500) {
        $(window).off('scroll'); // Временно отключим обработчик
        var result = await _dotNetReference.invokeMethodAsync('LoadItems'); // Получим новые сообщения
        if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
            $('#ScrollItems').append(result);
            $(window).on('scroll', ScrollEvent);
        }
    }
}
