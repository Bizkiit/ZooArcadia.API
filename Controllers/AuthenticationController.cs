using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            var user = await _context.userzoo
                                     .FirstOrDefaultAsync(u => u.username == loginRequest.Username && u.password == loginRequest.Password);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            Role? role = await _context.role
                                     .Where(r => r.username == user.username)
                                     .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound(new { Message = "Role not found for the user." });
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the login request.");
            return StatusCode(500, new { Message = "Internal server error." });
        }
    }
}

