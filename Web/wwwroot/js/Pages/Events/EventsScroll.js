var _dotNetReference;

export async function SetScrollEvent(div, dotNetReference) {
    _dotNetReference = dotNetReference; // �������� ������ �� C#
    var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // ������� ���������
    $('#ScrollItems').append(result); // ������� ���������� ��������� � ����
    $(window).on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    var div = event.target.id; // ������� id �����

    var winHeight = $(window).height();
    var scrollHeight = $('#Scroll').height();
    var scrollTop = $(window).scrollTop();
    var result = winHeight - (scrollHeight - scrollTop) - 64 - 48;

    if (result > -1000) {
    //if (document.getElementById(div).scrollTop + document.getElementById(div).height < 250) {
//        $('#' + div).off('scroll'); // �������� �������� ����������
//        var result = await _dotNetObject.invokeMethodAsync('GetNextSchedules'); // ������� ����� ���������
//        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
//            $('#' + div).append(result).on('scroll', DivScroller);
//        }
        console.log('EVENTS - ScrollEvent - 3');
    }
}

// ���������� ����� ���������
export async function AppendNewSchedules(div, messages) {
    if (messages != '' && messages != null) { // ���� ���� ���������, �� ��������� ��
        $('#' + div).append(messages);
        window.ScrollDivToBottom(div); // ��������� ���� � ����� ���
    }
}
