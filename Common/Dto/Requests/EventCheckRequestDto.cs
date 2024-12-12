namespace Common.Dto.Requests
{
    public class EventCheckRequestDto : RequestDtoBase
    {
        public override string Uri => "/Events/Check";

        // Если передан EventId, значит, проводится проверка при обновлении этого мероприятия, иначе при добавлении
        public int? EventId { get; set; }

        public string? EventName { get; set; }
    }
}
