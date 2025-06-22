using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models
{
    public class ProgrammeVM
    {
        public int id { get; set; }
        public int? UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Weeks { get; set; }


    }
}
