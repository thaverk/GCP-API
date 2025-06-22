using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class MissedSessionsEdited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "DateAssigned",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "DateCompleted",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "ExcerciseID",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "ExcerciseName",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "PAExerciseID",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "PerWeek",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "RM",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "RPE",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "SchemaID",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "Velocity",
                table: "MissedSessions");

            migrationBuilder.DropColumn(
                name: "WeightUsed",
                table: "MissedSessions");

            migrationBuilder.RenameColumn(
                name: "reps",
                table: "MissedSessions",
                newName: "GroupSessionID");

            migrationBuilder.AddColumn<int>(
                name: "SessionSetID",
                table: "Sessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionSetID",
                table: "SessionHistory",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionSetID",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SessionSetID",
                table: "SessionHistory");

            migrationBuilder.RenameColumn(
                name: "GroupSessionID",
                table: "MissedSessions",
                newName: "reps");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "MissedSessions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAssigned",
                table: "MissedSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCompleted",
                table: "MissedSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExcerciseID",
                table: "MissedSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExcerciseName",
                table: "MissedSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PAExerciseID",
                table: "MissedSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerWeek",
                table: "MissedSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RM",
                table: "MissedSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RPE",
                table: "MissedSessions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "MissedSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SchemaID",
                table: "MissedSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Velocity",
                table: "MissedSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "WeightUsed",
                table: "MissedSessions",
                type: "real",
                nullable: true);
        }
    }
}
