using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models.Entities
{
    public class PAExercise
    {
        public int id { get; set; }
        public string? Order { get; set; }

        [Required]
        public string? name { get; set; }

        public string? AddNote { get; set; }

        public int ProgramID { get; set; }


    }
}

