using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class editProgramVM
    {
        public List<Programme> Programmes { get; set; } = new List<Programme>();
        public PAExercise PAExercises { get; set; }
        public List<ExerciseSetAttrib> ExerciseSetAttribs { get; set; }

    }
}
