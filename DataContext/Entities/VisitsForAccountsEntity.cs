namespace DataContext.Entities
{
    public class VisitsForAccountsEntity : EntityBase
    {
        public DateTime CreateDate { get; set; }

        public DateTime LastDate { get; set; }

        public int AccountId { get; set; }
    }
}
