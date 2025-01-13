var _dotNetObject;

async function SetScrollEvent(div, dotNetObject) {
    _dotNetObject = dotNetObject;
    var result = await dotNetObject.invokeMethodAsync('GetMessages');
    $('#' + div).prepend(result);

    document.getElementById(div).scrollTo(0, document.getElementById(div).scrollHeight);

    $('#' + div).on('scroll', DivScroller);
}

async function DivScroller(event) {
    var div = event.target.id; // Получим id блока
    if (document.getElementById(div).scrollTop < 250) {
        $('#' + div).off('scroll');
        var result = await _dotNetObject.invokeMethodAsync('GetMessages');
        if (result != '') {
            $('#' + div).prepend(result);
            $('#' + div).on('scroll', DivScroller);
        }
    }
}


// Затухание и появление числа (напр.: изменилось кол-во непрочитанных сообщений в чате)
function ChangeNumberFadeInOut(tagClass, number, isShowZero) {
    var tag = $('.' + tagClass);
    if (tag.text() != number) {
        tag.fadeOut(200, function () {
            tag.text('');
            if (number > 0 || isShowZero) {
                tag.text(number);
                tag.fadeIn(200);
            }
        })
    }
}


// Увеличить число в теге
function IncreaseNumberFadeInOut(tagClass) {
    $('.' + tagClass)
        .each(function () {
            $(this).fadeOut(200, function () {
                var currentNumber = Number($(this).text());
                $(this).text(currentNumber + 1);
                $(this).fadeIn(200);
            });
        })
}

// Уменьшить число в теге
function DecreaseNumberFadeInOut(tagClass, isShowZero) {
    $('.' + tagClass)
        .each(function () {
            $(this).fadeOut(200, function () {
                var currentNumber = Number($(this).text());
                $(this).text('');
                currentNumber--;
                if (currentNumber > 0 || isShowZero) {
                    $(this).text(currentNumber);
                    $(this).fadeIn(200);
                }
            });
        })
}

function ChangeNumberInButtonsFadeInOut(tagClass, number) {
    var tag = $('.' + tagClass);
    if (tag.text() != number) {
        tag.fadeOut(200, function () {
            tag.text('');
            if (number) {
                tag.closest('button')
                    .removeClass('rz-secondary').addClass('rz-danger');
                tag.removeClass('IsHidden');
                tag.text(number);
                tag.fadeIn(200);
            }
            else
            {
                tag.addClass('IsHidden');
                tag.closest('button')
                    .removeClass('rz-danger').addClass('rz-secondary');
            }
        })
    }
}


// Прокрутка div вниз
function ScrollDivToBottom(divId) {
    $('#' + divId).scrollTop($('#' + divId)[0].scrollHeight);
}

function ScrollToElement(elementId) {
    var element = document.getElementById(elementId);
    if (element != null) {
        element.scrollIntoView();
    }
}

function ScrollToElementWithinDiv(elementWithinDivId, divElement) {
    var myElement = document.getElementById(elementWithinDivId);
    if (myElement != null) {
        var topPos = myElement.offsetTop;
        document.getElementById(divElement).scrollTop = topPos;
    }
}

function Redirect(uri) { window.location.replace(uri) }
