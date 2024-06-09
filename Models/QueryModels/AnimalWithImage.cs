using ZooArcadia.API.Models.DbModels;

namespace ZooArcadia.API.Models.QueryModels
{
    public class AnimalWithImage : Animal
    {
        public string imageBase64 { get; set; }
    }
}
