using System.ComponentModel.DataAnnotations;

namespace RegisterLoginJWT.Models.DTOs.User
{
    public class UserCreateDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } 

        public List<string>? Roles { get; set; } 
    }

}
