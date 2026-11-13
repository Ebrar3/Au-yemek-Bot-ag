using AuBot.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Data;

/// <summary>
/// AÜ Yemekhane Asistanı (Aü Bot) Entity Framework Core veritabanı bağlamı.
/// Tablo ilişkilerini, ASP.NET Core Identity tablolarını ve
/// başlangıç (seed) kategori/yemek verilerini yapılandırır.
/// </summary>
public class AuBotDbContext : IdentityDbContext<IdentityUser>
{
    public AuBotDbContext(DbContextOptions<AuBotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Kategori> Kategoriler { get; set; } = default!;
    public DbSet<Yemek> Yemekler { get; set; } = default!;
    public DbSet<Menu> Menuler { get; set; } = default!;
    public DbSet<MenuYemek> MenuYemekleri { get; set; } = default!;
    public DbSet<Degerlendirme> Degerlendirmeler { get; set; } = default!;
    public DbSet<MenuYukleme> MenuYuklemeleri { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Menü - Yemek çoka-çok (N-N) ara tablo birleşik birincil anahtarı
        modelBuilder.Entity<MenuYemek>()
            .HasKey(my => new { my.MenuId, my.YemekId });

        // Menü Yükleme (1-N) Menü ilişkisi: yükleme kaydı silinirse menüler silinmesin
        modelBuilder.Entity<Menu>()
            .HasOne(m => m.MenuYukleme)
            .WithMany(my => my.Menuler)
            .HasForeignKey(m => m.MenuYuklemeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Değerlendirme ilişkilerinde çoklu cascade delete çakışmasını önle
        modelBuilder.Entity<Degerlendirme>()
            .HasOne(d => d.Menu)
            .WithMany(m => m.Degerlendirmeler)
            .HasForeignKey(d => d.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Degerlendirme>()
            .HasOne(d => d.Yemek)
            .WithMany(y => y.Degerlendirmeler)
            .HasForeignKey(d => d.YemekId)
            .OnDelete(DeleteBehavior.Restrict);

        // Başlangıç (Seed) Kategorileri (Excel menüsündeki 4 sütun sırasına uygun)
        SeedKategoriler(modelBuilder);

        // Başlangıç (Seed) Örnek Yemekler (Ankara Üniversitesi Ekim 2026 menüsünden)
        SeedYemekler(modelBuilder);
    }

    private static void SeedKategoriler(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kategori>().HasData(
            new Kategori { Id = 1, Ad = "Çorba", Sira = 1 },
            new Kategori { Id = 2, Ad = "Ana Yemek", Sira = 2 },
            new Kategori { Id = 3, Ad = "Yardımcı Yemek", Sira = 3 },
            new Kategori { Id = 4, Ad = "Tatlı / Salata / İçecek", Sira = 4 }
        );
    }

    private static void SeedYemekler(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Yemek>().HasData(
            new Yemek { Id = 1, Ad = "Mahluta Çorba", AramaAdi = "MAHLUTA ÇORBA", KategoriId = 1, Kalori = 204 },
            new Yemek { Id = 2, Ad = "İzmir Köfte", AramaAdi = "İZMİR KÖFTE", KategoriId = 2, Kalori = 712 },
            new Yemek { Id = 3, Ad = "Bulgur Pilavı", AramaAdi = "BULGUR PİLAVI", KategoriId = 3, Kalori = 373 },
            new Yemek { Id = 4, Ad = "Cacık", AramaAdi = "CACIK", KategoriId = 4, Kalori = 94 },
            new Yemek { Id = 5, Ad = "Tarhana Çorba", AramaAdi = "TARHANA ÇORBA", KategoriId = 1, Kalori = 176 },
            new Yemek { Id = 6, Ad = "İskender Kebap", AramaAdi = "İSKENDER KEBAP", KategoriId = 2, Kalori = 567 },
            new Yemek { Id = 7, Ad = "Soslu Makarna", AramaAdi = "SOSLU MAKARNA", KategoriId = 3, Kalori = 456 },
            new Yemek { Id = 8, Ad = "Trileçe", AramaAdi = "TRİLEÇE", KategoriId = 4, Kalori = 375 }
        );
    }
}
