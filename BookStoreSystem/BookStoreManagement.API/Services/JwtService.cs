using BookStoreManagement.API.Data;
using BookStoreManagement.API.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BookStoreManagement.API.Models.Auth;


namespace BookStoreManagement.API.Services
{
    public class JwtService
    {
        private readonly ApplicationDBContext _dBContext;
        private readonly IConfiguration _configuration;

        public JwtService(ApplicationDBContext dBContext, IConfiguration configuration) 
        {
            _dBContext = dBContext;
            _configuration = configuration;

        }

        public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
        {

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var userAccount = await _dBContext.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);
            if (userAccount is null || !PasswordHashHandler.VerifyPassword(request.Password, userAccount.PasswordHash!))
                return null;

            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            if (string.IsNullOrEmpty(key))
            {
                // Neu vao day nghia la code van chua doc duoc file appsettings.json
                throw new Exception("Jwt Key is missing in configuration!");
            }

            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenvalidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {

                    new Claim(ClaimTypes.Name, userAccount.Username!),

                    new Claim(JwtRegisteredClaimNames.Sub, userAccount.UserId.ToString()),

                    new Claim(ClaimTypes.Role, userAccount.RoleId ?? "staff")
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature)

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponseModel
            {
                AccessToken = accessToken,
                Username = request.Username,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds

            };

        }

    }
}
