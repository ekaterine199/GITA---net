using RegisterLoginJWTMTO20.Enums;
using RegisterLoginJWTMTO20.Models.DTO_s.Role;

namespace RegisterLoginJWTMTO20.Models.DTO_s.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<RoleDTO> Roles { get; set; }
        public Status Status { get; set; }
    }
}
