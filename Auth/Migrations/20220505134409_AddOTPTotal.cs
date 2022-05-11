using Microsoft.EntityFrameworkCore.Migrations;

namespace Auth.Migrations
{
    public partial class AddOTPTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalOTP",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOTP",
                table: "Users");
        }
    }
}
