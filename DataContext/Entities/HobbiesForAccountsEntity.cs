namespace DataContext.Entities
{
    public class HobbiesForAccountsEntity : EntityBase
    {
        public int AccountId { get; set; }
        public int HobbyId { get; set; }
    }
}
