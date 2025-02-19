using Data.Dto.Views;

namespace Data.Models
{
    public class RelationInfo
    {
        /// <summary>
        /// Инициатор (отправитель)
        /// </summary>
        public AccountsViewDto Sender { get; set; } = null!;
        
        /// <summary>
        /// Получатель
        /// </summary>
        public AccountsViewDto Recipient { get; set; } = null!;
        
        /// <summary>
        /// Подтверждена ли связь?
        /// </summary>
        public bool IsConfirmed { get; set; } = false;
    }
}
