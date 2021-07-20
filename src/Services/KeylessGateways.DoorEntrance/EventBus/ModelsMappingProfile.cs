using AutoMapper;
using KeylessGateways.DoorEntrance.Data;
using KeylessGateways.Services.Shared.EventBus;

namespace KeylessGateways.DoorEntrance.EventBus
{
    public class ModelsMappingProfile : Profile
    {
        public ModelsMappingProfile()
        {
            CreateMap<UserDoorCreatedUpdatedEvent, UserDoor>();
        }
    }
}