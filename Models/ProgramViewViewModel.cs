using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class ProgramViewViewModel
    {
        public Tuple<List<Session>, List<int>, int, Session, PAExercise> LegacyModel { get; set; }
        public List<AthleteWorkoutViewModel> Workouts { get; set; } = new List<AthleteWorkoutViewModel>();
        public bool IsSingleWorkout { get; set; } = false;
    }
}