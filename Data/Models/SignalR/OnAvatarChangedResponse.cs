using Data.Dto;
using Data.Enums;

namespace Data.Models.SignalR
{
    public class OnAvatarChangedResponse : SignalRModelBase<OnAvatarChangedResponse>
    {
        public override EnumSignalRHandlers EnumSignalRHandlersClient => EnumSignalRHandlers.OnAvatarChangedClient;

        public PhotosForAccountsDto NewAvatar { get; set; } = null!;
    }
}
