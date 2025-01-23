var _dotNetReference;

export async function GetPreviousMessages(dotNetObject) {
    _dotNetReference = dotNetObject; // �������� ������ �� C#
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // ������� ���������
    $('#Scroll').prepend(result); // ������� ���������� ��������� � ����
    window.ScrollDivToBottom('Scroll'); // ��������� ���� � ����� ���
    $('#Scroll').on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    if (document.getElementById('Scroll').scrollTop < 250) {
        $('#Scroll').off('scroll'); // �������� �������� ����������
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // ������� ����� ���������
        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
            $('#Scroll').prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// ���������� ����� ���������
export async function AppendNewMessages(messages) {
    if (messages != '' && messages != null) { // ���� ���� ���������, �� ��������� ��
        $('#Scroll').append(messages);
        window.ScrollDivToBottom('Scroll'); // ��������� ���� � ����� ���
    }
}


export function MarkMessageAsRead(id, htmlItem) {
    $('#id_' + id).replaceWith(htmlItem);
}
