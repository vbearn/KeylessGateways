using AutoMapper;
using KeylessGateways.Identity.Data;
using KeylessGateways.Identity.Models;

namespace KeylessGateways.Identity.Models
{
    public class _ModelsMappingProfile : Profile
    {
        public _ModelsMappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserCreateUpdateDto, User>()
                .ForMember(des => des.UserName, opt => opt.MapFrom(i => i.Email));
        }
    }
}