using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class ExerciseCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "category",
                table: "Excercises",
                newName: "Category");

            migrationBuilder.AddColumn<int>(
                name: "GroupSessionID",
                table: "Sessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupSessionID",
                table: "SessionHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Excercises",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyPart",
                table: "Excercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryGroup",
                table: "Excercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryGroup1",
                table: "Excercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryGroup2",
                table: "Excercises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeURL",
                table: "Excercises",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupSessionID",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "GroupSessionID",
                table: "SessionHistory");

            migrationBuilder.DropColumn(
                name: "BodyPart",
                table: "Excercises");

            migrationBuilder.DropColumn(
                name: "PrimaryGroup",
                table: "Excercises");

            migrationBuilder.DropColumn(
                name: "SecondaryGroup1",
                table: "Excercises");

            migrationBuilder.DropColumn(
                name: "SecondaryGroup2",
                table: "Excercises");

            migrationBuilder.DropColumn(
                name: "YoutubeURL",
                table: "Excercises");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Excercises",
                newName: "category");

            migrationBuilder.AlterColumn<int>(
                name: "category",
                table: "Excercises",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
