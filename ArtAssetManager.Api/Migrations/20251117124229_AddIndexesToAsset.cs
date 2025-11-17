using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAssetManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Assets_DateAdded",
                table: "Assets",
                column: "DateAdded");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_DominantColor",
                table: "Assets",
                column: "DominantColor");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_FileHash",
                table: "Assets",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_FileSize",
                table: "Assets",
                column: "FileSize");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_FileType",
                table: "Assets",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_HasAlphaChannel",
                table: "Assets",
                column: "HasAlphaChannel");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ImageHeight",
                table: "Assets",
                column: "ImageHeight");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ImageWidth",
                table: "Assets",
                column: "ImageWidth");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LastModified",
                table: "Assets",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Rating",
                table: "Assets",
                column: "Rating");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_DateAdded",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_DominantColor",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_FileHash",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_FileSize",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_FileType",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_HasAlphaChannel",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ImageHeight",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ImageWidth",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_LastModified",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_Rating",
                table: "Assets");
        }
    }
}
