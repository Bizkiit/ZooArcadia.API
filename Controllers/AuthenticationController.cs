using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.DbModels;
using ZooArcadia.API.Models.QueryModels;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenService _jwtTokenService;

    public AuthenticationController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger, IConfiguration configuration, JwtTokenService jwtTokenService)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            var user = await _context.userzoo.FirstOrDefaultAsync(u => u.username == loginRequest.Username);
            if (user == null || !user.CheckPassword(loginRequest.Password))
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var role = await _context.role.FirstOrDefaultAsync(r => r.username == user.username);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found for the user." });
            }

            var token = _jwtTokenService.GenerateJwtToken(user, role);
            if (string.IsNullOrWhiteSpace(token) || token.Split('.').Length != 3)
            {
                _logger.LogError("Generated token is not well formed.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
            Console.WriteLine($"Token avant de retourner la réponse: {token}");

            var response = new
            {
                roleid = role.roleid,
                label = role.label,
                username = user.username,
                token = token,
            };

            Console.WriteLine($"Token dans l'objet de réponse: {response.token}");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the login request.");
            return StatusCode(500, new { Message = "Internal server error." });
        }
    }
}
