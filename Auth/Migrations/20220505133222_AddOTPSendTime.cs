using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Auth.Migrations
{
    public partial class AddOTPSendTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OTPSendTime",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OTPSendTime",
                table: "Users");
        }
    }
}
