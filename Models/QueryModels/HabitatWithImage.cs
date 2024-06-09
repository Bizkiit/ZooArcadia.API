using ZooArcadia.API.Models.DbModels;

namespace ZooArcadia.API.Models.QueryModels
{
    public class HabitatWithImage : Habitat
    {
        public string imageBase64 { get; set; }
    }
}
