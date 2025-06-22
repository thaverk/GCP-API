using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class AttributeSet
    {
        public int Id { get; set; }

        public string? order { get; set; }

        [ForeignKey(nameof(ProgramAttributes))]
        public int attributeID { get; set; }
        public ProgramAttributes? ProgramAttributes { get; set; }

        [ForeignKey(nameof(Sets))]
        public int SetID { get; set; }
        public PAExercise? Sets { get; set; }

    }
}
