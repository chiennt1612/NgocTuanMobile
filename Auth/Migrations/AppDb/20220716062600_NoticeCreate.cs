using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Auth.Migrations.AppDb
{
    public partial class NoticeCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "InvAmount",
                table: "InvoiceSave",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Contact",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Img2",
                table: "Contact",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Noted",
                table: "Contact",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Noted2",
                table: "Contact",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserNotice",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DeviceID = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    OS = table.Column<int>(type: "int", nullable: false),
                    NoticeTypeId = table.Column<int>(type: "int", nullable: false),
                    NoticeTypeName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsHTML = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotice", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotice_DeviceID",
                table: "UserNotice",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotice_Username",
                table: "UserNotice",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNotice");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Img2",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Noted",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "Noted2",
                table: "Contact");

            migrationBuilder.AlterColumn<int>(
                name: "InvAmount",
                table: "InvoiceSave",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
