using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models;
using ZooArcadia.API.Models.QueryModels;
using Microsoft.Extensions.Logging;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;

    public ServicesController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetServices()
    {
        return await _context.service.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Service>> PostService(Service service)
    {
        _context.service.Add(service);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetServices), new { id = service.serviceid }, service);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutService(int id, Service service)
    {
        if (id != service.serviceid)
        {
            return BadRequest();
        }

        _context.Entry(service).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ServiceExists(id))
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await _context.service.FindAsync(id);
        if (service == null)
        {
            return NotFound();
        }

        _context.service.Remove(service);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ServiceExists(int id)
    {
        return _context.service.Any(e => e.serviceid == id);
    }

}

