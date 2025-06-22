using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class AthleteWorkoutViewModel
    {
        public int PAExerciseID { get; set; }
        public string WorkoutName { get; set; }
        public string Notes { get; set; }
        public List<ExerciseGroupViewModel> Exercises { get; set; } = new List<ExerciseGroupViewModel>();
    }

    public class ExerciseGroupViewModel
    {
        public int ExerciseID { get; set; }
        public string ExerciseName { get; set; }
        public List<Session> Sets { get; set; } = new List<Session>();
    }
}