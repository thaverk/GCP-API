using PhasePlayWeb.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models
{
    public class OvSetExcercisesVM
    {
        public int id { get; set; }
        [ForeignKey(nameof(OverAllSet))]
        public int OverAllSetID { get; set; }
        public OverAllSet OverAllSet { get; set; }

        [ForeignKey(nameof(Excercises))]
        public int ExcerciseID { get; set; }
        public Excercises Excercise { get; set; }
    }
}
