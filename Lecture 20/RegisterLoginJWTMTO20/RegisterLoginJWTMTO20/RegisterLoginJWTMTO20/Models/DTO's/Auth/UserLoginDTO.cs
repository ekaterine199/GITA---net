namespace RegisterLoginJWTMTO20.Models.DTO_s.Auth
{
    public class UserLoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool StaySignedIn { get; set; }
    }
}
