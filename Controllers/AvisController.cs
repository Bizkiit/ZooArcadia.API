using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class AvisController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;

    public AvisController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Avis>>> GetAvis()
    {
        return await _context.avis.ToListAsync();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAvisVisibility([FromBody] Avis updateAvis)
    {
        var avis = await _context.avis.FindAsync(updateAvis.avisid);
        if (avis == null)
        {
            return NotFound();
        }

        avis.isvisible = updateAvis.isvisible;
        await _context.SaveChangesAsync();

        return NoContent();
    }

}

