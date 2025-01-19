var _dotNetReference;

export function SetDotNetReference(dotNetReference) {
    _dotNetReference = dotNetReference; // �������� ������ �� C#
}

export async function LoadData() {
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // ������� ���������
    $('#ScrollItems').prepend(result); // ������� ���������� ��������� � ����
    window.ScrollDivToBottom('ScrollItems'); // ��������� ���� � ����� ���
}

export async function SetScrollEvent() {
    $(window).on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    if (document.getElementById('ScrollItems').scrollTop < 250) {
        $('#ScrollItems').off('scroll'); // �������� �������� ����������
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousMessages'); // ������� ����� ���������
        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
            $('#ScrollItems').prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// ���������� ����� ���������
export async function AppendNewMessages(messages) {
    if (messages != '' && messages != null) { // ���� ���� ���������, �� ��������� ��
        $('#ScrollItems').append(messages);
        window.ScrollDivToBottom('ScrollItems'); // ��������� ���� � ����� ���
    }
}
