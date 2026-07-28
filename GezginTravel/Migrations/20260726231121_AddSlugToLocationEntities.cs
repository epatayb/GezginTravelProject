using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GezginTravel.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToLocationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Cities_CityId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Countries_CountryId",
                table: "Blogs");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Cities_CityId",
                table: "Blogs",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Countries_CountryId",
                table: "Blogs",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Cities_CityId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Countries_CountryId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Cities");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Cities_CityId",
                table: "Blogs",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Countries_CountryId",
                table: "Blogs",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");
        }
    }
}
