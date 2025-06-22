using PhasePlayWeb.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models
{
    public class OverAllSetVM
    {
        public int id { get; set; }

        [ForeignKey(nameof(Programme))]
        public int ProgramID { get; set; }
        public Programme Programme { get; set; }
    }
}
