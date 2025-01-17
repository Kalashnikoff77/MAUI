var _dotNetReference;

export async function SetScrollEvent(dotNetReference) {
    _dotNetReference = dotNetReference; // �������� ������ �� C#
    var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // ������� ���������
    $('#ScrollItems').append(result); // ������� ���������� ��������� � ����
    $(window).on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var result = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (result > -500) {
        $(window).off('scroll'); // �������� �������� ����������
        var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // ������� ����� ���������
        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
            $('#ScrollItems').append(result);
            $(window).on('scroll', ScrollEvent);
        }
    }
}
