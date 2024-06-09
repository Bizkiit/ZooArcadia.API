using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ZooArcadia.API.Models.DbModels
{
    public class Habitat
    {
        [Key]
        public int habitatid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public ICollection<Animal>? animal { get; set; }
        public ICollection<HabitatImageRelation>? habitatimagerelation { get; set; }
    }
}
