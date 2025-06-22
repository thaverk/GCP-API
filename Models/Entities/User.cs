// Models/Entities/User.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class User : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public float? BodyWeight { get; set; }
        public bool? FirstSignIn { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; }

        public ICollection<Teams> teams { get; } = new List<Teams>();
    }
}
