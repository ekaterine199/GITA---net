using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisterLoginJWTMTO20.Interfaces;
using RegisterLoginJWTMTO20.Models;
using RegisterLoginJWTMTO20.Models.DTO_s.Auth;
using RegisterLoginJWTMTO20.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Resources;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RegisterLoginJWTMTO20.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<int>> Register(UserRegisterDTO registerDTO)
        {
            var response = new ServiceResponse<int>();

            if (await UserExists(registerDTO.UserName))
            {
                response.Success = false;
                //response.Message = "User already exists";
                response.Message = ResourceHelper.GetResource("RegistrationErrorWhenUserExists");

                return response;
            }
            CreatePasswordHash(registerDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User()
            {
                UserName = registerDTO.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }
        public async Task<ServiceResponse<string>> Login(UserLoginDTO loginDTO)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDTO.UserName.ToLower());

            if (user != null)
            {
                if (user.Status == Enums.Status.Deleted)
                    return new ServiceResponse<string>() { Data = ResourceHelper.GetResource("LoginErrorWhenUserIsDeleted") };

                if (user.Status == Enums.Status.Inactive)
                    return new ServiceResponse<string>() { Data = ResourceHelper.GetResource("LoginErrorWhenUserIsInactive") };
            }
            
            var roleNames = new List<string>();

            if (user != null)
                roleNames = user.Roles.Select(x => x.Name).ToList();

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                response.Data = ResourceHelper.GetResource("LoginErrorWhenUserNotFound");
                return response;
            }
            else if (!VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect password";
                response.Message = ResourceHelper.GetResource("LoginErrorWhenPasswordNotCorrect");
                return response;
            }
            else
            {
                var result = GenerateTokens(user, loginDTO.StaySignedIn, roleNames);
                response.Data = result.AccessToken;
            }

            if (loginDTO.StaySignedIn)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return response;
        }

        #region PrivateMethods

        private async Task<bool> UserExists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower()))
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private TokenDTO GenerateTokens(User user, bool staySignedIn, List<string> roles)
        {
            string refreshToken = string.Empty;

            if (staySignedIn)
            {
                refreshToken = GenerateRefreshToken(user);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpirationDate = DateTime.Now.AddDays(2);
            }

            var accessToken = GenerateAccessToken(user, roles);

            return new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        private string GenerateAccessToken(User user, List<string> roles)
        {
            var claims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token:Secret").Value));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials,
                Issuer = "RegisterLoginJWTMTO20",
                Audience = "RegisterLoginJWTMTO20Client"
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token:Secret").Value));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = credentials,
                Issuer = "RegisterLoginJWTMTO20",
                Audience = "RegisterLoginJWTMTO20Client"
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            SecurityToken token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        #endregion
    }
}
