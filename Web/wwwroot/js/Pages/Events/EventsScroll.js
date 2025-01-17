var _dotNetReference;

export async function SetScrollEvent(div, dotNetReference) {
    _dotNetReference = dotNetReference; // �������� ������ �� C#
    var result = await _dotNetReference.invokeMethodAsync('GetNextSchedules'); // ������� ���������
    $('#ScrollItems').append(result); // ������� ���������� ��������� � ����
    $('#' + div).on('scroll', ScrollEvent); // ��������� ���������� ������� ���������
}

// ���������� ������� ���������
async function ScrollEvent(event) {
    var div = event.target.id; // ������� id �����

    //console.log('Height: ' + h + ' - FromTop: ' + $('#' + div).scrollTop());
    //console.log($('#Scroll').scrollTop() + '-' + $('#ScrollItems').height());

    //console.log($('#ScrollItems').height() + ' - ' + $('#Scroll').scrollTop() + ' = ' + ($('#ScrollItems').height() - $('#Scroll').scrollTop()));

    var scrollTop = $(window).scrollTop();
    var scrollBottom = $('#' + div).height() - $(window).height() - scrollTop;
    console.log($('#ScrollItems').height() + ' - ' + $('#Scroll').scrollTop());

    if (($('#ScrollItems').outerHeight() - $('#Scroll').scrollTop()) < 500) {
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
