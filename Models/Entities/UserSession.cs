namespace PhasePlayWeb.Models.Entities
{
    public class UserSession
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public int ProgramId { get; set; }
        public int GroupProgrammeId { get; set; }
        public int TotalSessions { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
