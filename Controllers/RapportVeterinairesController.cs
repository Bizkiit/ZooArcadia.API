using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<ActionResult<RapportVeterinaire>> AddRapportVeterinaire(RapportVeterinaire rapportVeterinaire)
    {
        try
        {
            _context.rapportveterinaire.Add(rapportVeterinaire);
            await _context.SaveChangesAsync();

            var animal = await _context.animal.FindAsync(rapportVeterinaire.animalid);
            if (animal != null)
            {
                animal.rapportveterinaireid = rapportVeterinaire.rapportveterinaireid;
                _context.animal.Update(animal);
                await _context.SaveChangesAsync();
            }

            return Ok(rapportVeterinaire);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding veterinary report");
            return BadRequest(ex.ToString());
        }
    }

    // Nouvel endpoint pour mettre à jour l'état des habitats
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
