var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
    SetDotNetReferenceInEventsRazor(dotNetReference);
}

export async function LoadItems() {
    $(window).off('scroll');
    var result = await _dotNetReference.invokeMethodAsync('LoadItemsAsync'); // Получим сообщения
    if (result != '') { // Если ещё есть сообщения, то добавляем их и включаем обработчик снова
        $('#ScrollItems').append(result); // Добавим полученные сообщения в окно
        $(window).on('scroll', ScrollEvent);
    };
}

// Обработчик события прокрутки
async function ScrollEvent(event) {
    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var scrollBottom = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (scrollBottom > -500) { LoadItems(); }
}

export function ClearItems() { $('#ScrollItems').empty(); }

export function ReplaceItem(id, htmlItem) { $('#id_' + id).replaceWith(htmlItem); }
