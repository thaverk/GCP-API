using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhasePlayWeb.Migrations
{
    /// <inheritdoc />
    public partial class newdatabaseerd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Excercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    category = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupProgrammes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    programmeId = table.Column<int>(type: "int", nullable: false),
                    GroupID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupProgrammes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PAExercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddNote = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAExercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weeks = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OccursOn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SRSchema",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetWeek = table.Column<int>(type: "int", nullable: false),
                    RM_Increase = table.Column<int>(type: "int", nullable: false),
                    RPE_Increase = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SRSchema", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SupersetExcerciseAttr",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupersetID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupersetExcerciseAttr", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Supersets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    restperiod = table.Column<int>(type: "int", nullable: false),
                    PAExerciseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supersets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserExercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userSchemaAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserSRSchemaId = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    PercentRM = table.Column<int>(type: "int", nullable: false),
                    RPE = table.Column<double>(type: "float", nullable: false),
                    Vel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSchemaAttributes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "userSRSchemas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSRSchemas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SetAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetID = table.Column<int>(type: "int", nullable: false),
                    SetNumber = table.Column<int>(type: "int", nullable: true),
                    ExcercisesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetAttributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_SetAttributes_Excercises_ExcercisesId",
                        column: x => x.ExcercisesId,
                        principalTable: "Excercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetAttributes_PAExercises_SetID",
                        column: x => x.SetID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverAllSets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverAllSets", x => x.id);
                    table.ForeignKey(
                        name: "FK_OverAllSets_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramID = table.Column<int>(type: "int", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramAttributes_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemaAttributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemaID = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    PercentRM = table.Column<int>(type: "int", nullable: false),
                    RPE = table.Column<double>(type: "float", nullable: false),
                    Vel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaAttributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_SchemaAttributes_SRSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "SRSchema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetExcercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcerciseID = table.Column<int>(type: "int", nullable: false),
                    ExcerciseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SchemaID = table.Column<int>(type: "int", nullable: false),
                    PerWeek = table.Column<int>(type: "int", nullable: true),
                    reps = table.Column<int>(type: "int", nullable: false),
                    RM = table.Column<int>(type: "int", nullable: true),
                    RPE = table.Column<double>(type: "float", nullable: true),
                    Velocity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAExerciseID = table.Column<int>(type: "int", nullable: false),
                    SupersetID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetExcercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_SetExcercises_Excercises_ExcerciseID",
                        column: x => x.ExcerciseID,
                        principalTable: "Excercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetExcercises_PAExercises_PAExerciseID",
                        column: x => x.PAExerciseID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SetExcercises_SRSchema_SchemaID",
                        column: x => x.SchemaID,
                        principalTable: "SRSchema",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OvSetExcercises",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OverAllSetID = table.Column<int>(type: "int", nullable: false),
                    ExcerciseID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvSetExcercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_OvSetExcercises_Excercises_ExcerciseID",
                        column: x => x.ExcerciseID,
                        principalTable: "Excercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OvSetExcercises_OverAllSets_OverAllSetID",
                        column: x => x.OverAllSetID,
                        principalTable: "OverAllSets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    attributeID = table.Column<int>(type: "int", nullable: false),
                    SetID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeSets_PAExercises_SetID",
                        column: x => x.SetID,
                        principalTable: "PAExercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeSets_ProgramAttributes_attributeID",
                        column: x => x.attributeID,
                        principalTable: "ProgramAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_attributeID",
                table: "AttributeSets",
                column: "attributeID");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeSets_SetID",
                table: "AttributeSets",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TeamID",
                table: "Groups",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_OverAllSets_ProgramID",
                table: "OverAllSets",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_OvSetExcercises_ExcerciseID",
                table: "OvSetExcercises",
                column: "ExcerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_OvSetExcercises_OverAllSetID",
                table: "OvSetExcercises",
                column: "OverAllSetID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramAttributes_ProgramID",
                table: "ProgramAttributes",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaAttributes_SchemaID",
                table: "SchemaAttributes",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_SetAttributes_ExcercisesId",
                table: "SetAttributes",
                column: "ExcercisesId");

            migrationBuilder.CreateIndex(
                name: "IX_SetAttributes_SetID",
                table: "SetAttributes",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_SetExcercises_ExcerciseID",
                table: "SetExcercises",
                column: "ExcerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_SetExcercises_PAExerciseID",
                table: "SetExcercises",
                column: "PAExerciseID");

            migrationBuilder.CreateIndex(
                name: "IX_SetExcercises_SchemaID",
                table: "SetExcercises",
                column: "SchemaID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_UserID",
                table: "Teams",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttributeSets");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "GroupProgrammes");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "OvSetExcercises");

            migrationBuilder.DropTable(
                name: "SchemaAttributes");

            migrationBuilder.DropTable(
                name: "SetAttributes");

            migrationBuilder.DropTable(
                name: "SetExcercises");

            migrationBuilder.DropTable(
                name: "SupersetExcerciseAttr");

            migrationBuilder.DropTable(
                name: "Supersets");

            migrationBuilder.DropTable(
                name: "UserExercises");

            migrationBuilder.DropTable(
                name: "userSchemaAttributes");

            migrationBuilder.DropTable(
                name: "userSRSchemas");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropTable(
                name: "ProgramAttributes");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "OverAllSets");

            migrationBuilder.DropTable(
                name: "Excercises");

            migrationBuilder.DropTable(
                name: "PAExercises");

            migrationBuilder.DropTable(
                name: "SRSchema");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Programs");
        }
    }
}
