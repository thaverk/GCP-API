using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class FirstSignIn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FirstSignIn",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstSignIn",
                table: "AspNetUsers");
        }
    }
}
