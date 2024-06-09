using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;

    public AnimalsController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
    {
        try
        {
            var animals = await _context.animal
                .Include(a => a.race)
                .Include(a => a.habitat)
                //.Include(a => a.animalimagerelation)
                //.ThenInclude(ai => ai.image)
                .ToListAsync();

            return Ok(animals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching animals");
            return BadRequest(ex.ToString());
        }
    }



    [HttpPost]
    public async Task<ActionResult<Animal>> AddAnimal(AnimalWithImage animalWithImage)
    {
        var animal = new Animal
        {
            name = animalWithImage.name,
            habitatid = animalWithImage.habitatid,
            status = animalWithImage.status,
            raceid = animalWithImage.raceid
        };

        _context.animal.Add(animal);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(animalWithImage.imageBase64))
        {
            var imageData = Convert.FromBase64String(animalWithImage.imageBase64);
            var image = new Image { imagedata = imageData };
            _context.image.Add(image);
            await _context.SaveChangesAsync();

            var animalImageRelation = new AnimalImageRelation
            {
                animalid = animal.animalid,
                imageid = image.imageid,
                animal = animal,
                image = image
            };

            _context.animalimagerelation.Add(animalImageRelation);
            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetAnimals), new { id = animal.animalid }, animal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnimal(int id, Animal animal)
    {
        if (id != animal.animalid)
        {
            return BadRequest();
        }

        _context.Entry(animal).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnimalExists(id))
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

    private bool AnimalExists(int id)
    {
        return _context.animal.Any(e => e.animalid == id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
        var animal = await _context.animal
            .Include(a => a.animalimagerelation)
            .ThenInclude(ar => ar.image)
            .FirstOrDefaultAsync(a => a.animalid == id);

        if (animal == null)
        {
            return NotFound();
        }

        foreach (var relation in animal.animalimagerelation.ToList())
        {
            var image = await _context.image.FindAsync(relation.imageid);
            if (image != null)
            {
                _context.image.Remove(image);
            }
            _context.animalimagerelation.Remove(relation);
        }

        _context.animal.Remove(animal);

        var race = await _context.race.FirstOrDefaultAsync(r => r.raceid == animal.raceid);
        if (race != null)
        {
            _context.race.Remove(race);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
