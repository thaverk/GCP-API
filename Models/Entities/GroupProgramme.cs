using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class GroupProgramme
    {
        public int id { get; set; }

        [ForeignKey(nameof(Programme))]
        public int programmeId { get; set; }

        [ForeignKey(nameof(Groups))]
        public int GroupID { get; set; }

        public DateTime DateAdded { get; set; }




    }
}
