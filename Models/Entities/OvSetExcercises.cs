using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class OvSetExcercises
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
