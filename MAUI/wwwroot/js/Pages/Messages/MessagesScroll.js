var _dotNetObject;

export async function SetScrollEvent(div, dotNetObject) {
    _dotNetObject = dotNetObject; // �������� ������ �� C#
    var result = await _dotNetObject.invokeMethodAsync('GetPreviousMessages'); // ������� ���������
    $('#' + div).prepend(result); // ������� ���������� ��������� � ����
    window.ScrollDivToBottom(div); // ��������� ���� � ����� ���
    $('#' + div).on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    var div = event.target.id; // ������� id �����
    if (document.getElementById(div).scrollTop < 250) {
        $('#' + div).off('scroll'); // �������� �������� ����������
        var result = await _dotNetObject.invokeMethodAsync('GetPreviousMessages'); // ������� ����� ���������
        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
            $('#' + div).prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// ���������� ����� ���������
export async function AppendNewMessages(div, messages) {
    if (messages != '' && messages != null) { // ���� ���� ���������, �� ��������� ��
        $('#' + div).append(messages);
        window.ScrollDivToBottom(div); // ��������� ���� � ����� ���
    }
}
