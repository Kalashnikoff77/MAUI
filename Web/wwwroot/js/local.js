//// scrollTop - текущая позиция верхней части прокрутки от начала div
//// scrollHeight - высота окна прокрутки
//// scrollTo (ш, в) - установить позицию прокрутки
//firstTime = true;
//function FreezeScrollBar(tag) {
//    var top = document.getElementById(tag).scrollTop;
//    var height = document.getElementById(tag).scrollHeight;
//    document.getElementById(tag).scrollTo(0, height);
//    console.log(top + ' - ' + height);

//    if (firstTime == true) {
//        $('#' + tag).scroll(function () {
//            var top = document.getElementById(tag).scrollTop;
//            var height = document.getElementById(tag).scrollHeight;
//            console.log('EVENT: Top - ' + top + ', ' + 'Height - ' + height)
//            //document.getElementById(tag).scrollTo(0, top);
//        });
//        firstTime = false;
//    }
//}

//function UnFreezeScrollBar(tag) {
//    var element = $('#' + tag);
//    document.ontouchmove = function (e) {
//        return true;
//    }
//}


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
