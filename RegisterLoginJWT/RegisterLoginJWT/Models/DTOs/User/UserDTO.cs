namespace RegisterLoginJWT.Models.DTOs.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; } // Convert Role objects to a list of role names

    }
}
