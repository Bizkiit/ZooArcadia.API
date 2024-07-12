using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooArcadia.API.Models.DbModels;

[Route("api/[controller]")]
[ApiController]
public class FooterController : ControllerBase
{
    private readonly ZooArcadiaDbContext _context;

    public FooterController(ZooArcadiaDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<Footer>> GetFooter()
    {
        var footer = await _context.footer.FirstOrDefaultAsync();
        if (footer == null)
        {
            return NotFound();
        }
        return Ok(footer);
    }

    [HttpPost]
    public async Task<ActionResult<Footer>> UpdateFooter(Footer footer)
    {
        var existingFooter = await _context.footer.FindAsync(footer.id);

        if (existingFooter == null)
        {
            existingFooter = new Footer
            {
                hours = footer.hours,
                address1 = footer.address1,
                address2 = footer.address2,
                address3 = footer.address3,
                email = footer.email
            };
            _context.footer.Add(existingFooter);
        }
        else
        {
            existingFooter.hours = footer.hours;
            existingFooter.address1 = footer.address1;
            existingFooter.address2 = footer.address2;
            existingFooter.address3 = footer.address3;
            existingFooter.email = footer.email;
        }

        await _context.SaveChangesAsync();
        return Ok(existingFooter);
    }

}

