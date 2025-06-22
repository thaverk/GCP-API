using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class MovedSessionstogroupfocused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupSession",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    GroupProgrammeID = table.Column<int>(type: "int", nullable: false)
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
                name: "SessionSet",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetNumber = table.Column<int>(type: "int", nullable: false),
                    GroupSessionID = table.Column<int>(type: "int", nullable: false),
                    ExerciseSetAttribID = table.Column<int>(type: "int", nullable: false),
                    PAExerciseID = table.Column<int>(type: "int", nullable: false),
                    PerWeek = table.Column<int>(type: "int", nullable: true),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProgrammeID = table.Column<int>(type: "int", nullable: false)
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
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupSession_EventID",
                table: "GroupSession",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_GroupSession_GroupProgrammeID",
                table: "GroupSession",
                column: "GroupProgrammeID");

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
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionSet");

            migrationBuilder.DropTable(
                name: "GroupSession");
        }
    }
}
