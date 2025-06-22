using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class SchemaAttributes
    {
        public int id { get; set; }

        [ForeignKey(nameof(S_RSchema))]
        public int SchemaID { get; set; }
        public S_RSchema S_RSchema { get; set; }

        public int Reps { get; set; }
        public double PercentRM { get; set; }
        public double RPE { get; set; }
        public string? Vel { get; set; }
    }
}
