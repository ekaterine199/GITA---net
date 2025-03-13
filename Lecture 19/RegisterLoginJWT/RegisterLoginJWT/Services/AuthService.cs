using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisterLoginJWT.Interfaces;
using RegisterLoginJWT.Models;
using RegisterLoginJWT.Models.DTOs.Auth;
using RegisterLoginJWT.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RegisterLoginJWT.Services
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
        public async Task<ServiceResponse<string>> Login(UserLoginDTO loginDTO)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.Include(x => x.Roles).FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName);
            
            var roleNames = user.Roles.Select(x=>x.Name).ToList();
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            else if (!VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Incorrect password";
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

            //response.Data = "Login successful";
            return response;
        }

        public async Task<ServiceResponse<int>> Register(UserRegisterDTO registerDTO)
        {
            var response = new ServiceResponse<int>();
            if(await UserExists(registerDTO.UserName))
            {
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }
            CreatePasswordHash(registerDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Fetch the default "User" role from the database
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (defaultRole == null)
            {
                response.Success = false;
                response.Message = "Default role 'User' not found in the system";
                return response;
            }

            var user = new User()
            {
                UserName = registerDTO.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Roles = new List<Role> { defaultRole } 
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }

        #region PrivateMetods

        private async Task<bool> UserExists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower()))
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordsalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }


        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private TokenDTO GenerateTokens(User user, bool staySignedin, List<string> roles)
        {
            string refreshToken = string.Empty;
            if (staySignedin)
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
          
            //Symmetric securitykey
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token:Secret").Value));

            // SignInCredentials
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //SecurityTokenDescriptor
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials,  
                Issuer = "RegisterLoginJWT",
                Audience = "RegisterLoginJWTClient"
            };

            //JwtSecurityTokenHandler
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            //SecurityToken
            SecurityToken  token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private string GenerateRefreshToken(User user)
        {
            // claims
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };  

            //Symmetric securitykey
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Token:Secret").Value));
            // SignInCredentials
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //SecurityTokenDescriptor
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = credentials,
                Issuer = "RegisterLoginJWT",
                Audience = "RegisterLoginJWTClient"
            };

            //JwtSecurityTokenHandler
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            //SecurityToken
            SecurityToken token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }


       
        #endregion
    }
}
