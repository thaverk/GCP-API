namespace PhasePlayWeb.Models
{
    // Models/WorkoutDayVM.cs
    public class WorkoutDayVM
    {
        public int Week { get; set; }
        public int Day { get; set; }
        public string WorkoutName { get; set; }
        public string Notes { get; set; }
        public List<WorkoutExerciseVM> Exercises { get; set; } = new List<WorkoutExerciseVM>();
    }

    // Models/WorkoutExerciseVM.cs
    public class WorkoutExerciseVM
    {
        public int ExerciseID { get; set; }
        public int SchemaID { get; set; }
        public string ExerciseName { get; set; }
        public string SchemaName { get; set; }
    }
}
