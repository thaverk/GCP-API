using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class TeamMembers
    {

        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public string UserName { get; set; }

        [ForeignKey(nameof(Teams))]
        public int TeamId { get; set; }
    }
}
