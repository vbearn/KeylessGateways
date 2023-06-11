using AutoMapper;
using KeylessGateways.DoorEntrance.Data;

namespace KeylessGateways.DoorEntrance.Models
{
    public class _ModelsMappingProfile : Profile
    {
        public _ModelsMappingProfile()
        {

            CreateMap<DoorEntranceHistory, DoorEntranceHistoryDto>();


           
        }
    }
}