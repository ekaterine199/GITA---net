using RegisterLoginJWTMTO20.Interfaces;
using RegisterLoginJWTMTO20.Models.Entities;
using RegisterLoginJWTMTO20.Models;
using System.Security.Cryptography;
using System.Text;
using RegisterLoginJWTMTO20.Models.DTO_s.User;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;

namespace RegisterLoginJWTMTO20.Services
{
    public class UserService : IUserService

    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<string>> CreateAsync(UserCreateDTO dto)
        {
            try
            {
                var response = new ServiceResponse<string>();
                if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
                {
                    response.Success = false;
                    response.Message = ResourceHelper.GetResource("UserAlreadyExists");
                    return response;
                }
                using var hmac = new HMACSHA512();
                var user = new User
                {
                    UserName = dto.UserName,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                    PasswordSalt = hmac.Key,
                    Roles = new List<Role>()
                };
                if (dto.RoleIds != null && dto.RoleIds.Any())
                {
                    var roles = await _context.Roles
                        .Where(r => dto.RoleIds.Contains(r.Id))
                        .ToListAsync();
                    user.Roles.AddRange(roles);
                }
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                response.Data = ResourceHelper.GetResource("UserAdditionSuccessful");
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false, Message = ex.GetFullMessage() };
            }

        }
        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var response = new ServiceResponse<bool>();
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = ResourceHelper.GetResource("UserNotFound");
                    return response;
                }
                user.Status = Enums.Status.Deleted;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Message = ResourceHelper.GetResource("UserDeleteSuccessfull");
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Success = false, Message = ex.GetFullMessage() };
            }

        }
        public async Task<ServiceResponse<List<UserDTO>>> GetAllAsync()
        {
            try
            {
                var users = await _context.Users
                .Include(u => u.Roles)
                .ToListAsync();
                var userDTOs = users.Select(x => _mapper.Map<UserDTO>(x)).ToList();
                return new ServiceResponse<List<UserDTO>> { Success = true, Data = userDTOs };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<UserDTO>>() { Success = false, Message = ex.Message };
            }
        }
        public async Task<ServiceResponse<UserDTO>> GetByIdAsync(int id)
        {
            try
            {
                var response = new ServiceResponse<UserDTO>();
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = ResourceHelper.GetResource("UserNotFound");
                    return response;
                }
                response.Data = _mapper.Map<UserDTO>(user);
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserDTO>() { Success = false, Message = ex.GetFullMessage() };
            }
        }
        public async Task<ServiceResponse<string>> UpdateAsync(UserUpdateDTO dto)
        {
            try
            {
                var response = new ServiceResponse<string>();
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == dto.Id);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = ResourceHelper.GetResource("UserNotFound");
                    return response;
                }
                if (!string.IsNullOrWhiteSpace(dto.UserName))
                    user.UserName = dto.UserName;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    using var hmac = new HMACSHA512();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
                    user.PasswordSalt = hmac.Key;
                }
                if (dto.RoleIds != null)
                {
                    user.Roles.Clear();
                    var roles = await _context.Roles
                        .Where(r => dto.RoleIds.Contains(r.Id))
                        .ToListAsync();
                    user.Roles.AddRange(roles);
                }
                if (dto.Status != null)
                    user.Status = dto.Status;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                response.Data = ResourceHelper.GetResource("UserUpdateSuccessful");
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false, Message = ex.GetFullMessage() };
            }
        }
    }
}
