using Microsoft.EntityFrameworkCore;
using RegisterLoginJWT.Interfaces;
using RegisterLoginJWT.Models;
using RegisterLoginJWT.Models.DTOs.Role;
using RegisterLoginJWT.Models.DTOs.User;
using RegisterLoginJWT.Models.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace RegisterLoginJWT.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<string>> CreateAsync(UserCreateDTO dto)
        {
            try
            {
                var response = new ServiceResponse<string>();

                // Check if the username already exists
                if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName))
                {
                    response.Success = false;
                    response.Message = "Username already exists.";
                    return response;
                }

                // Hash password
                using var hmac = new HMACSHA512();
                var user = new User
                {
                    UserName = dto.UserName,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                    PasswordSalt = hmac.Key,
                    Roles = new List<Role>()
                };

                // Assign roles if provided
                if (dto.Roles != null && dto.Roles.Any())
                {
                    var roles = await _context.Roles
                        .Where(r => dto.Roles.Contains(r.Name))
                        .ToListAsync();

                    user.Roles.AddRange(roles);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = "User created successfully.";
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false, Message = ex.Message };
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
                    response.Message = "User not found.";
                    return response;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<List<UserDTO>>> GetAllAsync()
        {
            try
            {
                var users = await _context.Users
                .Include(u => u.Roles) // Include roles
                .ToListAsync();

                var userDTOs = users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = u.Roles.Select(r => r.Name).ToList()
                }).ToList();

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
                    response.Message = "User not found.";
                    return response;
                }

                response.Success = true;
                response.Data = new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Roles = user.Roles.Select(r => r.Name).ToList()
                };

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
                    response.Message = "User not found.";
                    return response;
                }

                if (!string.IsNullOrWhiteSpace(dto.UserName))
                {
                    user.UserName = dto.UserName;
                }

                if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                {
                    using var hmac = new HMACSHA512();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NewPassword));
                    user.PasswordSalt = hmac.Key;
                }

                // Update roles if provided
                if (dto.Roles != null)
                {
                    user.Roles.Clear();
                    var roles = await _context.Roles
                        .Where(r => dto.Roles.Contains(r.Name))
                        .ToListAsync();

                    user.Roles.AddRange(roles);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Data = "User updated successfully.";
                return response;
            }
            catch (Exception ex)
            {

                return new ServiceResponse<string>() { Success = false, Message = ex.GetFullMessage() };
            }
           
        }
    }
    
}
