namespace Common.Dto
{
    public class RelationsForAccountsDto : DtoBase
    {
        public DateTime CreateDate { get; set; }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        // EnumRelations - текущие отношения между пользователями (заблокирован, подписан, дружат)
        public short Type { get; set; }

        // Подтвердил ли пользователь запрос (например, на дружбу)
        public bool IsConfirmed { get; set; }
    }
}
