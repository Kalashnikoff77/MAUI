var _dotNetReference;

export async function SetScrollEvent(div, dotNetReference) {
    _dotNetReference = dotNetReference; // �������� ������ �� C#
    var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // ������� ���������
    $('#' + div).append(result); // ������� ���������� ��������� � ����
    $('#' + div).on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    var div = event.target.id; // ������� id �����
    if (document.getElementById(div).scrollTop < 250) {
//        $('#' + div).off('scroll'); // �������� �������� ����������
//        var result = await _dotNetObject.invokeMethodAsync('GetNextSchedules'); // ������� ����� ���������
//        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
//            $('#' + div).append(result).on('scroll', DivScroller);
//        }
        console.log('EVENTS - ScrollEvent');
    }
}

// ���������� ����� ���������
export async function AppendNewSchedules(div, messages) {
    if (messages != '' && messages != null) { // ���� ���� ���������, �� ��������� ��
        $('#' + div).append(messages);
        window.ScrollDivToBottom(div); // ��������� ���� � ����� ���
    }
}
