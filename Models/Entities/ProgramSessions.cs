namespace PhasePlayWeb.Models.Entities
{
    public class ProgramSessions
    {
        public int id { get; set; }
        public int ExcerciseID { get; set; }
        public string ExcerciseName { get; set; }
        public int SchemaID { get; set; }
        public int? PerWeek { get; set; }
        public int reps { get; set; }
        public int? RM { get; set; }
        public double? RPE { get; set; }
        public string? Velocity { get; set; }
        public int PAExerciseID { get; set; }
        public float? WeightUsed { get; set; }
        public DateTime DateAssigned { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int ProgramID { get; set; }
        public bool? Completed { get; set; }
        public string? Reason { get; set; }
        public int GroupProgrammeId { get; set; }
    }
}
