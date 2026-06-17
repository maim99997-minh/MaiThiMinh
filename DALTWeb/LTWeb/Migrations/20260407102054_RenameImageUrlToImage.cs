using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTWeb.Migrations
{
    /// <inheritdoc />
    public partial class RenameImageUrlToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "SeafoodItems",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "SeafoodItems",
                newName: "ImageUrl");
        }
    }
}
