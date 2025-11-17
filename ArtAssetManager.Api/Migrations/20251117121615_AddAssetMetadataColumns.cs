using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAssetManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetMetadataColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MetadataJson",
                table: "Assets",
                newName: "DominantColor");

            migrationBuilder.AddColumn<int>(
                name: "BitDepth",
                table: "Assets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAlphaChannel",
                table: "Assets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageHeight",
                table: "Assets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageWidth",
                table: "Assets",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BitDepth",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "HasAlphaChannel",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ImageHeight",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ImageWidth",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "DominantColor",
                table: "Assets",
                newName: "MetadataJson");
        }
    }
}
