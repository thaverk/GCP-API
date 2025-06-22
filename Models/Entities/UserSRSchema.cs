using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class UserSRSchema
    {
        public int id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }


        public string name { get; set; }
        public int SetWeek { get; set; }
    }
}
