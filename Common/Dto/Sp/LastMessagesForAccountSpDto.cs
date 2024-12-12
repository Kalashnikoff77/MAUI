using Common.Dto.Views;

namespace Common.Dto.Sp
{
    public class LastMessagesForAccountSpDto : DtoBase
    {
        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public string? Text { get; set; }

        public AccountsViewDto? Sender { get; set; }
        public AccountsViewDto? Recipient { get; set; }

        public int? UnreadMessages { get; set; }
    }
}
