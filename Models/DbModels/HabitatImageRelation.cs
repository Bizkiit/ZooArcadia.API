using System.Text.Json.Serialization;

namespace ZooArcadia.API.Models.DbModels
{
    public class HabitatImageRelation
    {
        public int habitatid { get; set; }
        public int imageid { get; set; }
        public Habitat habitat { get; set; }
        public Image image { get; set; }
    }
}
