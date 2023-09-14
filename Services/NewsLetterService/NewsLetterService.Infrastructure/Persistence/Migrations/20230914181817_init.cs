using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsLetterService.Infrastructure.Persistence.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personnel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NationalCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsLetterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonnelId = table.Column<int>(type: "int", nullable: false),
                    NewsLetterId = table.Column<int>(type: "int", nullable: false),
                    Act = table.Column<int>(type: "int", nullable: false),
                    DateOfAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLetterHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsLetterHistory_Personnel_PersonnelId",
                        column: x => x.PersonnelId,
                        principalTable: "Personnel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsLetterHistory_NewsLetterId_PersonnelId_Act",
                table: "NewsLetterHistory",
                columns: new[] { "NewsLetterId", "PersonnelId", "Act" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsLetterHistory_PersonnelId",
                table: "NewsLetterHistory",
                column: "PersonnelId");

            migrationBuilder.CreateIndex(
                name: "IX_Personnel_Id",
                table: "Personnel",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsLetterHistory");

            migrationBuilder.DropTable(
                name: "Personnel");
        }
    }
}
