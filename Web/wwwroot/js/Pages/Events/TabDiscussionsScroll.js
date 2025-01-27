var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function GetPreviousDiscussions() {
    var result = await _dotNetReference.invokeMethodAsync('GetPreviousDiscussionsAsync'); // ������� ���������
    $('#Scroll').prepend(result); // ������� ���������� ��������� � ����
    window.ScrollDivToBottom('Scroll'); // ��������� ���� � ����� ���
    $('#Scroll').on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    if (document.getElementById('Scroll').scrollTop < 250) {
        $('#Scroll').off('scroll'); // �������� �������� ����������
        var result = await _dotNetReference.invokeMethodAsync('GetPreviousDiscussionsAsync'); // ������� ����� ���������
        if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
            $('#Scroll').prepend(result).on('scroll', ScrollEvent);
        }
    }
}

// ���������� ����� ���������
export async function AppendNewDiscussions(discussion) {
    if (discussion != '' && discussion != null) { // ���� ���� ���������, �� ��������� ��
        $('#Scroll').append(discussion);
        window.ScrollDivToBottom('Scroll'); // ��������� ���� � ����� ���
    }
}
