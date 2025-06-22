using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class ExerciseSetAttrib
    {
        public int id { get; set; }

        [ForeignKey(nameof(Excercises))]
        public int ExcerciseID { get; set; }
        public string ExcerciseName { get; set; }
        public Excercises Excercises { get; set; }

        [ForeignKey(nameof(S_RSchema))]
        public int SchemaID { get; set; }
        public S_RSchema S_RSchema { get; set; }

        public int? PerWeek { get; set; }


        public int reps { get; set; }
        public double? RM { get; set; }
        public double? RPE { get; set; }
        public string? Velocity { get; set; }

        [ForeignKey(nameof(PAExercise))]
        public int PAExerciseID { get; set; }
        public PAExercise PAExercise { get; set; }

        [ForeignKey(nameof(Superset))]
        public int? SupersetID { get; set; }

        public int Day { get; set; }

    }

}
