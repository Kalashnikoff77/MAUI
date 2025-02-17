using AutoMapper;
using Data.Models.SignalR;

namespace SignalR
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OnMessagesUpdatedRequest, OnMessagesUpdatedResponse>();
        }
    }
}
