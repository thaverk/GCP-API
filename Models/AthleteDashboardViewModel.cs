using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class AthleteDashboardViewModel
    {
        public User User { get; set; }
        public List<Event> Events { get; set; }
    }
}
