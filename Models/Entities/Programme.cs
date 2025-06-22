using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class Programme
    {
        public int id { get; set; }

        public int? GroupID { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserID { get; set; }


        public string? Name { get; set; }

        public int Weeks { get; set; }

        public string? OccursOn { get; set; }
    }
}
