using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhasePlayWeb.Models.Entities;

namespace PhasePlayWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserExercises> UserExercises { get; set; }
        public DbSet<UserSRSchema> userSRSchemas { get; set; }
        public DbSet<UserSchemaAttributes> userSchemaAttributes { get; set; }
        public DbSet<UserType> UserTypes { get; set; }

        public DbSet<Excercises> Excercises { get; set; }
        public DbSet<GroupMembers> GroupMembers { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<OverAllSet> OverAllSets { get; set; }
        public DbSet<OvSetExcercises> OvSetExcercises { get; set; }
        public DbSet<Programme> Programs { get; set; }
        public DbSet<ProgramAttributes> ProgramAttributes { get; set; }
        public DbSet<ExerciseSetAttrib> SetExcercises { get; set; }
        public DbSet<PAExercise> PAExercises { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<TeamMembers> TeamMembers { get; set; }
        public DbSet<AttributeSet> AttributeSets { get; set; }
        public DbSet<SetAttributes> SetAttributes { get; set; }
        public DbSet<SchemaAttributes> SchemaAttributes { get; set; }
        public DbSet<S_RSchema> SRSchema { get; set; }
        public DbSet<Superset> Supersets { get; set; }
        public DbSet<SupersetExcerciseAttr> SupersetExcerciseAttr { get; set; }
        public DbSet<GroupProgramme> GroupProgrammes { get; set; }
        public DbSet<SessionDate> SessionDates { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<SessionHistory> SessionHistory { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<PlayerPhysicalTesting> PlayerPhysicalTesting { get; set; }
        public DbSet<MissedSessions> MissedSessions { get; set; }
        public DbSet<GroupSession> GroupSession { get; set; }
        public DbSet<SessionSet> SessionSet { get; set; }
        public DbSet<PercentRM_Mapping> PercentRM_Mapping { get; set; }
        public DbSet<Workout> Workouts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PlayerPhysicalTesting>()
                .ToTable("physical_testing", "uwc_mens_15s")
                .HasNoKey();// Indicate that this entity does not have a primary key

            modelBuilder.Entity<PlayerPhysicalTesting>()
           .Property(p => p.HeightM)
           .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
           .Property(p => p.BodyWeightKg)
           .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.B_JumpDBL)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.RMBench)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.RMSquat)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.PullUps)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.Bronco)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.TenMSprint)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.Test505L)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.Test505R)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.DiffLR)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.SquatRelative)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.BenchRelative)
          .HasColumnType("decimal(10,3)");

            modelBuilder.Entity<PlayerPhysicalTesting>()
          .Property(p => p.AgilityT)
          .HasColumnType("decimal(10,3)");

        }




    }


}
