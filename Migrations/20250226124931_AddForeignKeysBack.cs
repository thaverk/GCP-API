using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure data integrity for SessionSet
            migrationBuilder.Sql(@"
        DELETE FROM SessionSet
        WHERE ExerciseSetAttribID NOT IN (SELECT id FROM SetExcercises)
    ");

            // Ensure data integrity for SetExcercises
            migrationBuilder.Sql(@"
        DELETE FROM SetExcercises
        WHERE ExcerciseID NOT IN (SELECT Id FROM Excercises)
    ");

            // Drop existing constraints if they exist
            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT * 
            FROM sys.foreign_keys 
            WHERE name = 'FK_SessionSet_SetExcercises_ExerciseSetAttribID'
        )
        BEGIN
            ALTER TABLE SessionSet
            DROP CONSTRAINT FK_SessionSet_SetExcercises_ExerciseSetAttribID;
        END
    ");

            migrationBuilder.Sql(@"
        IF EXISTS (
            SELECT * 
            FROM sys.foreign_keys 
            WHERE name = 'FK_SetExcercises_Excercises_ExcerciseID'
        )
        BEGIN
            ALTER TABLE SetExcercises
            DROP CONSTRAINT FK_SetExcercises_Excercises_ExcerciseID;
        END
    ");

            // Now add the constraints
            migrationBuilder.AddForeignKey(
                name: "FK_SessionSet_SetExcercises_ExerciseSetAttribID",
                table: "SessionSet",
                column: "ExerciseSetAttribID",
                principalTable: "SetExcercises",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SetExcercises_Excercises_ExcerciseID",
                table: "SetExcercises",
                column: "ExcerciseID",
                principalTable: "Excercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionSet_SetExcercises_ExerciseSetAttribID",
                table: "SessionSet");

            migrationBuilder.DropForeignKey(
                name: "FK_SetExcercises_Excercises_ExcerciseID",
                table: "SetExcercises");
        }
    }
}
