namespace PhasePlayWeb.Models
{
    public class PastProgramView
    {
        public string ProgramName { get; set; }
        public int? GroupProgrammeID { get; set; }
        public int? GroupSessionID { get; set; }
        public int? SessionSetID { get; set; }
        public int ProgramId { get; set; }
        public string UserID { get; set; }
        public int GroupID { get; set; }
        public string Phase { get; set; }
        public int? Week { get; set; }
        public DateTime DateAssigned { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int? TotalSessions { get; set; }

    }
}
