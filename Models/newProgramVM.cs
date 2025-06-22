using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PhasePlayWeb.Models
{
    public class newProgramVM
    {
        public int id { get; set; }
        public int? UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Weeks { get; set; }

        public string Phase { get; set; }

        // Collection of workouts
        public List<WorkoutDayVM> Workouts { get; set; } = new List<WorkoutDayVM>();
    }
}