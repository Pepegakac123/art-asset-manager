using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArtAssetManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetadataJson",
                table: "Assets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Assets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MaterialSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CoverAssetId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomCoverUrl = table.Column<string>(type: "TEXT", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaterialSets_Assets_CoverAssetId",
                        column: x => x.CoverAssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssetMaterialSet",
                columns: table => new
                {
                    AssetsId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialSetsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMaterialSet", x => new { x.AssetsId, x.MaterialSetsId });
                    table.ForeignKey(
                        name: "FK_AssetMaterialSet_Assets_AssetsId",
                        column: x => x.AssetsId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetMaterialSet_MaterialSets_MaterialSetsId",
                        column: x => x.MaterialSetsId,
                        principalTable: "MaterialSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetMaterialSet_MaterialSetsId",
                table: "AssetMaterialSet",
                column: "MaterialSetsId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSets_CoverAssetId",
                table: "MaterialSets",
                column: "CoverAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialSets_Name",
                table: "MaterialSets",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetMaterialSet");

            migrationBuilder.DropTable(
                name: "MaterialSets");

            migrationBuilder.DropColumn(
                name: "MetadataJson",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Assets");
        }
    }
}
