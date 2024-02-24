using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TG_Bot_MVC.Migrations
{
    /// <inheritdoc />
    public partial class NullForGroup_DepId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Departments_DepartmentId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Groups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Departments_DepartmentId",
                table: "Groups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "IdDepartment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Departments_DepartmentId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Departments_DepartmentId",
                table: "Groups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "IdDepartment",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
