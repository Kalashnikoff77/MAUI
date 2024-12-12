namespace DataContext.Entities
{
    public class SchedulesForAccountsEntity : EntityBase
    {
        public int ScheduleId { get; set; }

        public int AccountId { get; set; }

        public short? AccountGender { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int TicketCost { get; set; }

        public bool IsPaid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
