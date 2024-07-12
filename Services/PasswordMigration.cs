using System.Linq;
using BCrypt.Net;
using ZooArcadia.API.Models.DbModels;

public class PasswordMigration
{
    private readonly ZooArcadiaDbContext _context;

    public PasswordMigration(ZooArcadiaDbContext context)
    {
        _context = context;
    }

    public void MigratePasswords()
    {
        var users = _context.userzoo.ToList();
        foreach (var user in users)
        {
            if (!user.password.StartsWith("$2a$") && !user.password.StartsWith("$2b$") && !user.password.StartsWith("$2y$"))
            {
                user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            }
        }

        _context.SaveChanges();
    }
}
