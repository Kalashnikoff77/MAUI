﻿using Data.Dto.Views;

namespace Data.Dto.Responses
{
    public class GetMessagesResponseDto : ResponseDtoBase
    {
        public AccountsViewDto Sender { get; set; } = null!;
        public AccountsViewDto Recipient { get; set; } = null!;

        public List<MessagesDto> Messages { get; set; } = null!;
    }
}
