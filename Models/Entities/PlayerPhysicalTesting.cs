namespace PhasePlayWeb.Models.Entities
{
    public class PlayerPhysicalTesting
    {
        public DateOnly? Date { get; set; }
        public string? Name { get; set; }

        public int? HeightM { get; set; }
        public Decimal? BodyWeightKg { get; set; }
        public Decimal? B_JumpDBL { get; set; }
        public Decimal? RMBench { get; set; }
        public Decimal? RMSquat { get; set; }
        public Decimal? PullUps { get; set; }
        public Decimal? Bronco { get; set; }
        public Decimal? TenMSprint { get; set; }
        public Decimal? Test505L { get; set; }
        public Decimal? Test505R { get; set; }
        public Decimal? DiffLR { get; set; }
        public Decimal? SquatRelative { get; set; }
        public Decimal? BenchRelative { get; set; }
        public Decimal? AgilityT { get; set; }
        public int? RepeatedSprints { get; set; }
    }
}

