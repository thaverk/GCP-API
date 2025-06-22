using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class Superset
    {
        public int id { get; set; }
        public int restperiod { get; set; }

        [ForeignKey(nameof(PAExercise))]
        public int PAExerciseId { get; set; }

    }
}
