using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissedSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "SessionID",
                table: "SessionHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);


            migrationBuilder.CreateTable(
                name: "MissedSessions",
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
                    RM = table.Column<double>(type: "float", nullable: true),
                    RPE = table.Column<double>(type: "float", nullable: true),
                    Velocity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAExerciseID = table.Column<int>(type: "int", nullable: false),
                    WeightUsed = table.Column<float>(type: "real", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupProgrammeId = table.Column<int>(type: "int", nullable: false),
                    SessionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissedSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_MissedSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MissedSessions_UserId",
                table: "MissedSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "SessionID",
                table: "SessionHistory");


        }
    }
}
