using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models
{
    public class TeamsVM
    {
        public int id { get; set; }

        [Required]
        public string TeamName { get; set; }


    }
}
