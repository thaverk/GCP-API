using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class GroupMembers
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]

        public string UserId { get; set; }

        [ForeignKey(nameof(Groups))]
        public int GroupId { get; set; }

    }
}
