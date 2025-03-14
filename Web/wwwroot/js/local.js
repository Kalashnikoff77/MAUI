﻿// Затухание и появление числа (напр.: изменилось кол-во непрочитанных сообщений в чате)
function ChangeNumberFadeInOut(tag, number, isShowZero) {
    var tg = $(tag);
    if (tg.text() != number) {
        tg.fadeOut(200, function () {
            tg.text('');
            if (number > 0 || isShowZero) {
                tg.text(number);
                tg.fadeIn(200);
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
function ScrollDivToBottom(elementId) {
    if (document.getElementById(elementId) != null) {
        $('#' + elementId).scrollTop($('#' + elementId)[0].scrollHeight);
    }
}

function ScrollToElement(elementId) {
    if (document.getElementById(elementId) != null) {
        elementId.scrollIntoView();
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
