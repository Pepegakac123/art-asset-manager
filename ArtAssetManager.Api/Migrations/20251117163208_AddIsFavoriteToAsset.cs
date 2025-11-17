using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAssetManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFavoriteToAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "Assets",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "Assets");
        }
    }
}
