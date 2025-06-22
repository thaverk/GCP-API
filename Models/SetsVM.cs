using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models
{
    public class SetsVM
    {
        public int id { get; set; }

        [Required]
        public string name { get; set; }
        public string AddNote { get; set; }
    }
}
