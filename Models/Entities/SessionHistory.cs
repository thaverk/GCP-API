using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class SessionHistory
    {
        public int id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        public int? SessionSetID { get; set; }
        public int? GroupSessionID { get; set; }
        public int ExcerciseID { get; set; }
        public string ExcerciseName { get; set; }
        public int SchemaID { get; set; }
        public int? PerWeek { get; set; }
        public int reps { get; set; }
        public double? RM { get; set; }
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
        public double? RecommendedWeight { get; set; }





    }
}
