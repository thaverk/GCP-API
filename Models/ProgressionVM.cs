namespace PhasePlayWeb.Models
{
    public class ProgressionVM
    {
        public UserVModel User { get; set; }
        //Collection to hold athlete information
        public List<TeamMembersVM> TeamMembers { get; set; }
        // Collection to hold team information
        public List<TeamViewModel> Teams { get; set; }

        // Collection to hold group information
        public List<GroupViewModel> Groups { get; set; }

        // Collection to hold program information
        public List<ProgramViewModel> Programs { get; set; }

        public float? HighestWeight { get; set; }
        // Property to hold progress data
        public List<ProgressData> ProgressData { get; set; }

        // Add a property to hold the list of exercises
        public List<ExerciseViewModel> Excercises { get; set; }
        // Constructor to initialize collections
        public ProgressionVM()
        {
            Teams = new List<TeamViewModel>();
            Groups = new List<GroupViewModel>();
            TeamMembers = new List<TeamMembersVM>();
            Programs = new List<ProgramViewModel>();
            ProgressData = new List<ProgressData>();
            Excercises = new List<ExerciseViewModel>();
        }

        //public List<ProgressData> CalculateHighestWeight(List<SessionHistory> sessionHistories, bool isUser)
        //{
        //    List<ProgressData> progressData = new List<ProgressData>();

        //    if (isUser)
        //    {
        //        HighestWeightUsed = sessionHistories.Max(sh => sh.WeightUsed);
        //        progressData = sessionHistories
        //            .Select(sh => new ProgressData { DateCompleted = sh.DateCompleted.Value, HighestWeightUsed = sh.WeightUsed.Value })
        //            .ToList();
        //    }
        //    else
        //    {
        //        HighestAverageWeightUsed = sessionHistories
        //            .GroupBy(sh => sh.UserId)
        //            .Select(g => g.Average(sh => sh.WeightUsed.Value))
        //            .Max();
        //        progressData = sessionHistories
        //            .GroupBy(sh => sh.UserId)
        //            .Select(g => new ProgressData { DateCompleted = g.First().DateCompleted.Value, HighestWeightUsed = g.Average(sh => sh.WeightUsed.Value) })
        //            .ToList();
        //    }
        //    return progressData;
        //}


    }
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    // Class to hold progress data
    public class ProgressData
    {
        public DateTime DateCompleted { get; set; }
        public double HighestWeightUsed { get; set; }
    }
    //ViewModel for User
    public class UserVModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    //ViewModel for Team
    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }

    //ViewModel for TeamMembers
    public class TeamMembersVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public required string UserName { get; set; }
        public int TeamId { get; set; }
    }
    //ViewModel for Group
    public class GroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    //ViewModel for Program
    public class ProgramViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}

