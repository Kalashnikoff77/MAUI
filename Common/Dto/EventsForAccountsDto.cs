using Common.Dto.Views;

namespace Common.Dto
{
    public class EventsForAccountsDto : DtoBase
    {
        public short? UserGender { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int TicketCost { get; set; }

        public bool IsPaid { get; set; }

        public AccountsViewDto Account { get; set; } = null!;
    }
}
