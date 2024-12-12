namespace DataContext.Entities
{
    public class UsersEntity : EntityBase
    {
        public Guid Guid { get; set; }

        public string Name { get; set; } = null!;

        public short Height { get; set; }
        public short Weight { get; set; }

        public short Gender { get; set; }

        public string? About { get; set; }

        public DateTime BirthDate { get; set; }

        public int AccountId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
