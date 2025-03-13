using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RegisterLoginJWT.Interfaces;
using RegisterLoginJWT.Models;
using RegisterLoginJWT.Models.DTOs.Role;
using RegisterLoginJWT.Models.Entities;

namespace RegisterLoginJWT.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public RoleService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<string>> CreateAsync(RoleCreateDTO dto)
        {
            try
            {
                await _context.Roles.AddAsync(_mapper.Map<Role>(dto));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false, Message = ex.Message };
            }
            

            return new ServiceResponse<string> { Message = "Role added successfully" };
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (role == null)
                    return new ServiceResponse<bool>() { Success = false, Message = "Role not found" };
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return new ServiceResponse<bool> { Data = true, Message = "Role deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<List<RoleDTO>>> GetAllAsync()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                return new ServiceResponse<List<RoleDTO>> {Data =  roles.Select(x => _mapper.Map<RoleDTO>(x)).ToList() }; 
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<RoleDTO>>() { Success = false, Message = ex.Message};
            }
        }

        public async Task<ServiceResponse<RoleDTO>> GetByIdAsync(int id)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (role == null)
                    return new ServiceResponse<RoleDTO>() { Success = false, Message = "Role not found" };

                return new ServiceResponse<RoleDTO>() { Data = _mapper.Map<RoleDTO>(role) };

            }
            catch (Exception ex)
            {

                return new ServiceResponse<RoleDTO>() { Success = false, Message = ex.GetFullMessage() };
            }
        }

        public async Task<ServiceResponse<string>> UpdateAsync(RoleUpdateDTO dto)
        {
            try
            {
                var roleToUpdate = await _context.Roles.AnyAsync(x => x.Id == dto.Id);

                if (!roleToUpdate)
                    return new ServiceResponse<string>() { Success = false, Message = "Role not found" };

                _context.Roles.Update(_mapper.Map<Role>(dto));
                await _context.SaveChangesAsync();
                return new ServiceResponse<string>() { Data = "Role updated successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>() { Success = false, Message = ex.GetFullMessage() };
            }
        }
    }
}
