using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Role
    {
        [Key]
        public int roleid { get; set; }
        public string username { get; set; }
        public string label { get; set; }
    }
}
