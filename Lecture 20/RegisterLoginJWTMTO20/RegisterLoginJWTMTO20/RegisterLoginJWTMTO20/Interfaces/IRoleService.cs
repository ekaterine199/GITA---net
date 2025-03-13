using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.Role;

namespace RegisterLoginJWTMTO20.Interfaces
{
    public interface IRoleService
    {
        Task<ServiceResponse<List<RoleDTO>>> GetAllAsync();
        Task<ServiceResponse<RoleDTO>> GetByIdAsync(int id);
        Task<ServiceResponse<string>> CreateAsync(RoleCreateDTO dto);
        Task<ServiceResponse<string>> UpdateAsync(RoleUpdateDTO dto);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
}
