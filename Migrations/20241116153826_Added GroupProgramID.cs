using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddedGroupProgramID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupProgrammeId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupProgrammeId",
                table: "SessionDates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupProgrammeId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "GroupProgrammeId",
                table: "SessionDates");
        }
    }
}
