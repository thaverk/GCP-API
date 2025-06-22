using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhasePlayWeb.Models.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public string? RecurrenceRule { get; set; } //Recurrency Rule (Repeats Events)
        public DateTime DateAdded { get; set; }

        [NotMapped] // This property won't be stored in the database
        public bool IsCompleted { get; set; }
    }

}
