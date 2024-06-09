using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class HabitatsController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<HabitatsController> _logger;

    public HabitatsController(ZooArcadiaDbContext context, ILogger<HabitatsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Habitat>>> GetHabitats()
    {
        try
        {
            return await _context.habitat
            .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Habitat>> GetHabitat(int id)
    {
        var habitat = await _context.habitat
            .FirstOrDefaultAsync(h => h.habitatid == id);

        if (habitat == null)
        {
            return NotFound();
        }

        return habitat;
    }

    [HttpPost]
    public async Task<ActionResult<Habitat>> PostHabitat(HabitatWithImage habitatWithImage)
    {
        var habitat = new Habitat
        {
            name = habitatWithImage.name,
            description = habitatWithImage.description,
            comment = habitatWithImage.comment,
            animal = habitatWithImage.animal,
            habitatimagerelation = new List<HabitatImageRelation>()
        };

        _context.habitat.Add(habitat);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(habitatWithImage.imageBase64))
        {
            var imageData = Convert.FromBase64String(habitatWithImage.imageBase64);
            var image = new Image { imagedata = imageData };
            _context.image.Add(image);
            await _context.SaveChangesAsync();

            var habitatImageRelation = new HabitatImageRelation
            {
                habitatid = habitat.habitatid,
                imageid = image.imageid,
                habitat = habitat,
                image = image
            };

            _context.habitatimagerelation.Add(habitatImageRelation);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetHabitat), new { id = habitat.habitatid }, habitat);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutHabitat(int id, Habitat habitat)
    {
        if (id != habitat.habitatid)
        {
            return BadRequest();
        }

        _context.Entry(habitat).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HabitatExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    private bool HabitatExists(int id)
    {
        return _context.habitat.Any(e => e.habitatid == id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHabitat(int id)
    {
        var habitat = await _context.habitat
            .Include(h => h.habitatimagerelation)
            .ThenInclude(hr => hr.image)
            .FirstOrDefaultAsync(h => h.habitatid == id);

        if (habitat == null)
        {
            return NotFound();
        }

        // Remove habitat image relations and images
        foreach (var relation in habitat.habitatimagerelation.ToList())
        {
            var image = await _context.image.FindAsync(relation.imageid);
            if (image != null)
            {
                _context.image.Remove(image);
            }
            _context.habitatimagerelation.Remove(relation);
        }

        _context.habitat.Remove(habitat);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
