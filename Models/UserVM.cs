using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models
{
    public class UserVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }


        [EmailAddress]
        public string? Email { get; set; }




        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public float? BodyWeight { get; set; }
    }
}
