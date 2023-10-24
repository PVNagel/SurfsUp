using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfsUp.Migrations
{
    public partial class GuestUserAdded2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestUserIp",
                table: "Renting",
                type: "nvarchar(45)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GuestUsers",
                columns: table => new
                {
                    Ip = table.Column<string>(type: "nvarchar(45)", nullable: false),
                    RentingsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestUsers", x => x.Ip);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Renting_GuestUserIp",
                table: "Renting",
                column: "GuestUserIp");

            migrationBuilder.AddForeignKey(
                name: "FK_Renting_GuestUsers_GuestUserIp",
                table: "Renting",
                column: "GuestUserIp",
                principalTable: "GuestUsers",
                principalColumn: "Ip");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renting_GuestUsers_GuestUserIp",
                table: "Renting");

            migrationBuilder.DropTable(
                name: "GuestUsers");

            migrationBuilder.DropIndex(
                name: "IX_Renting_GuestUserIp",
                table: "Renting");

            migrationBuilder.DropColumn(
                name: "GuestUserIp",
                table: "Renting");
        }
    }
}
