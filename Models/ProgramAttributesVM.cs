using PhasePlayWeb.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models
{
    public class ProgramAttributesVM
    {
        public int ID { get; set; }



        [ForeignKey(nameof(Programme))]
        public int ProgramID { get; set; }
        public int PerWeek { get; set; }
        public Programme Programme { get; set; }



        public int? Sets { get; set; }


    }
}
