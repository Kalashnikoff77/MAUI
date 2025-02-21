var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
    SetDotNetReferenceInMessagesDialogRazor(dotNetReference);
}

export async function ScrollDivToBottom() {
    window.ScrollDivToBottom('ScrollMessages'); // ��������� ���� � ����� ���
}

export async function LoadItems() {
    $('#ScrollMessages').off('scroll'); // �������� �������� ����������
    var result = await _dotNetReference.invokeMethodAsync('LoadItemsAsync'); // ������� ���������
    if (result != '') { // ���� ��� ���� ���������, �� ��������� �� � �������� ���������� �����
        $('#ScrollMessages').prepend(result).on('scroll', ScrollEvent);
    }
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    if (document.getElementById('ScrollMessages').scrollTop < 250) {
        LoadItems();
    }
}

// ���������� ����� ���������
export async function AppendNewMessages(messages) {
    if (messages != '' && messages != null) { // ���� ���� ���������, �� ��������� ��
        $('#ScrollMessages').append(messages);
        window.ScrollDivToBottom('ScrollMessages'); // ��������� ���� � ����� ���
    }
}

// �������� ��������� ��� �����������
export function MarkMessageAsRead(id, htmlItem) {
    $('#messageid_' + id).replaceWith(htmlItem);
}

// �������� ���������
export function DeleteMessage(messageId) {
    $('#messageid_' + messageId).remove();
}

// �������� ���� ���������
export function DeleteMessages() {
    $('#ScrollMessages').off('scroll'); // �������� ���������
    $('#ScrollMessages').empty(); // ������� ��� ���������
}
