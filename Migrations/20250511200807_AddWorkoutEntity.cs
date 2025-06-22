using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcerciseID = table.Column<int>(type: "int", nullable: false),
                    ExcerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false),
                    PerWeek = table.Column<int>(type: "int", nullable: true),
                    reps = table.Column<int>(type: "int", nullable: false),
                    RM = table.Column<int>(type: "int", nullable: true),
                    RPE = table.Column<double>(type: "float", nullable: true),
                    Velocity = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_SchemaID",
                table: "Workouts",
                column: "SchemaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Workouts");
        }
    }
}
