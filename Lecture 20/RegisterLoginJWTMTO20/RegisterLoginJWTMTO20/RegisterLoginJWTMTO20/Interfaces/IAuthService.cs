using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.Auth;

namespace RegisterLoginJWTMTO20.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserRegisterDTO registerDTO);
        Task<ServiceResponse<string>> Login(UserLoginDTO loginDTO);
    }
}
