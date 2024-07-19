using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ZooArcadia.API.Services
{
    public class JwtKeyService
    {
        public SymmetricSecurityKey Key { get; }

        public JwtKeyService(IConfiguration configuration, ILogger<JwtKeyService> logger)
        {
            var key = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                logger.LogError("La clé JWT est null ou vide.");
                throw new ArgumentNullException(nameof(key), "JWT key cannot be null or empty.");
            }

            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            logger.LogInformation("JWT Key loaded successfully.");
        }
    }

}
