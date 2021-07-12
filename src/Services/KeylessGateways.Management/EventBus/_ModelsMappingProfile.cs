using AutoMapper;
using KeylessGateways.Management.Data;
using KeylessGateways.Services.Shared.EventBus;

namespace KeylessGateways.Management.EventBus
{
    public class _ModelsMappingProfile : Profile
    {
        public _ModelsMappingProfile()
        {
            CreateMap<UserDoor, UserDoorCreatedEvent>();
            CreateMap<UserDoor, UserDoorUpdatedEvent>();
            CreateMap<UserDoor, UserDoorDeletedEvent>();
            
        }
    }
}