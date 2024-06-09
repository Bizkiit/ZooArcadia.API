using System.ComponentModel.DataAnnotations;

namespace ZooArcadia.API.Models.DbModels
{
    public class Service
    {
        [Key]
        public int serviceid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
