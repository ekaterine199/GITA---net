namespace RegisterLoginJWT.Models.DTOs.Auth
{
    public class UserLoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool StaySignedIn { get; set; }
    }
}
