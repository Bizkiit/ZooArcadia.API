using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class RapportVeterinairesController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<RapportVeterinairesController> _logger;

    public RapportVeterinairesController(ZooArcadiaDbContext context, ILogger<RapportVeterinairesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RapportVeterinaire>>> GetRapportsVeterinaires()
    {
        try
        {
            var rapports = await _context.rapportveterinaire
                .Include(r => r.animal)
                .Include(r => r.animal.race)
                .ToListAsync();

            return Ok(rapports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching veterinary reports");
            return BadRequest(ex.ToString());
        }
    }


    [HttpPost]
    public async Task<ActionResult<RapportVeterinaire>> AddRapportVeterinaire(RapportVeterinaire rapportVeterinaire)
    {
        try
        {
            var animal = await _context.animal.FindAsync(rapportVeterinaire.animal.animalid);
            if (animal == null)
            {
                return BadRequest(new { errors = new { animal = new[] { "Animal not found." } } });
            }

            rapportVeterinaire.animal = animal;

            _context.rapportveterinaire.Add(rapportVeterinaire);
            await _context.SaveChangesAsync();

            return Ok(rapportVeterinaire);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding veterinary report");
            return BadRequest(ex.ToString());
        }
    }

    [HttpPut("UpdateHabitatComment")]
    public async Task<IActionResult> UpdateHabitatComment(Habitat updatedHabitat)
    {
        var habitat = await _context.habitat.FindAsync(updatedHabitat.habitatid);
        if (habitat == null)
        {
            return NotFound("Habitat not found");
        }

        habitat.comment = updatedHabitat.comment;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating habitat comment");
            return BadRequest(ex.Message);
        }

        return NoContent();
    }
}
