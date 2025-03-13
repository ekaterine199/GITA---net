using AutoMapper;
using RegisterLoginJWT.Models.DTOs.Role;
using RegisterLoginJWT.Models.Entities;

namespace RegisterLoginJWT
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoleCreateDTO, Role>().ReverseMap();
            CreateMap<RoleUpdateDTO, Role>().ReverseMap();
            CreateMap<RoleDTO, Role>().ReverseMap();

        }

    }
}
