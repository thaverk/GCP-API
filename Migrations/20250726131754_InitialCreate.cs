using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAllDay = table.Column<bool>(type: "boolean", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "text", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Excercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    YoutubeURL = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    BodyPart = table.Column<string>(type: "text", nullable: true),
                    PrimaryGroup = table.Column<string>(type: "text", nullable: true),
                    SecondaryGroup1 = table.Column<string>(type: "text", nullable: true),
                    SecondaryGroup2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupProgrammes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    programmeId = table.Column<int>(type: "integer", nullable: false),
                    GroupID = table.Column<int>(type: "integer", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupProgrammes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PAExercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    AddNote = table.Column<string>(type: "text", nullable: true),
                    ProgramID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAExercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PercentRM_Mapping",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PercentRM = table.Column<double>(type: "double precision", nullable: false),
                    RPE = table.Column<double>(type: "double precision", nullable: false),
                    RIR = table.Column<string>(type: "text", nullable: false),
                    VelocityRange = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PercentRM_Mapping", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupID = table.Column<int>(type: "integer", nullable: true),
                    UserID = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Weeks = table.Column<int>(type: "integer", nullable: false),
                    OccursOn = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SRSchema",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    SetWeek = table.Column<int>(type: "integer", nullable: false),
                    RM_Increase = table.Column<int>(type: "integer", nullable: false),
                    RPE_Increase = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SRSchema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SupersetExcerciseAttr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupersetID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupersetExcerciseAttr", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Supersets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restperiod = table.Column<int>(type: "integer", nullable: false),
                    PAExerciseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supersets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserExercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    BodyWeight = table.Column<float>(type: "real", nullable: true),
                    FirstSignIn = table.Column<bool>(type: "boolean", nullable: true),
                    Height = table.Column<float>(type: "real", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userSchemaAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserSRSchemaId = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    PercentRM = table.Column<int>(type: "integer", nullable: false),
                    RPE = table.Column<double>(type: "double precision", nullable: false),
                    Vel = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSchemaAttributes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserID = table.Column<string>(type: "text", nullable: false),
                    ProgramId = table.Column<int>(type: "integer", nullable: false),
                    GroupProgrammeId = table.Column<int>(type: "integer", nullable: false),
                    TotalSessions = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userSRSchemas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    SetWeek = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSRSchemas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupSession",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventID = table.Column<int>(type: "integer", nullable: false),
                    GroupProgrammeID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSession", x => x.id);
                    table.ForeignKey(
                        name: "FK_GroupSession_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupSession_GroupProgrammes_GroupProgrammeID",
                        column: x => x.GroupProgrammeID,
                        principalTable: "GroupProgrammes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetID = table.Column<int>(type: "integer", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: true),
                    ExcercisesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetAttributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_SetAttributes_Excercises_ExcercisesId",
                        column: x => x.ExcercisesId,
                        principalTable: "Excercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetAttributes_PAExercises_SetID",
                        column: x => x.SetID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverAllSets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverAllSets", x => x.id);
                    table.ForeignKey(
                        name: "FK_OverAllSets_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramAttributes_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemaAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SchemaID = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    PercentRM = table.Column<double>(type: "double precision", nullable: false),
                    RPE = table.Column<double>(type: "double precision", nullable: false),
                    Vel = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaAttributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_SchemaAttributes_SRSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "SRSchema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetExcercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExcerciseID = table.Column<int>(type: "integer", nullable: false),
                    ExcerciseName = table.Column<string>(type: "text", nullable: false),
                    SchemaID = table.Column<int>(type: "integer", nullable: false),
                    PerWeek = table.Column<int>(type: "integer", nullable: true),
                    reps = table.Column<int>(type: "integer", nullable: false),
                    RM = table.Column<double>(type: "double precision", nullable: true),
                    RPE = table.Column<double>(type: "double precision", nullable: true),
                    Velocity = table.Column<string>(type: "text", nullable: true),
                    PAExerciseID = table.Column<int>(type: "integer", nullable: false),
                    SupersetID = table.Column<int>(type: "integer", nullable: true),
                    Day = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetExcercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_SetExcercises_Excercises_ExcerciseID",
                        column: x => x.ExcerciseID,
                        principalTable: "Excercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetExcercises_PAExercises_PAExerciseID",
                        column: x => x.PAExerciseID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetExcercises_SRSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "SRSchema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    ExcerciseID = table.Column<int>(type: "integer", nullable: false),
                    ExcerciseName = table.Column<string>(type: "text", nullable: false),
                    SchemaID = table.Column<int>(type: "integer", nullable: false),
                    PerWeek = table.Column<int>(type: "integer", nullable: true),
                    reps = table.Column<int>(type: "integer", nullable: false),
                    RM = table.Column<int>(type: "integer", nullable: true),
                    RPE = table.Column<double>(type: "double precision", nullable: true),
                    Velocity = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Workouts_SRSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "SRSchema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissedSessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    GroupSessionID = table.Column<int>(type: "integer", nullable: false),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    GroupProgrammeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissedSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_MissedSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionHistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SessionSetID = table.Column<int>(type: "integer", nullable: true),
                    GroupSessionID = table.Column<int>(type: "integer", nullable: true),
                    ExcerciseID = table.Column<int>(type: "integer", nullable: false),
                    ExcerciseName = table.Column<string>(type: "text", nullable: false),
                    SchemaID = table.Column<int>(type: "integer", nullable: false),
                    PerWeek = table.Column<int>(type: "integer", nullable: true),
                    reps = table.Column<int>(type: "integer", nullable: false),
                    RM = table.Column<double>(type: "double precision", nullable: true),
                    RPE = table.Column<double>(type: "double precision", nullable: true),
                    Velocity = table.Column<string>(type: "text", nullable: true),
                    PAExerciseID = table.Column<int>(type: "integer", nullable: false),
                    WeightUsed = table.Column<float>(type: "real", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    GroupProgrammeId = table.Column<int>(type: "integer", nullable: false),
                    RecommendedWeight = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionHistory", x => x.id);
                    table.ForeignKey(
                        name: "FK_SessionHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SessionSetID = table.Column<int>(type: "integer", nullable: true),
                    GroupSessionID = table.Column<int>(type: "integer", nullable: true),
                    ExcerciseID = table.Column<int>(type: "integer", nullable: false),
                    ExcerciseName = table.Column<string>(type: "text", nullable: false),
                    SchemaID = table.Column<int>(type: "integer", nullable: false),
                    PerWeek = table.Column<int>(type: "integer", nullable: true),
                    reps = table.Column<int>(type: "integer", nullable: false),
                    RM = table.Column<double>(type: "double precision", nullable: true),
                    RPE = table.Column<double>(type: "double precision", nullable: true),
                    Velocity = table.Column<string>(type: "text", nullable: true),
                    PAExerciseID = table.Column<int>(type: "integer", nullable: false),
                    WeightUsed = table.Column<float>(type: "real", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProgramID = table.Column<int>(type: "integer", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    GroupProgrammeId = table.Column<int>(type: "integer", nullable: false),
                    RecommendedWeight = table.Column<double>(type: "double precision", nullable: true),
                    e1RM = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserID = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OvSetExcercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OverAllSetID = table.Column<int>(type: "integer", nullable: false),
                    ExcerciseID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvSetExcercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_OvSetExcercises_Excercises_ExcerciseID",
                        column: x => x.ExcerciseID,
                        principalTable: "Excercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OvSetExcercises_OverAllSets_OverAllSetID",
                        column: x => x.OverAllSetID,
                        principalTable: "OverAllSets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order = table.Column<string>(type: "text", nullable: true),
                    attributeID = table.Column<int>(type: "integer", nullable: false),
                    SetID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeSets_PAExercises_SetID",
                        column: x => x.SetID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeSets_ProgramAttributes_attributeID",
                        column: x => x.attributeID,
                        principalTable: "ProgramAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionSet",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    GroupSessionID = table.Column<int>(type: "integer", nullable: false),
                    ExerciseSetAttribID = table.Column<int>(type: "integer", nullable: false),
                    PAExerciseID = table.Column<int>(type: "integer", nullable: false),
                    PerWeek = table.Column<int>(type: "integer", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProgrammeID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSet", x => x.id);
                    table.ForeignKey(
                        name: "FK_SessionSet_GroupSession_GroupSessionID",
                        column: x => x.GroupSessionID,
                        principalTable: "GroupSession",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionSet_PAExercises_PAExerciseID",
                        column: x => x.PAExerciseID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionSet_Programs_ProgrammeID",
                        column: x => x.ProgrammeID,
                        principalTable: "Programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionSet_SetExcercises_ExerciseSetAttribID",
                        column: x => x.ExerciseSetAttribID,
                        principalTable: "SetExcercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionDates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionID = table.Column<int>(type: "integer", nullable: false),
                    EventID = table.Column<int>(type: "integer", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: false),
                    GroupProgrammeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionDates", x => x.id);
                    table.ForeignKey(
                        name: "FK_SessionDates_Events_EventID",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionDates_Sessions_SessionID",
                        column: x => x.SessionID,
                        principalTable: "Sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TeamID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_attributeID",
                table: "AttributeSets",
                column: "attributeID");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_SetID",
                table: "AttributeSets",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TeamID",
                table: "Groups",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSession_EventID",
                table: "GroupSession",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSession_GroupProgrammeID",
                table: "GroupSession",
                column: "GroupProgrammeID");

            migrationBuilder.CreateIndex(
                name: "IX_MissedSessions_UserId",
                table: "MissedSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OverAllSets_ProgramID",
                table: "OverAllSets",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_OvSetExcercises_ExcerciseID",
                table: "OvSetExcercises",
                column: "ExcerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_OvSetExcercises_OverAllSetID",
                table: "OvSetExcercises",
                column: "OverAllSetID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramAttributes_ProgramID",
                table: "ProgramAttributes",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaAttributes_SchemaID",
                table: "SchemaAttributes",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDates_EventID",
                table: "SessionDates",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDates_SessionID",
                table: "SessionDates",
                column: "SessionID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionHistory_UserId",
                table: "SessionHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSet_ExerciseSetAttribID",
                table: "SessionSet",
                column: "ExerciseSetAttribID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSet_GroupSessionID",
                table: "SessionSet",
                column: "GroupSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSet_PAExerciseID",
                table: "SessionSet",
                column: "PAExerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSet_ProgrammeID",
                table: "SessionSet",
                column: "ProgrammeID");

            migrationBuilder.CreateIndex(
                name: "IX_SetAttributes_ExcercisesId",
                table: "SetAttributes",
                column: "ExcercisesId");

            migrationBuilder.CreateIndex(
                name: "IX_SetAttributes_SetID",
                table: "SetAttributes",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_SetExcercises_ExcerciseID",
                table: "SetExcercises",
                column: "ExcerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_SetExcercises_PAExerciseID",
                table: "SetExcercises",
                column: "PAExerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_SetExcercises_SchemaID",
                table: "SetExcercises",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserID",
                table: "Teams",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_SchemaID",
                table: "Workouts",
                column: "SchemaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttributeSets");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "MissedSessions");

            migrationBuilder.DropTable(
                name: "OvSetExcercises");

            migrationBuilder.DropTable(
                name: "PercentRM_Mapping");

            migrationBuilder.DropTable(
                name: "SchemaAttributes");

            migrationBuilder.DropTable(
                name: "SessionDates");

            migrationBuilder.DropTable(
                name: "SessionHistory");

            migrationBuilder.DropTable(
                name: "SessionSet");

            migrationBuilder.DropTable(
                name: "SetAttributes");

            migrationBuilder.DropTable(
                name: "SupersetExcerciseAttr");

            migrationBuilder.DropTable(
                name: "Supersets");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "UserExercises");

            migrationBuilder.DropTable(
                name: "userSchemaAttributes");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "userSRSchemas");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "ProgramAttributes");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "OverAllSets");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "GroupSession");

            migrationBuilder.DropTable(
                name: "SetExcercises");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "GroupProgrammes");

            migrationBuilder.DropTable(
                name: "Excercises");

            migrationBuilder.DropTable(
                name: "PAExercises");

            migrationBuilder.DropTable(
                name: "SRSchema");
        }
    }
}
