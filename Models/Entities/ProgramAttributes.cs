using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class ProgramAttributes
    {
        public int Id { get; set; }


        [ForeignKey(nameof(Programme))]
        public int ProgramID { get; set; }

        public Programme Programme { get; set; }


        public int WeekNumber { get; set; }




    }
}
