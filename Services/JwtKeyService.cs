using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ZooArcadia.API.Services
{
    public class JwtKeyService
    {
        public SymmetricSecurityKey Key { get; }

        public JwtKeyService(IConfiguration configuration)
        {
            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        }
    }
}
