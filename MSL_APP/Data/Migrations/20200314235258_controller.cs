using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSL_APP.Data.Migrations
{
    public partial class controller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "AspNetUsers",
                newName: "StudentId");

            migrationBuilder.AddColumn<int>(
                name: "EligibleId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EligibleStudent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StudentID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    StudentEmail = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EligibleStudent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductKeyLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    UsedKeyCount = table.Column<int>(nullable: false),
                    ActiveStatus = table.Column<string>(nullable: true),
                    DownloadLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductKey",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NameId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    UsedKey = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductKey_ProductName_NameId",
                        column: x => x.NameId,
                        principalTable: "ProductName",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EligibleId",
                table: "AspNetUsers",
                column: "EligibleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductKey_NameId",
                table: "ProductKey",
                column: "NameId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EligibleStudent_EligibleId",
                table: "AspNetUsers",
                column: "EligibleId",
                principalTable: "EligibleStudent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EligibleStudent_EligibleId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EligibleStudent");

            migrationBuilder.DropTable(
                name: "ProductKey");

            migrationBuilder.DropTable(
                name: "ProductKeyLog");

            migrationBuilder.DropTable(
                name: "ProductName");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EligibleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EligibleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "AspNetUsers",
                newName: "StudentID");
        }
    }
}
