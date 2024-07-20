using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Animal
    {
        [Key]
        public int animalid { get; set; }
        public int? rapportveterinaireid { get; set; }
        public int? habitatid { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public int raceid { get; set; }

        public Race? race { get; set; }
        public Habitat? habitat { get; set; }
        public ICollection<RapportVeterinaire>? rapportveterinaire { get; set; }
        public ICollection<AnimalImageRelation>? animalimagerelation { get; set; }
        public ICollection<AnimalFeeding>? animalfeeding { get; set; }
    }
}
