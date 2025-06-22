namespace PhasePlayWeb.Models
{
    public class AddMemberToGroupRequest
    {
        public int GroupId { get; set; }
        public List<string> Members { get; set; }
    }
}
