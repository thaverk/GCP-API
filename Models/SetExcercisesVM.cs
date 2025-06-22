using PhasePlayWeb.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models
{
    public class SetExcercisesVM
    {
        public int id { get; set; }

        [ForeignKey(nameof(Excercises))]
        public int ExcerciseID { get; set; }
        public Excercises Excercises { get; set; }

        [ForeignKey(nameof(Sets))]
        public int SetID { get; set; }
        public string ExcerciseName { get; set; }
        public PAExercise Sets { get; set; }

        public string Cluster { get; set; }
        public int RPM { get; set; }
        public int sets { get; set; }
        public int reps { get; set; }
    }
}
