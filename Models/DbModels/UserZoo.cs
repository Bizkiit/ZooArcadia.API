using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class UserZoo
    {
        [Key]
        public string username { get; set; }
        public string password { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }

        public void SetPassword(string password)
        {
            password = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool CheckPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, this.password);
        }
    }
}
