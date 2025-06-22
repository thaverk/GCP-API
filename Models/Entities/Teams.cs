using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{

    public class Teams
    {

        public int Id { get; set; }




        public string Name { get; set; }

        [ForeignKey(nameof(User))]

        public string UserID { get; set; }
        public User User { get; set; }

    }
}
