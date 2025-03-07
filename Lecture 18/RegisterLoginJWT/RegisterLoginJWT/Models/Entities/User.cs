using System.ComponentModel.DataAnnotations;

namespace RegisterLoginJWT.Models.Entities
{
    public class User : BaseClass
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt   { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationDate { get; set; }

    }
}
