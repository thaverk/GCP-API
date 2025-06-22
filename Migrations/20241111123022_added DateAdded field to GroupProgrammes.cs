using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class addedDateAddedfieldtoGroupProgrammes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Programs");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "GroupProgrammes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "GroupProgrammes");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Programs",
                type: "datetime2",
                nullable: true);
        }
    }
}
