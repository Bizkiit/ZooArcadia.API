using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

[Route("api/[controller]")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly AnimalService _animalService;

    public AnimalsController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger, AnimalService animalService)
    {
        _context = context;
        _logger = logger;
        _animalService = animalService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
    {
        try
        {
            var animals = await _context.animal
                .Include(a => a.race)
                .Include(a => a.habitat)
                .Include(a => a.animalimagerelation)
                .ThenInclude(ai => ai.image)
                .ToListAsync();

            return Ok(animals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching animals");
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(int id)
    {
        try
        {
            var animal = await _context.animal
                .Include(a => a.race)
                .Include(a => a.habitat)
                .Include(a => a.animalimagerelation)
                .ThenInclude(ai => ai.image)
                .FirstOrDefaultAsync(a => a.animalid == id);

            if (animal == null)
            {
                return NotFound();
            }

            return Ok(animal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching animal");
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("GetAnimalsByHabitat/{habitatid}")]
    public IActionResult GetAnimalsByHabitat(int habitatid)
    {
        var animals = _context.animal
                              .Include(a => a.race)
                              .Include(a => a.habitat)
                              .Include(a => a.animalimagerelation)
                                  .ThenInclude(air => air.image)
                              .Where(a => a.habitatid == habitatid)
                              .ToList();

        return Ok(animals);
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


    [HttpPost("{id}/incrementClick")]
    public async Task<IActionResult> IncrementClick(int id)
    {
        await _animalService.IncrementClickAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/clickStatistics")]
    public async Task<ActionResult<AnimalMongoDB>> GetClickStatistics(int id)
    {
        var animal = await _animalService.GetClickStatisticsAsync(id);
        if (animal == null)
        {
            return NotFound();
        }
        return Ok(animal);
    }

    [HttpGet("clickStatistics")]
    public async Task<ActionResult<List<AnimalMongoDB>>> GetAllClickStatistics()
    {
        var animals = await _animalService.GetAllClickStatisticsAsync();
        return Ok(animals);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnimal(int id, AnimalWithImage animalWithImage)
    {
        if (id != animalWithImage.animalid)
        {
            return BadRequest();
        }

        var animal = await _context.animal
            .Include(a => a.animalimagerelation)
            .ThenInclude(ai => ai.image)
            .FirstOrDefaultAsync(a => a.animalid == id);

        if (animal == null)
        {
            return NotFound();
        }

        animal.name = animalWithImage.name;
        animal.habitatid = animalWithImage.habitatid;
        animal.status = animalWithImage.status;
        animal.raceid = animalWithImage.raceid;

        if (!string.IsNullOrEmpty(animalWithImage.imageBase64))
        {
            var imageData = Convert.FromBase64String(animalWithImage.imageBase64);

            var existingImageRelation = animal.animalimagerelation.FirstOrDefault();
            if (existingImageRelation != null)
            {
                existingImageRelation.image.imagedata = imageData;
                _context.image.Update(existingImageRelation.image);
            }
            else
            {
                var newImage = new Image { imagedata = imageData };
                _context.image.Add(newImage);
                await _context.SaveChangesAsync();

                var newAnimalImageRelation = new AnimalImageRelation
                {
                    animalid = animal.animalid,
                    imageid = newImage.imageid,
                    animal = animal,
                    image = newImage
                };

                _context.animalimagerelation.Add(newAnimalImageRelation);
            }
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
