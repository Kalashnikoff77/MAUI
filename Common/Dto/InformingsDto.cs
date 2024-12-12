namespace Common.Dto
{
    public class InformingsDto : DtoBase
    {
        public int AccountId { get; set; }

        public bool IsNewNotification { get; set; } = true;

        public bool IsNewMessage { get; set; } = true;
    }
}
