using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class Workout
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        [ForeignKey(nameof(Excercises))]
        public int ExcerciseID { get; set; }
        public string ExcerciseName { get; set; }
        [ForeignKey(nameof(S_RSchema))]
        public int SchemaID { get; set; }
        public S_RSchema S_RSchema { get; set; }
        public int? PerWeek { get; set; }
        public int reps { get; set; }
        public int? RM { get; set; }
        public double? RPE { get; set; }
        public string? Velocity { get; set; }
    }
}
