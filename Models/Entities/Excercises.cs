namespace PhasePlayWeb.Models.Entities
{
    public class Excercises
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? YoutubeURL { get; set; }
        public string? Category { get; set; }
        public string? BodyPart { get; set; }
        public string? PrimaryGroup { get; set; }
        public string? SecondaryGroup1 { get; set; }
        public string? SecondaryGroup2 { get; set; }

    }
}
