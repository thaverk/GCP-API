using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class OverAllSet
    {
        public int id { get; set; }

        [ForeignKey(nameof(Programme))]
        public int ProgramID { get; set; }
        public Programme Programme { get; set; }

    }
}
