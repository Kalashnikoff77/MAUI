﻿using Data.Dto.Views;

namespace Data.Dto
{
    public class MessagesDto : DtoBase
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public string Text { get; set; } = null!;
    }
}
