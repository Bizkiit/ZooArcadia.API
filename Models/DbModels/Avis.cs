using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Avis
    {
        [Key]
        public int avisid { get; set; }
        public string? pseudo { get; set; }
        public string? comment { get; set; }
        public bool? isvisible { get; set; }
    }

}
