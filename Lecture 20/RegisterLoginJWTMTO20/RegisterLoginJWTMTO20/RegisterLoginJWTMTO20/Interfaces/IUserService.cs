using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.User;

namespace RegisterLoginJWTMTO20.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UserDTO>>> GetAllAsync();
        Task<ServiceResponse<UserDTO>> GetByIdAsync(int id);
        Task<ServiceResponse<string>> CreateAsync(UserCreateDTO dto);
        Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
}
