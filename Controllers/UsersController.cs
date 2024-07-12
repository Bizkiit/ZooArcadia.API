using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<UsersController> _logger;
    private readonly EmailService _emailService;

    public UsersController(ZooArcadiaDbContext context, ILogger<UsersController> logger, EmailService emailService)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    /// <summary>
    /// Gets the users.
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Administrateur")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        _logger.LogInformation("GetUsers endpoint called");

        var authHeader = Request.Headers["Authorization"].ToString();
        _logger.LogInformation($"Authorization header received: {authHeader}");

        var users = await _context.userzoo
                                  .Join(_context.role,
                                        user => user.username,
                                        role => role.username,
                                        (user, role) => new
                                        {
                                            user.firstname,
                                            user.lastname,
                                            user.username,
                                            role.label
                                        })
                                  .Where(u => u.label != "Administrateur")
                                  .ToListAsync();

        return Ok(users);
    }


    [Authorize(Roles = "Administrateur")]
    [HttpPost]
    public async Task<ActionResult<UserZoo>> PostUser(UsersWithRole userZoo)
    {
        if (await _context.userzoo.AnyAsync(u => u.username == userZoo.username))
        {
            return BadRequest(new { Message = "Username already exists." });
        }

        var user = new UserZoo
        {
            username = userZoo.username,
            lastname = userZoo.lastname,
            firstname = userZoo.firstname
        };

        user.SetPassword(userZoo.password);

        _context.userzoo.Add(user);
        await _context.SaveChangesAsync();

        var role = new Role
        {
            username = userZoo.username,
            label = userZoo.label
        };

        _context.role.Add(role);
        await _context.SaveChangesAsync();

        // Envoi de l'email avec le username et le mot de passe
        var subject = "Bienvenue dans l'équipe Zoo Arcadia";
        var body = $"<p>Bonjour {user.firstname} {user.lastname},</p>" +
                   $"<p>Votre compte a été créé avec succès. Voici vos informations de connexion :</p>" +
                   $"<p>Nom d'utilisateur : {user.username}</p>" +
                   $"<p>Rapprochez vous de votre administrateur pour récuperer votre mot de passe</p>" +
                   "<p>Merci de faire partie de Zoo Arcadia !</p>";

        _emailService.SendEmail(user.username, subject, body);

        _logger.LogInformation("User created: {Username}", user.username);

        return CreatedAtAction(nameof(GetUsers), new { id = user.username }, user);
    }

    [Authorize(Roles = "Administrateur")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _context.userzoo.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.userzoo.Remove(user);
        await _context.SaveChangesAsync();

        var role = await _context.role.FirstOrDefaultAsync(r => r.username == id);
        if (role != null)
        {
            _context.role.Remove(role);
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("User deleted: {username}", id);

        return NoContent();
    }
}
