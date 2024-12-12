namespace DataContext.Entities
{
    public class AccountsWishLists : EntityBase
    {
        public string? Comment { get; set; }

        public DateTime StartDate { get; set; }

        public int AccountId { get; set; }
    }
}
