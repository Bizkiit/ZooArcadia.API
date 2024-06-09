using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooArcadia.API.Models.DbModels
{
    public class AnimalFeeding
    {
        [Key]
        public int? feedingid { get; set; }
        public int animalid { get; set; }
        public DateTime feedingdate { get; set; }
        public TimeSpan feedingtime { get; set; }
        public string foodtype { get; set; }
        public decimal quantity { get; set; }
        public Animal? animal { get; set; }
    }
}
