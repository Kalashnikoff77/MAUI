var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function ScrollDivToBottom() {
    window.ScrollDivToBottom('ScrollNotifications'); // ��������� ���� � ����� ���
}

export async function LoadItems() {
    $('#ScrollNotifications').off('scroll'); // �������� �������� ����������
    var result = await _dotNetReference.invokeMethodAsync('LoadItemsAsync'); // ������� ���������
    if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
        $('#ScrollNotifications').prepend(result).on('scroll', ScrollEvent);
    }
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    if (document.getElementById('ScrollNotifications').scrollTop < 250) {
        LoadItems();
    }
}
