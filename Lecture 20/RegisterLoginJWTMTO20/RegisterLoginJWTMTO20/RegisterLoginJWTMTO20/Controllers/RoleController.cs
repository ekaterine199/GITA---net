using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisterLoginJWTMTO20.Interfaces;
using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.Role;

namespace RegisterLoginJWTMTO20.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<ServiceResponse<string>> CreateAsync(RoleCreateDTO roleCreateDTO)
        {
            return await _roleService.CreateAsync(roleCreateDTO);
        }

        [HttpGet]
        public async Task<ServiceResponse<List<RoleDTO>>> GetAllAsync()
        {
            return await _roleService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse<RoleDTO>> GetByIdAsync(int id)
        {
            return await _roleService.GetByIdAsync(id);
        }

        [HttpDelete]
        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            return await _roleService.DeleteAsync(id);
        }

        [HttpPut]
        public async Task<ServiceResponse<string>> UpdateAsync(RoleUpdateDTO dto)
        {
            return await _roleService.UpdateAsync(dto);
        }
    }
}
