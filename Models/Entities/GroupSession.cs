using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class GroupSession
    {
        [Key]
        public int id { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventID { get; set; }
        public Event Event { get; set; }

        [ForeignKey(nameof(GroupProgramme))]
        public int GroupProgrammeID { get; set; }
        public GroupProgramme GroupProgramme { get; set; }

    }
}
