using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class RapportVeterinaire
    {
        [Key]
        public int rapportveterinaireid { get; set; }
        public DateTime? date { get; set; }
        public string detail { get; set; }
        public int quantity { get; set; }
        public string foodtype { get; set; }
        public string status { get; set; }
        public int animalid { get; set; }
        public ICollection<Animal>? animal { get; set; }
    }
}
