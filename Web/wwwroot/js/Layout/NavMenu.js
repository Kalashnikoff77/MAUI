var _dotNetReference;

export async function Initialize(dotNetReference) {
    _dotNetReference = dotNetReference;
}

export async function GetMessagesCount() {
    var result = await _dotNetReference.invokeMethodAsync('GetMessagesCountAsync'); // ������� ���-�� ������������� ���������
    ChangeNumberFadeInOut('#NumberOfMessages', result);
}
