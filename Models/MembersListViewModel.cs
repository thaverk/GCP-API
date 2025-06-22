namespace PhasePlayWeb.Models
{
    public class MembersListViewModel
    {
        public List<TeamVModel> Teams { get; set; }
        public List<GroupVModel> Groups { get; set; }
        public List<UserViewModel> User { get; set; }

    }

    public class TeamVModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MembersCount { get; set; }
        public int GroupsCount { get; set; }
        public List<GroupVModel> Groups { get; set; }
    }

    public class GroupVModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserViewModel> User { get; set; }
        public int TeamID { get; set; }
    }

    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}