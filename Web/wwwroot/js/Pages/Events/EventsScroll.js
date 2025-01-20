export async function LoadItems() {
    var result = await _dotNetReference.invokeMethodAsync('LoadItems'); // ������� ���������
    if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
        $('#ScrollItems').append(result); // ������� ���������� ��������� � ����
    };
}

export async function SetScrollEvent() {
    $(window).off('scroll'); // �������� ���������� �� ������, ���� �� �������
    $(window).on('scroll', ScrollEvent); // ��������� ���������� ������� ��������� ������
}

export function ClearItems() {
    $('#ScrollItems').empty();
}


// ���������� ������� ���������
async function ScrollEvent(event) {
    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var scrollBottom = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (scrollBottom > -500) {
        $(window).off('scroll'); // �������� �������� ����������
        var result = await _dotNetReference.invokeMethodAsync('LoadItems'); // ������� ����� ���������
        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
            $('#ScrollItems').append(result);
            $(window).on('scroll', ScrollEvent);
        }
    }
}
