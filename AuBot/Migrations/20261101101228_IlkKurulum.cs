using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuBot.Migrations
{
    /// <inheritdoc />
    public partial class IlkKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menuler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateOnly>(type: "date", nullable: false),
                    Ogun = table.Column<int>(type: "int", nullable: false),
                    ToplamKalori = table.Column<int>(type: "int", nullable: true),
                    TatilMi = table.Column<bool>(type: "bit", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menuler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Yemekler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AramaAdi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    KategoriId = table.Column<int>(type: "int", nullable: false),
                    Kalori = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yemekler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yemekler_Kategoriler_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Degerlendirmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    YemekId = table.Column<int>(type: "int", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    Yorum = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Onaylandi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degerlendirmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_Menuler_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menuler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_Yemekler_YemekId",
                        column: x => x.YemekId,
                        principalTable: "Yemekler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuYemekleri",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    YemekId = table.Column<int>(type: "int", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuYemekleri", x => new { x.MenuId, x.YemekId });
                    table.ForeignKey(
                        name: "FK_MenuYemekleri_Menuler_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menuler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuYemekleri_Yemekler_YemekId",
                        column: x => x.YemekId,
                        principalTable: "Yemekler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Kategoriler",
                columns: new[] { "Id", "Ad", "Sira" },
                values: new object[,]
                {
                    { 1, "Çorba", 1 },
                    { 2, "Ana Yemek", 2 },
                    { 3, "Yardımcı Yemek", 3 },
                    { 4, "Tatlı / Salata / İçecek", 4 }
                });

            migrationBuilder.InsertData(
                table: "Yemekler",
                columns: new[] { "Id", "Ad", "AramaAdi", "Kalori", "KategoriId" },
                values: new object[,]
                {
                    { 1, "Mahluta Çorba", "MAHLUTA ÇORBA", 204, 1 },
                    { 2, "İzmir Köfte", "İZMİR KÖFTE", 712, 2 },
                    { 3, "Bulgur Pilavı", "BULGUR PİLAVI", 373, 3 },
                    { 4, "Cacık", "CACIK", 94, 4 },
                    { 5, "Tarhana Çorba", "TARHANA ÇORBA", 176, 1 },
                    { 6, "İskender Kebap", "İSKENDER KEBAP", 567, 2 },
                    { 7, "Soslu Makarna", "SOSLU MAKARNA", 456, 3 },
                    { 8, "Trileçe", "TRİLEÇE", 375, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_MenuId",
                table: "Degerlendirmeler",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_YemekId",
                table: "Degerlendirmeler",
                column: "YemekId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuYemekleri_YemekId",
                table: "MenuYemekleri",
                column: "YemekId");

            migrationBuilder.CreateIndex(
                name: "IX_Yemekler_KategoriId",
                table: "Yemekler",
                column: "KategoriId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Degerlendirmeler");

            migrationBuilder.DropTable(
                name: "MenuYemekleri");

            migrationBuilder.DropTable(
                name: "Menuler");

            migrationBuilder.DropTable(
                name: "Yemekler");

            migrationBuilder.DropTable(
                name: "Kategoriler");
        }
    }
}
