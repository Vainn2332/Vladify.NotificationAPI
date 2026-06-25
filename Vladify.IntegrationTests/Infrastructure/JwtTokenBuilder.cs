using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vladify.IntegrationTests.Constants;

namespace Vladify.IntegrationTests;

public static class JwtTokenBuilder
{
    public static string GenerateTestJWT(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestConstants.TestSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(TestConstants.CustomEmailClaimName,email)
        };

        var token = new JwtSecurityToken(
            issuer: TestConstants.Issuer,
            audience: TestConstants.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
