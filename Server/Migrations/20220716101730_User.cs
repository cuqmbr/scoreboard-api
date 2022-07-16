using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    public partial class User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Scoreboard",
                table: "Scoreboard");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Scoreboard");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Scoreboard",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Scoreboard",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scoreboard",
                table: "Scoreboard",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scoreboard_UserId",
                table: "Scoreboard",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scoreboard_Users_UserId",
                table: "Scoreboard",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scoreboard_Users_UserId",
                table: "Scoreboard");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Scoreboard",
                table: "Scoreboard");

            migrationBuilder.DropIndex(
                name: "IX_Scoreboard_UserId",
                table: "Scoreboard");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Scoreboard");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Scoreboard");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Scoreboard",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Scoreboard",
                table: "Scoreboard",
                column: "Username");
        }
    }
}
