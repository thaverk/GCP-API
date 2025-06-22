namespace PhasePlayWeb.Models
{
    public class AthleteViewProgrammeView
    {

        public int programmeId { get; set; }

        public string? Name { get; set; }
        public string PAExerciseName { get; set; }

        public int? Weeks { get; set; }

        public string? OccursOn { get; set; }

        public DateTime? DateAdded { get; set; }

        public DateTime? NextSessionDate { get; set; }

        public int? WeekOfProgramme { get; set; }
        public int? GroupProgrammeId { get; set; }
        public int? GroupSessionID { get; set; }

    }
}
