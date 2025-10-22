using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieHubMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddMoviImageFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cinemas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cinemas");
        }
    }
}
