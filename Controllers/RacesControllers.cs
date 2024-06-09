using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class RacesController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<RacesController> _logger;

    public RacesController(ZooArcadiaDbContext context, ILogger<RacesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Race>>> GetRaces()
    {
        try
        {
            var races = await _context.race.ToListAsync();
            return Ok(races);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting races.");
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("AddRace")]
    public async Task<IActionResult> AddRace(Race newRace)
    {
        if (newRace == null || string.IsNullOrEmpty(newRace.label))
        {
            return BadRequest("Invalid race data.");
        }

        _context.race.Add(newRace);
        await _context.SaveChangesAsync();

        return Ok();
    }

}
