using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TG_Bot_MVC.Migrations
{
    /// <inheritdoc />
    public partial class DelUserSettingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettingId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SettingId",
                table: "Users",
                type: "int",
                nullable: true);
        }
    }
}
