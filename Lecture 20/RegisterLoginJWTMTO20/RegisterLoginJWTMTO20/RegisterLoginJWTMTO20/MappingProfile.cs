using AutoMapper;
using RegisterLoginJWTMTO20.Models.DTO_s.Role;
using RegisterLoginJWTMTO20.Models.DTO_s.User;
using RegisterLoginJWTMTO20.Models.Entities;

namespace RegisterLoginJWTMTO20
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoleCreateDTO, Role>().ReverseMap();
            CreateMap<RoleUpdateDTO, Role>().ReverseMap();
            CreateMap<RoleDTO, Role>().ReverseMap();

            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<UserCreateDTO, User>().ReverseMap();
            CreateMap<UserUpdateDTO, User>().ReverseMap();
        }
    }
}
