using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class MissedSessions
    {
        public int id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        public int GroupSessionID { get; set; }
        public int ProgramID { get; set; }

        public int GroupProgrammeId { get; set; }





    }
}
