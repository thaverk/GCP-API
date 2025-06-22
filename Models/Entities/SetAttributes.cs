using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class SetAttributes
    {
        public int id { get; set; }

        [ForeignKey(nameof(Sets))]
        public int SetID { get; set; }
        public PAExercise Sets { get; set; }

        public int? SetNumber { get; set; }





        [ForeignKey(nameof(Excercises))]
        public int ExcercisesId { get; set; }
        public Excercises Excercises { get; set; }

    }
}
