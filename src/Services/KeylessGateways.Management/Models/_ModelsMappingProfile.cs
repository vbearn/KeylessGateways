using AutoMapper;
using KeylessGateways.Management.Data;
using KeylessGateways.Services.Shared.EventBus;

namespace KeylessGateways.Management.Models
{
    public class _ModelsMappingProfile : Profile
    {
        public _ModelsMappingProfile()
        {
            CreateMap<Door, DoorDto>();
            CreateMap<DoorUpdateDto, Door>();

            CreateMap<UserDoor, UserDoorDto>();
            CreateMap<UserDoorCreateUpdateDto, UserDoor>();
            
        }
    }
}