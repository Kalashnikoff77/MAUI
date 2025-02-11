using Data.Enums;

namespace Data.Dto
{
    public class RelationsForAccountsDto : DtoBase
    {
        public DateTime CreateDate { get; set; }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        // Текущие отношения между пользователями (заблокирован, дружат)
        public EnumRelations Type { get; set; }

        // Подтвердил ли пользователь запрос (например, на дружбу)
        public bool IsConfirmed { get; set; }
    }
}
