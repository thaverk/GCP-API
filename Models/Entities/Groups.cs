using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class Groups


    {

        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey(nameof(Teams))]
        public int TeamID { get; set; }
        public Teams Teams { get; set; }


    }
}
