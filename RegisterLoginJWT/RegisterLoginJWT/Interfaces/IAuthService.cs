using RegisterLoginJWT.Models;
using RegisterLoginJWT.Models.DTOs.Auth;

namespace RegisterLoginJWT.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegisterDTO registerDTO);
        Task<ServiceResponse<string>> Login(UserLoginDTO loginDTO);
    }
}
