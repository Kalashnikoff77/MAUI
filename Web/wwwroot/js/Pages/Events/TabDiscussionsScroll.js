var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function ScrollDivToBottom() {
    window.ScrollDivToBottom('ScrollDiscussions'); // ��������� ���� � ����� ���
}

export async function LoadDiscussions() {
    $('#ScrollDiscussions').off('scroll'); // �������� �������� ����������
    var result = await _dotNetReference.invokeMethodAsync('LoadDiscussionsAsync'); // ������� ���������
    if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
        $('#ScrollDiscussions').prepend(result).on('scroll', ScrollEvent);
    }
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    if (document.getElementById('ScrollDiscussions').scrollTop < 250) {
        LoadDiscussions();
    }
}

// ���������� ����� ���������
export async function AppendNewDiscussions(discussions) {
    if (discussions != '' && discussions != null) { // ���� ���� ���������, �� ��������� ��
        $('#ScrollDiscussions').append(discussions);
        window.ScrollDivToBottom('ScrollDiscussions'); // ��������� ���� � ����� ���
    }
}
