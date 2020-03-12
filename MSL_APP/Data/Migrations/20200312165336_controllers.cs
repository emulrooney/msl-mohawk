using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSL_APP.Data.Migrations
{
    public partial class controllers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EligibleStudent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StudentID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    StudentEmail = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EligibleStudent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductKey",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NameId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    ActiveStatus = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductKey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductKeyLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    StudentId = table.Column<int>(nullable: false),
                    StudentEmail = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    ProductKey = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductKeyLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    QuantityLimit = table.Column<int>(nullable: false),
                    KeyCount = table.Column<int>(nullable: false),
                    ActiveStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredStudent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    EligibleId = table.Column<int>(nullable: true),
                    StudentId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    StudentEmail = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisteredStudent_EligibleStudent_EligibleId",
                        column: x => x.EligibleId,
                        principalTable: "EligibleStudent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegisteredStudent_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredStudent_EligibleId",
                table: "RegisteredStudent",
                column: "EligibleId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredStudent_UserId",
                table: "RegisteredStudent",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductKey");

            migrationBuilder.DropTable(
                name: "ProductKeyLog");

            migrationBuilder.DropTable(
                name: "ProductName");

            migrationBuilder.DropTable(
                name: "RegisteredStudent");

            migrationBuilder.DropTable(
                name: "EligibleStudent");
        }
    }
}
