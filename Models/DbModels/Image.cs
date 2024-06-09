using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Image
    {
        [Key]
        public int imageid { get; set; }
        public byte[] imagedata { get; set; }
        public ICollection<AnimalImageRelation> animalimagerelation { get; set; }
        public ICollection<HabitatImageRelation> habitatimagerelation { get; set; }
    }
}
