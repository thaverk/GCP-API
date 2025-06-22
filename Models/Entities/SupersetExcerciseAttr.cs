using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class SupersetExcerciseAttr
    {
        public int id { get; set; }

        [ForeignKey(nameof(Superset))]
        public int SupersetID { get; set; }

    }
}
