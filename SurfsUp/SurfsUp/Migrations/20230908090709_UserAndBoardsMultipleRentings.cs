using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfsUp.Migrations
{
    public partial class UserAndBoardsMultipleRentings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Renting_BoardId",
                table: "Renting");

            migrationBuilder.DropIndex(
                name: "IX_Renting_SurfsUpUserId",
                table: "Renting");

            migrationBuilder.CreateIndex(
                name: "IX_Renting_BoardId",
                table: "Renting",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Renting_SurfsUpUserId",
                table: "Renting",
                column: "SurfsUpUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Renting_BoardId",
                table: "Renting");

            migrationBuilder.DropIndex(
                name: "IX_Renting_SurfsUpUserId",
                table: "Renting");

            migrationBuilder.CreateIndex(
                name: "IX_Renting_BoardId",
                table: "Renting",
                column: "BoardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Renting_SurfsUpUserId",
                table: "Renting",
                column: "SurfsUpUserId",
                unique: true);
        }
    }
}
