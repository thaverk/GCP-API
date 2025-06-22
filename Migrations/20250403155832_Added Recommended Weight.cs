using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddedRecommendedWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RecommendedWeight",
                table: "Sessions",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedWeight",
                table: "Sessions");
        }
    }
}
