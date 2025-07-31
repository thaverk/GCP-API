using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Models
{
    public class TeamBuilderRequest
    {
        public Teams UserTeam { get; set; }
        public string[] MemberIds { get; set; }
    }
}
