using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class SessionSet
    {
        public int id { get; set; }
        public int SetNumber { get; set; }

        [ForeignKey(nameof(GroupSession))]
        public int GroupSessionID { get; set; }
        public GroupSession GroupSession { get; set; }

        [ForeignKey(nameof(ExerciseSetAttrib))]
        public int ExerciseSetAttribID { get; set; }
        public ExerciseSetAttrib ExerciseSetAttrib { get; set; }

        [ForeignKey(nameof(PAExercise))]
        public int PAExerciseID { get; set; }
        public PAExercise PaExercise { get; set; }

        public int? PerWeek { get; set; }

        public DateTime DateAssigned { get; set; }

        [ForeignKey(nameof(Programme))]
        public int ProgrammeID { get; set; }
        public Programme Programme { get; set; }



    }
}
