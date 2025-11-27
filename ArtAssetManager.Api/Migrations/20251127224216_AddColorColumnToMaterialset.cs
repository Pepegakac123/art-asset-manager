using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAssetManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddColorColumnToMaterialset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomColor",
                table: "MaterialSets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomColor",
                table: "MaterialSets");
        }
    }
}
