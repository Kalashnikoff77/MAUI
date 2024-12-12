namespace Common.Dto
{
    public class UsersDto : DtoBase
    {
        public Guid Guid { get; set; }

        public string Name { get; set; } = null!;

        public short Height { get; set; }

        public short Weight { get; set; }

        // MudBlazor не работает с short.
        public int Gender { get; set; }

        public string? About { get; set; }

        public DateTime BirthDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
