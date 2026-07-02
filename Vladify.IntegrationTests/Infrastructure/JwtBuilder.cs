using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vladify.BusinessLogic.Exceptions;
using Vladify.IntegrationTests.Constants;

namespace Vladify.IntegrationTests.Infrastructure;

public static class JwtBuilder
{
    public static readonly string TestSecretKey = Environment.GetEnvironmentVariable(EnvKeyNames.TestJwt)
        ?? throw new NotFoundException("failed to get jwt key from environment!");

    public static string GenerateTestJWT(string email)
    {
        var keyBytes = Encoding.UTF8.GetBytes(TestSecretKey);
        var key = new SymmetricSecurityKey(keyBytes);
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
