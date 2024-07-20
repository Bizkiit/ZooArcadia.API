using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZooArcadia.API.Models.DbModels;
using ZooArcadia.API.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly JwtKeyService _jwtKeyService;

    public JwtTokenService(IConfiguration configuration, JwtKeyService jwtKeyService)
    {
        _configuration = configuration;
        _jwtKeyService = jwtKeyService;
    }

    public string GenerateJwtToken(UserZoo user, Role role)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, role.label),
        new Claim("roleid", role.roleid.ToString()),
    };

        var key = _jwtKeyService.Key;
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(30),
            SigningCredentials = creds,
            Audience = _configuration["Jwt:Audience"],
            Issuer = _configuration["Jwt:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
