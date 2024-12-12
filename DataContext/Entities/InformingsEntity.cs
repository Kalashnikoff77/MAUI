namespace DataContext.Entities
{
    public class InformingsEntity : EntityBase
    {
        public int AccountId { get; set; }

        public bool IsNewNotification { get; set; }

        public bool IsNewMessage { get; set; }
    }
}
