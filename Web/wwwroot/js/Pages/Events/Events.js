var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
    SetDotNetReferenceInEventsRazor(dotNetReference);
}

export async function LoadItems() {
    $(window).off('scroll');
    var result = await _dotNetReference.invokeMethodAsync('LoadItemsAsync'); // ������� ���������
    if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
        $('#ScrollEvents').append(result); // ������� ���������� ��������� � ����
        $(window).on('scroll', ScrollEvent);
    };
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var scrollBottom = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (scrollBottom > -500) { LoadItems(); }
}

export function ClearItems() { $('#ScrollEvents').empty(); }

export function ReplaceItem(id, htmlItem) { $('#scheduleid_' + id).replaceWith(htmlItem); }
