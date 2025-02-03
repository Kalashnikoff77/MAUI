using Data.Dto.Views;
using Data.Enums;

namespace Data.Dto.Sp
{
    public class LastMessagesForAccountSpDto : DtoBase
    {
        public EnumMessages Type { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public string? Text { get; set; }

        public AccountsViewDto? Sender { get; set; }
        public AccountsViewDto? Recipient { get; set; }

        public int? UnreadMessages { get; set; }
    }
}
