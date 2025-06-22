using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace PhasePlayWeb.Models.Entities
{
    [Table("rpe", Schema = "uwc_mens_15s") ]
    public class PlayerRpeData
    {
        [Column("date")]
        public DateOnly Date { get; set; }

        [Column("name")]
        [MaxLength(255)]
        public string name { get; set; }

        [Column("session_type")]
        [MaxLength(255)]
        public string session_type { get; set; }

        [Column("duration")]
        public int duration { get; set; }

        [Column("rpe")]
        public int rpe { get; set; }

        [Column("player_id")]
        public int? player_id { get; set; }

        [Column("Comments")]
        [MaxLength(50)]
        public string? Comments { get; set; }

        [Column("session_id")]
        public int? session_id { get; set; }

        [Column("Timestamp")]
        [MaxLength(255)]
        public string? Timestamp { get; set; }


    }
}
