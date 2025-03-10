using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisterLoginJWT.Interfaces;
using RegisterLoginJWT.Models;
using RegisterLoginJWT.Models.DTOs.User;

namespace RegisterLoginJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService; 
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<ServiceResponse<string>> CreateAsync(UserCreateDTO userCreateDTO)
        {
            return await _userService.CreateAsync(userCreateDTO);
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

        [HttpDelete]
        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            return await _userService.DeleteAsync(id);
        }

        [HttpPut]
        public async Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO userUpdateDTO)
        {
            return await _userService.UpdateAsync(userUpdateDTO);
        }
    }
}
