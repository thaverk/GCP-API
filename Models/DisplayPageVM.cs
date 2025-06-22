using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class DisplayPageVM
    {
        public List<SetAttributes> SetAttributes { get; set; }
        public List<ExerciseSetAttrib> SetExcercises { get; set; }
        public List<PAExercise> Sets { get; set; }
        public List<Excercises> Excercises { get; set; }
        public List<S_RSchema> Schemas { get; set; }
        public int? SelectedWeek { get; set; }
        public string AddNote { get; set; }
    }
}
