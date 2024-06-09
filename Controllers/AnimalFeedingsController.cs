using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class AnimalFeedingsController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;

    public AnimalFeedingsController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnimalFeeding>>> GetAnimalFeedings([FromQuery] int animalId)
    {
        _logger.LogInformation($"Fetching feedings for animalId: {animalId}");

        try
        {
            var feedings = await _context.animalfeeding
                .Where(f => f.animalid == animalId)
                .OrderByDescending(f => f.feedingdate)
                .ThenByDescending(f => f.feedingtime)
                .ToListAsync();

            if (feedings == null || !feedings.Any())
            {
                _logger.LogWarning($"No feedings found for animalId: {animalId}");
            }

            return Ok(feedings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching animal feedings for animalId: {animalId}");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddAnimalFeeding(AnimalFeeding feeding)
    {

        _context.animalfeeding.Add(feeding);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Animal feeding added successfully" });
    }

}

