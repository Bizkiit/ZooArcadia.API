namespace ZooArcadia.API.Models.DbModels
{
    public class AnimalImageRelation
    {
        public int animalid { get; set; }
        public int imageid { get; set; }
        public Animal animal { get; set; }
        public Image image { get; set; }
    }
}
