using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class SessionsandEventTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "BodyWeight",
                table: "AspNetUsers",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceRule = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionHistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExcerciseID = table.Column<int>(type: "int", nullable: false),
                    ExcerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false),
                    PerWeek = table.Column<int>(type: "int", nullable: true),
                    reps = table.Column<int>(type: "int", nullable: false),
                    RM = table.Column<int>(type: "int", nullable: true),
                    RPE = table.Column<double>(type: "float", nullable: true),
                    Velocity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAExerciseID = table.Column<int>(type: "int", nullable: false),
                    WeightUsed = table.Column<float>(type: "real", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionHistory", x => x.id);
                    table.ForeignKey(
                        name: "FK_SessionHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExcerciseID = table.Column<int>(type: "int", nullable: false),
                    ExcerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false),
                    PerWeek = table.Column<int>(type: "int", nullable: true),
                    reps = table.Column<int>(type: "int", nullable: false),
                    RM = table.Column<int>(type: "int", nullable: true),
                    RPE = table.Column<double>(type: "float", nullable: true),
                    Velocity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAExerciseID = table.Column<int>(type: "int", nullable: false),
                    WeightUsed = table.Column<float>(type: "real", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Sessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionDates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionID = table.Column<int>(type: "int", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionDates");

            migrationBuilder.DropTable(
                name: "SessionHistory");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropColumn(
                name: "BodyWeight",
                table: "AspNetUsers");
        }
    }
}
