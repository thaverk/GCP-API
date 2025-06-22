using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class SessionDate
    {
        public int id { get; set; }

        [ForeignKey(nameof(Session))]
        public int SessionID { get; set; }
        public Session Session { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventID { get; set; }
        public Event Event { get; set; }

        public DateTime DateAdded { get; set; }
        public bool Completed { get; set; }
        public int GroupProgrammeId { get; set; }

    }
}
