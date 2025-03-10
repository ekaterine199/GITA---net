using System.ComponentModel.DataAnnotations;

namespace RegisterLoginJWT.Models.DTOs.User
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; } // Optional update

        [MinLength(6)]
        public string? NewPassword { get; set; } // Optional password update

        public List<string>? Roles { get; set; } // Optional role update (for admins)
    }

}
