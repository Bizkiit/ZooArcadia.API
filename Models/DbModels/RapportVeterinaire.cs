using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public Animal animal { get; set; }
    }
}
