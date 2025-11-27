using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAssetManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixScanFolderRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_ScanFolders_ScanFolderId",
                table: "Assets");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_ScanFolders_ScanFolderId",
                table: "Assets",
                column: "ScanFolderId",
                principalTable: "ScanFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_ScanFolders_ScanFolderId",
                table: "Assets");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_ScanFolders_ScanFolderId",
                table: "Assets",
                column: "ScanFolderId",
                principalTable: "ScanFolders",
                principalColumn: "Id");
        }
    }
}
