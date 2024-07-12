using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZooArcadia.API.Models.QueryModels;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(EmailService emailService, ILogger<ContactController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult PostContactRequest([FromBody] ContactRequest contactRequest)
    {
        if (contactRequest == null)
        {
            return BadRequest("Invalid contact request.");
        }

        try
        {
            var subject = $"Nouveau message de contact : {contactRequest.Title}";
            var body = $"<p>{contactRequest.Description}</p><p>De: {contactRequest.Email}</p>";

            _emailService.SendEmail("zooarcadia14210@gmail.com", subject, body);
            _logger.LogInformation("Email sent successfully.");

            return Ok(new { message = "Message sent successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email.");
            return StatusCode(500, new { message = "Internal server error: " + ex.Message });
        }
    }
}
