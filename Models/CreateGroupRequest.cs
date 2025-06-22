namespace PhasePlayWeb.Models
{
    public class CreateGroupRequest
    {
        public string GroupName { get; set; }
        public int TeamId { get; set; }
        public List<string> MemberIds { get; set; }
    }
}
