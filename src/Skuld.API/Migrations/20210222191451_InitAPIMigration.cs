using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skuld.API.Migrations
{
    public partial class InitAPIMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    ClientSecret = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Token = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    OwnerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    TotalAPICalls = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens");
        }
    }
}
