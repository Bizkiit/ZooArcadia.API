using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Footer
    {
        [Key]
        public int id { get; set; }
        public string hours { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string email { get; set; }
    }
}
