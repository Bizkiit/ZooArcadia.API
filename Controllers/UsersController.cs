using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;

    public UsersController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
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

    [HttpPost]
    public async Task<ActionResult<UserZoo>> PostUser(UsersWithRole userZoo)
    {
        var user = new UserZoo
        {
            username = userZoo.username,
            password = userZoo.password,
            lastname = userZoo.lastname,
            firstname = userZoo.firstname
        };

        _context.userzoo.Add(user);
        await _context.SaveChangesAsync();

        var role = new Role
        {
            username = userZoo.username,
            label = userZoo.label
        };

        _context.role.Add(role);
        await _context.SaveChangesAsync();

        // Envoi de l'email avec le username
        // TODO: Ajoutez la logique d'envoi d'email ici

        return CreatedAtAction(nameof(GetUsers), new { id = user.username }, user);
    }

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

        return NoContent();
    }

}

