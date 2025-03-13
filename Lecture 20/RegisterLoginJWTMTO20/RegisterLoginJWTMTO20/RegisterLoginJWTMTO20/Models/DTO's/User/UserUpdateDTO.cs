using RegisterLoginJWTMTO20.Enums;

namespace RegisterLoginJWTMTO20.Models.DTO_s.User
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<int> RoleIds { get; set; }
        public Status Status { get; set; }
    }
}
