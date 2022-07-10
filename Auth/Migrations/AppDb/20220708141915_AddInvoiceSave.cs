using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Auth.Migrations.AppDb
{
    public partial class AddInvoiceSave : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "_OrderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "About",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_About", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdvPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvPosition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOnHome = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOnMenuMain = table.Column<bool>(type: "bit", nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FAQ",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQ", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceSave",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(230)", maxLength: 230, nullable: true),
                    MaSoBiMat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InvSerial = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InvNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    InvDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaxPer = table.Column<double>(type: "float", nullable: false),
                    InvAmountWithoutTax = table.Column<double>(type: "float", nullable: false),
                    InvCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    InvRemarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    InvAmount = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSave", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOnHome = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOnMenuMain = table.Column<bool>(type: "bit", nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsCategories_NewsCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "NewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParamSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParamKey = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParamValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParamSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupIdList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price1 = table.Column<double>(type: "float", nullable: false),
                    PriceText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "_Order",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CookieID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<double>(type: "float", nullable: true),
                    FeeShip = table.Column<double>(type: "float", nullable: true),
                    IsAgree = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Order__OrderStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "_OrderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Adv",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Pos = table.Column<int>(type: "int", nullable: false),
                    Img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AdvScript = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adv", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adv_AdvPosition_Pos",
                        column: x => x.Pos,
                        principalTable: "AdvPosition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryMain = table.Column<long>(type: "bigint", nullable: true),
                    CategoryReference = table.Column<long>(type: "bigint", nullable: true),
                    Img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImgSlide1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImgSlide2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImgSlide3 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImgSlide4 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImgSlide5 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsNews = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    Quota = table.Column<int>(type: "int", nullable: false),
                    IsSale = table.Column<bool>(type: "bit", nullable: false),
                    IsHot = table.Column<bool>(type: "bit", nullable: false),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Categories_CategoryMain",
                        column: x => x.CategoryMain,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Product_Categories_CategoryReference",
                        column: x => x.CategoryReference,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryMain = table.Column<long>(type: "bigint", nullable: false),
                    CategoryReference = table.Column<long>(type: "bigint", nullable: true),
                    TagsReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImgBanner = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsNews = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserCreator = table.Column<long>(type: "bigint", nullable: false),
                    DateCreator = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModify = table.Column<long>(type: "bigint", nullable: true),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserDeleted = table.Column<long>(type: "bigint", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaTitle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaKeyword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaBox = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MetaRobotTag = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Article_NewsCategories_CategoryMain",
                        column: x => x.CategoryMain,
                        principalTable: "NewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Article_NewsCategories_CategoryReference",
                        column: x => x.CategoryReference,
                        principalTable: "NewsCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fullname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsCompany = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: true),
                    ContactDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    CookieID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAgree = table.Column<bool>(type: "bit", nullable: true),
                    IsSave = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contact__OrderStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "_OrderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contact_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "_OrderItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Units = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK__OrderItem__Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__OrderItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX__Order_StatusId",
                table: "_Order",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX__OrderItem_OrderId",
                table: "_OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX__OrderItem_ProductId",
                table: "_OrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Adv_Pos",
                table: "Adv",
                column: "Pos");

            migrationBuilder.CreateIndex(
                name: "IX_Article_CategoryMain",
                table: "Article",
                column: "CategoryMain");

            migrationBuilder.CreateIndex(
                name: "IX_Article_CategoryReference",
                table: "Article",
                column: "CategoryReference");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ServiceId",
                table: "Contact",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_StatusId",
                table: "Contact",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsCategories_ParentId",
                table: "NewsCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryMain",
                table: "Product",
                column: "CategoryMain");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryReference",
                table: "Product",
                column: "CategoryReference");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_Address");

            migrationBuilder.DropTable(
                name: "_OrderItem");

            migrationBuilder.DropTable(
                name: "About");

            migrationBuilder.DropTable(
                name: "Adv");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "FAQ");

            migrationBuilder.DropTable(
                name: "InvoiceSave");

            migrationBuilder.DropTable(
                name: "ParamSettings");

            migrationBuilder.DropTable(
                name: "_Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "AdvPosition");

            migrationBuilder.DropTable(
                name: "NewsCategories");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "_OrderStatus");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
