using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuBot.Migrations
{
    /// <inheritdoc />
    public partial class MenuYuklemeleriTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuYuklemeId",
                table: "Menuler",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuYuklemeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DosyaAdi = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Ay = table.Column<int>(type: "int", nullable: false),
                    Yil = table.Column<int>(type: "int", nullable: false),
                    Ogun = table.Column<int>(type: "int", nullable: false),
                    YukleyenKullaniciId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    YuklemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EklenenMenuSayisi = table.Column<int>(type: "int", nullable: false),
                    GuncellenenMenuSayisi = table.Column<int>(type: "int", nullable: false),
                    TatilGunSayisi = table.Column<int>(type: "int", nullable: false),
                    YeniYemekSayisi = table.Column<int>(type: "int", nullable: false),
                    Uyarilar = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuYuklemeleri", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menuler_MenuYuklemeId",
                table: "Menuler",
                column: "MenuYuklemeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menuler_MenuYuklemeleri_MenuYuklemeId",
                table: "Menuler",
                column: "MenuYuklemeId",
                principalTable: "MenuYuklemeleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menuler_MenuYuklemeleri_MenuYuklemeId",
                table: "Menuler");

            migrationBuilder.DropTable(
                name: "MenuYuklemeleri");

            migrationBuilder.DropIndex(
                name: "IX_Menuler_MenuYuklemeId",
                table: "Menuler");

            migrationBuilder.DropColumn(
                name: "MenuYuklemeId",
                table: "Menuler");
        }
    }
}
