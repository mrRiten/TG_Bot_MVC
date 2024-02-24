using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace TG_Bot_MVC.Migrations
{
    /// <inheritdoc />
    public partial class Start : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    IdDepartment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DepartmentName = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.IdDepartment);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    IdStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StatusName = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.IdStatus);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WeekOfSchedules",
                columns: table => new
                {
                    IdWeekOfSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    WeekOfScheduleName = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekOfSchedules", x => x.IdWeekOfSchedule);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    IdGroup = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GroupName = table.Column<string>(type: "longtext", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.IdGroup);
                    table.ForeignKey(
                        name: "FK_Groups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "IdDepartment",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "longtext", nullable: false),
                    UserTGId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_Users_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CorrectSchedules",
                columns: table => new
                {
                    IdCorSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    WeekOfScheduleId = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false),
                    SerializeDataLessons = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectSchedules", x => x.IdCorSchedule);
                    table.ForeignKey(
                        name: "FK_CorrectSchedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "IdGroup",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorrectSchedules_WeekOfSchedules_WeekOfScheduleId",
                        column: x => x.WeekOfScheduleId,
                        principalTable: "WeekOfSchedules",
                        principalColumn: "IdWeekOfSchedule",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DefaultSchedules",
                columns: table => new
                {
                    IdDefSchedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    WeekOfScheduleId = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false),
                    SerializeDataLessons = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultSchedules", x => x.IdDefSchedule);
                    table.ForeignKey(
                        name: "FK_DefaultSchedules_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "IdGroup",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DefaultSchedules_WeekOfSchedules_WeekOfScheduleId",
                        column: x => x.WeekOfScheduleId,
                        principalTable: "WeekOfSchedules",
                        principalColumn: "IdWeekOfSchedule",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReplasementLessons",
                columns: table => new
                {
                    IdRepLesson = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    WeekOfScheduleId = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false),
                    SerializeDataLessons = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplasementLessons", x => x.IdRepLesson);
                    table.ForeignKey(
                        name: "FK_ReplasementLessons_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "IdGroup",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReplasementLessons_WeekOfSchedules_WeekOfScheduleId",
                        column: x => x.WeekOfScheduleId,
                        principalTable: "WeekOfSchedules",
                        principalColumn: "IdWeekOfSchedule",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    IdSetting = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    isMailing = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TimeOfLessons = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.IdSetting);
                    table.ForeignKey(
                        name: "FK_Settings_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "IdGroup",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Settings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectSchedules_GroupId",
                table: "CorrectSchedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectSchedules_WeekOfScheduleId",
                table: "CorrectSchedules",
                column: "WeekOfScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultSchedules_GroupId",
                table: "DefaultSchedules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultSchedules_WeekOfScheduleId",
                table: "DefaultSchedules",
                column: "WeekOfScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DepartmentId",
                table: "Groups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplasementLessons_GroupId",
                table: "ReplasementLessons",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplasementLessons_WeekOfScheduleId",
                table: "ReplasementLessons",
                column: "WeekOfScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_GroupId",
                table: "Settings",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StatusId",
                table: "Users",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrectSchedules");

            migrationBuilder.DropTable(
                name: "DefaultSchedules");

            migrationBuilder.DropTable(
                name: "ReplasementLessons");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "WeekOfSchedules");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Statuses");
        }
    }
}
