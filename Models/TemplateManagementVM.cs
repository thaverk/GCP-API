using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class TemplateManagementVM
    {
        public List<Excercises> Exercises { get; set; } = new List<Excercises>();
        public List<S_RSchema> SetsAndRepsSchemas { get; set; } = new List<S_RSchema>();
        public List<Programme> Programmes { get; set; } = new List<Programme>();
        public List<Teams> Teams { get; set; } = new List<Teams>();
    }
}