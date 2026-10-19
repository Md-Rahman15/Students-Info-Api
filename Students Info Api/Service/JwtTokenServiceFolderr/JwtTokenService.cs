using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Students_Info_Api.AuthData.Configuration;
using Students_Info_Api.AuthData.DM;
using Students_Info_Api.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Students_Info_Api.Service.JwtTokenServiceFolderr
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings jwtsettings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
           jwtsettings = options.Value;
        }
        public string GenerateJwtToken(User_DM user)
        {
            var myClaims = new List<Claim>() {

                new Claim(JwtRegisteredClaimNames.Sub,user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Roles)

            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsettings.SecretKey));

            var mySigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtsettings.Issuer,
                audience: jwtsettings.Audience,
                claims: myClaims,
                expires:DateTime.UtcNow.AddMinutes(jwtsettings.ExpirationMinutes),
                signingCredentials: mySigningCredentials

            );

            var jwtTokenHandler = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return jwtTokenHandler;


        }
    }
}
