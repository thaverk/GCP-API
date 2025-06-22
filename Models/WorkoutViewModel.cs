namespace PhasePlayWeb.Models
{
    public class WorkoutViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<WorkoutExerciseViewModel> Exercises { get; set; }
    }

    public class WorkoutExerciseViewModel
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int SchemaId { get; set; }
        public int? PerWeek { get; set; }
        public int Reps { get; set; }
        public int? RM { get; set; }
        public double? RPE { get; set; }
        public string Velocity { get; set; }
    }

}
