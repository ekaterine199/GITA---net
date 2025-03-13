using RegisterLoginJWT.Models;
using RegisterLoginJWT.Models.DTOs.Role;

namespace RegisterLoginJWT.Interfaces
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
