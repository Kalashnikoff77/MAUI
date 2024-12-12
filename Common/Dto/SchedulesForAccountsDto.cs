namespace Common.Dto
{
    public class SchedulesForAccountsDto : DtoBase
    {
        public int ScheduleId { get; set; }
        public int AccountId { get; set; }
        public short? AccountGender { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int TicketCost { get; set; }
        public bool IsPaid { get; set; }
    }
}
