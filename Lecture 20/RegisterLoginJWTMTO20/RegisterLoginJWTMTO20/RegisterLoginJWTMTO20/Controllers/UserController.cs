using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisterLoginJWTMTO20.Interfaces;
using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.User;

namespace RegisterLoginJWTMTO20.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ServiceResponse<List<UserDTO>>> GetAllAsync()
        {
            return await _userService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse<UserDTO>> GetByIdAsync(int id)
        {
            return await _userService.GetByIdAsync(id);
        }

        [HttpPost]
        public async Task<ServiceResponse<string>> CreateAsync(UserCreateDTO dto)
        {
            return await _userService.CreateAsync(dto);
        }

        [HttpPut]
        public async Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto)
        {
            return await _userService.UpdateAsync(dto);
        }

        [HttpDelete]
        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            return await _userService.DeleteAsync(id);
        }
    }
}
