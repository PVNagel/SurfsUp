using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurfsUp.Migrations
{
    public partial class GuestUserAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renting_AspNetUsers_SurfsUpUserId",
                table: "Renting");

            migrationBuilder.AlterColumn<string>(
                name: "SurfsUpUserId",
                table: "Renting",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Renting_AspNetUsers_SurfsUpUserId",
                table: "Renting",
                column: "SurfsUpUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Renting_AspNetUsers_SurfsUpUserId",
                table: "Renting");

            migrationBuilder.AlterColumn<string>(
                name: "SurfsUpUserId",
                table: "Renting",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Renting_AspNetUsers_SurfsUpUserId",
                table: "Renting",
                column: "SurfsUpUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
