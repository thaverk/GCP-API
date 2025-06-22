using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models.Entities
{
    public class PercentRM_Mapping
    {
        [Key]
        public int id { get; set; }
        public double PercentRM { get; set; }
        public double RPE { get; set; }
        public string RIR { get; set; }
        public string VelocityRange { get; set; }
    }
}
