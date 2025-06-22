using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class UserSchemaAttributes
    {
        public int id { get; set; }
        [ForeignKey(nameof(UserSRSchema))]
        public int UserSRSchemaId { get; set; }

        public int Reps { get; set; }
        public int PercentRM { get; set; }
        public double RPE { get; set; }
        public int? Vel { get; set; }
    }
}

