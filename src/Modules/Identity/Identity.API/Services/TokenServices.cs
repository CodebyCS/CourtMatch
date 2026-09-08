using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(IdentityUser user, IList<string> roles)
    {
        var secretKey = _configuration["JwtSettings:Secret"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT Secret Key is not configured in User Secrets");
        }

        // Alterado para UTF8.GetBytes por ser Padrão mundial (ASCII so suporta caracteres basicos em inglês)
        var key = Encoding.UTF8.GetBytes(secretKey);

        // Informações básicas do utilizador
        var claims = new List<Claim>
        {
          new Claim(ClaimTypes.NameIdentifier, user.Id),
          new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
          new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };

        // Injeta as Roles no Token (ex: "Manager")
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Monta o token assinado
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(4), // Validade do token
            Issuer = _configuration["JwtSettings:Issuer"] ?? "CourtMatch",
            Audience = _configuration["JwtSettings:Audience"] ?? "CourtMatchClient",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}