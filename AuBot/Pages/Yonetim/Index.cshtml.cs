using System.Globalization;
using AuBot.Data;
using AuBot.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AuBot.Pages.Yonetim;

public record AyMenuOzeti(
    int Yil,
    int Ay,
    string AyAdi,
    int OgleSayisi,
    int AksamSayisi,
    int VejetaryenSayisi,
    int TatilSayisi,
    int ToplamSayi);

public record KategoriDagilimi(
    int Id,
    string Ad,
    int? Sira,
    int YemekSayisi);

public class IndexModel : PageModel
{
    private static readonly CultureInfo TrKultur = new("tr-TR");
    private readonly AuBotDbContext _context;

    public IndexModel(AuBotDbContext context)
    {
        _context = context;
    }

    // Genel Sistem İstatistikleri
    public int ToplamMenuSayisi { get; set; }
    public int OgleMenuSayisi { get; set; }
    public int AksamMenuSayisi { get; set; }
    public int VejetaryenMenuSayisi { get; set; }
    public int TatilMenuSayisi { get; set; }

    public int ToplamYemekSayisi { get; set; }
    public int KaloriliYemekSayisi { get; set; }
    public int ToplamKategoriSayisi { get; set; }
    public int ToplamExcelYuklemeSayisi { get; set; }
    public int ToplamUyariSayisi { get; set; }

    // Bugünün Menü Durumu
    public DateOnly Bugun { get; set; }
    public string BugunFormatli { get; set; } = string.Empty;
    public Menu? BugunOgle { get; set; }
    public Menu? BugunAksam { get; set; }
    public Menu? BugunVejetaryen { get; set; }

    // Aylık Dağılım, Kategori Dağılımı ve Son Yüklemeler
    public List<AyMenuOzeti> AylikOzetler { get; set; } = new();
    public List<KategoriDagilimi> Kategoriler { get; set; } = new();
    public List<MenuYukleme> SonExcelYuklemeleri { get; set; } = new();

    public async Task OnGetAsync()
    {
        Bugun = DateOnly.FromDateTime(DateTime.Today);
        BugunFormatli = Bugun.ToDateTime(TimeOnly.MinValue).ToString("dd MMMM yyyy, dddd", TrKultur);

        // 1) Menü İstatistikleri
        var menuler = await _context.Menuler
            .AsNoTracking()
            .Select(m => new { m.Tarih, m.Ogun, m.TatilMi })
            .ToListAsync();

        ToplamMenuSayisi = menuler.Count;
        OgleMenuSayisi = menuler.Count(m => m.Ogun == OgunTuru.Ogle);
        AksamMenuSayisi = menuler.Count(m => m.Ogun == OgunTuru.Aksam);
        VejetaryenMenuSayisi = menuler.Count(m => m.Ogun == OgunTuru.Vejetaryen);
        TatilMenuSayisi = menuler.Count(m => m.TatilMi);

        // 2) Yemek ve Kategori İstatistikleri
        ToplamYemekSayisi = await _context.Yemekler.CountAsync();
        KaloriliYemekSayisi = await _context.Yemekler.CountAsync(y => y.Kalori != null);
        ToplamKategoriSayisi = await _context.Kategoriler.CountAsync();

        Kategoriler = await _context.Kategoriler
            .AsNoTracking()
            .OrderBy(k => k.Sira ?? 99)
            .ThenBy(k => k.Ad)
            .Select(k => new KategoriDagilimi(k.Id, k.Ad, k.Sira, k.Yemekler.Count))
            .ToListAsync();

        // 3) Bugünün Menüleri
        var bugunMenuleri = await _context.Menuler
            .AsNoTracking()
            .Include(m => m.MenuYemekleri.OrderBy(my => my.Sira))
                .ThenInclude(my => my.Yemek)
            .Where(m => m.Tarih == Bugun)
            .ToListAsync();

        BugunOgle = bugunMenuleri.FirstOrDefault(m => m.Ogun == OgunTuru.Ogle);
        BugunAksam = bugunMenuleri.FirstOrDefault(m => m.Ogun == OgunTuru.Aksam);
        BugunVejetaryen = bugunMenuleri.FirstOrDefault(m => m.Ogun == OgunTuru.Vejetaryen);

        // 4) Aylık Menü Özeti
        AylikOzetler = menuler
            .GroupBy(m => new { m.Tarih.Year, m.Tarih.Month })
            .OrderByDescending(g => g.Key.Year)
            .ThenByDescending(g => g.Key.Month)
            .Select(g => new AyMenuOzeti(
                g.Key.Year,
                g.Key.Month,
                new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", TrKultur),
                g.Count(x => x.Ogun == OgunTuru.Ogle),
                g.Count(x => x.Ogun == OgunTuru.Aksam),
                g.Count(x => x.Ogun == OgunTuru.Vejetaryen),
                g.Count(x => x.TatilMi),
                g.Count()))
            .ToList();

        // 5) Excel Yükleme Geçmişi
        SonExcelYuklemeleri = await _context.MenuYuklemeleri
            .AsNoTracking()
            .OrderByDescending(x => x.YuklemeTarihi)
            .ThenByDescending(x => x.Id)
            .Take(10)
            .ToListAsync();

        ToplamExcelYuklemeSayisi = await _context.MenuYuklemeleri.CountAsync();
        ToplamUyariSayisi = SonExcelYuklemeleri
            .Where(x => !string.IsNullOrWhiteSpace(x.Uyarilar))
            .Sum(x => x.Uyarilar!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
    }

    public string AyAdiGetir(int yil, int ay)
    {
        if (ay < 1 || ay > 12 || yil < 2000 || yil > 2100)
            return $"{ay:00}/{yil}";
        return new DateTime(yil, ay, 1).ToString("MMMM yyyy", TrKultur);
    }
}
