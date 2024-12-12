namespace DataContext.Entities
{
    public class IdentitiesEntity : EntityBase
    {
        public int AccountId { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
