using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Race
    {
        [Key]
        public int raceid { get; set; }
        public string label { get; set; }
        public string description { get; set; }

        public ICollection<Animal>? animal { get; set; }
    }
}
